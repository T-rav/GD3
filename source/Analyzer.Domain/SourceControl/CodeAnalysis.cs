using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Developer;
using Analyzer.Domain.SourceControlV2;

namespace Analyzer.Domain.SourceControl
{
    public class CodeAnalysis
    {
        public IList<Author> Authors { get; }
        public IList<CommitStat> CommitStats { get; }
        public AnalysisContext AnalysisContext { get; }

        public CodeAnalysis(IList<Author> authors, IList<CommitStat> commitStats, AnalysisContext context)
        {
            Authors = authors;
            CommitStats = commitStats;
            AnalysisContext = context;
        }

        public IList<IndividualPeriodStats> Individual_Period_Stats()
        {
            var result = new List<IndividualPeriodStats>();
            foreach (var author in Authors)
            {
                // todo : make stats object for each? Or rules to build IStat objects
                var activeDays = Fetch_Active_Days(author);
                var averageCommitsPerDay = Fetch_Average_Commits_Per_Day(author, activeDays);
                var ptt100 = Calculate_Ptt100(author, activeDays);

                result.Add(new IndividualPeriodStats
                {
                    Author = author,
                    ActiveDays = activeDays,
                    AverageCommitsPerDay = averageCommitsPerDay,
                    Ptt100 = ptt100
                });
            }

            return result;
        }

        private double Fetch_Average_Commits_Per_Day(Author author, int activeDays)
        {
            var totalCommits = CommitStats.Count(x => x.Author == author);
            if (activeDays == 0 || totalCommits == 0)
            {
                return 0.0;
            }

            var averageCommitsPerDay = (double)totalCommits / activeDays;
            return Math.Round(averageCommitsPerDay, 2);
        }

        private int Fetch_Active_Days(Author author)
        {
            var activeDays = CommitStats.Where(x => x.Author == author)
                .Select(x => new
                {
                    x.When
                })
                .GroupBy(x => x.When)
                .Count();
            return activeDays;
        }

        private double Calculate_Ptt100(Author author, int activeDays)
        {
            var hundredLines = 100.00;
            var productionLinesPerHour = Lines_Per_Hour(author, activeDays);
            var result = Math.Abs(Math.Round(hundredLines / productionLinesPerHour, 2));
            if (double.IsPositiveInfinity(result))
            {
                return 0;
            }

            return result;
        }

        private double Lines_Per_Hour(Author author, int activeDays)
        {  
            var linesOfChange = Author_Lines_Of_Change(author);
            var hoursPerDay = AnalysisContext.ReportRange.HoursPerWeek / AnalysisContext.ReportRange.DaysPerWeek;
            var periodHoursWorked = hoursPerDay / activeDays;
            var linesPerHour = (linesOfChange / periodHoursWorked);

            if (double.IsNaN(linesPerHour))
            {
                linesPerHour = 0;
            }
            return Math.Round(linesPerHour, 2);
        }

        private int Author_Lines_Of_Change(Author author)
        {
            var authorCommits = CommitStats.Where(x => x.Author == author).ToList();
            var linesAdded = authorCommits.Sum(x => x.Patch.LinesAdded);
            var linesRemoved = authorCommits.Sum(x => x.Patch.LinesRemoved);
            var periodLinesOfChange = linesAdded - linesRemoved;

            return periodLinesOfChange;
        }
    }
}