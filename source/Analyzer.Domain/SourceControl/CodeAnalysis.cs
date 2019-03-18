using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.SourceControl.Stats;

namespace Analyzer.Domain.SourceControl
{
    public class CodeAnalysis
    {
        public IList<Author> Authors { get; }
        public IList<Commit> Commits { get; }
        public AnalysisContext AnalysisContext { get; }

        public CodeAnalysis(IList<Author> authors, IList<Commit> commits, AnalysisContext context)
        {
            Authors = authors;
            Commits = commits ?? new List<Commit>();
            AnalysisContext = context;
        }

        public CodeStats Build_Stats()
        {
            var commitStats = Build_Commit_Stats(Commits);
            var dailyDeveloperStats = Build_Daily_Developer_Stats(commitStats, Authors, AnalysisContext.ReportRange);

            return new CodeStats
            {
                ReportingPeriod = AnalysisContext.ReportRange,
                CommitStats = commitStats,
                DeveloperStatsPerDay = dailyDeveloperStats,
                TeamStatsPerDay = Build_Daily_Team_Stats(dailyDeveloperStats, AnalysisContext.ReportRange)
            };
        }

        private List<TeamStatsForDay> Build_Daily_Team_Stats(List<DeveloperStatsForDay> dailyDeveloperStats, ReportingPeriod reportingRange)
        {
            var result = new List<TeamStatsForDay>();

            var daysInRange = reportingRange.Generate_Dates_For_Range();
            foreach (var day in daysInRange)
            {
                var statsForDay = dailyDeveloperStats.Where(x=>x.When == day.Date).Where(x=>x.Commits > 0);
                var activeDevelopers = statsForDay.GroupBy(x=>x.Author).Count();

                result.Add(new TeamStatsForDay
                {
                    When = day.Date,
                    ActiveDevelopers = activeDevelopers,
                    Commits = statsForDay.Sum(x=>x.Commits),
                    Impact = Math.Round(statsForDay.Sum(x=>x.Impact),3),
                    Churn = Math.Round(statsForDay.Sum(x=>x.Churn),3),
                    Ptt100 = Calculate_Ptt100_For_Team(statsForDay, activeDevelopers),
                    RiskFactor = Calculate_Team_RiskFactor(statsForDay, activeDevelopers)
                });
            }

            return result;
        }

        private static double Calculate_Ptt100_For_Team(IEnumerable<DeveloperStatsForDay> statsForDay, int activeDevelopers)
        {
            if (activeDevelopers == 0)
            {
                return 0;
            }

            return Math.Round(statsForDay.Sum(x=>x.Ptt100) / activeDevelopers,3);
        }

        private static double Calculate_Team_RiskFactor(IEnumerable<DeveloperStatsForDay> statsForDay, int activeDevelopers)
        {
            if (activeDevelopers == 0)
            {
                return 0;
            }

            return Math.Round(statsForDay.Sum(x=>x.RiskFactor) / activeDevelopers,3);
        }

        private List<DeveloperStatsForDay> Build_Daily_Developer_Stats(IList<CommitStat> commits, IList<Author> authors, ReportingPeriod analysisContextReportRange)
        {
            var result = new List<DeveloperStatsForDay>();

            var daysInRange = analysisContextReportRange.Generate_Dates_For_Range();
            foreach (var day in daysInRange)
            {
                foreach (var author in authors)
                {
                    var stats = commits.Where(x => x.When.Date == day.Date).Where(x => x.Author == author);
                    var linesOfChange = stats.Sum(x => x.LinesRemoved + x.LinesAdded);
                    var productionLinesOfChange = stats.Sum(x => x.LinesAdded - x.LinesRemoved);

                    var totalCommits = stats.Count();

                    result.Add(new DeveloperStatsForDay
                    {
                        Author = author,
                        When = day.Date,
                        Commits = totalCommits,
                        Impact = stats.Sum(x=>x.Impact(AnalysisContext.IgnorePatterns, AnalysisContext.IgnoreComments)),
                        Churn = stats.Sum(x=>x.Churn()),
                        Ptt100 = Calculate_Ptt100(productionLinesOfChange),
                        RiskFactor = Risk_Factor(linesOfChange, totalCommits)
                    });
                }
            }

            return result;
        }

        private double Calculate_Ptt100(int productionLinesOfChange)
        {
            var hundredLines = 100.00;
            var productionLinesPerHour = Change_Per_Hour(productionLinesOfChange, 8); // todo : make this configurable
            var result = Math.Abs(Math.Round(hundredLines / productionLinesPerHour, 2));
            if (double.IsPositiveInfinity(result))
            {
                return 0;
            }

            return result;
        }

        private double Risk_Factor(int linesOfChange, double totalCommits)
        {
            var result = Math.Round((linesOfChange / totalCommits), 2);
            if (result.Equals(double.NaN) || result.Equals(double.PositiveInfinity))
            {
                return 0.0;
            }

            return result;
        }

        private double Change_Per_Hour(int linesOfChange, double dailyHours)
        {
            var linesPerHour = (linesOfChange / dailyHours);

            return Math.Round(linesPerHour, 2);
        }

        private List<CommitStat> Build_Commit_Stats(IList<Commit> commits)
        {
            var result = commits.Select(commit => new CommitStat(commit)).ToList();

            return result;
        }

        // todo : need a build stats method that makes it all come together
        //public IList<CommitStat> Individual_Period_Stats()
        //{
        //    // todo : rework to build 
        //    var result = new List<CommitStat>();
        //    foreach (var author in Authors)
        //    {
        //        // todo : make stats object for each? Or rules to build IStat objects
        //        var activeDays = Fetch_Active_Days(author);
        //        var averageCommitsPerDay = Fetch_Average_Commits_Per_Day(author, activeDays);
        //        var ptt100 = Calculate_Ptt100(author, activeDays);

        //        // todo : should be daily stats which aggregate up
        //        //result.Add(new CommitStat
        //        //{
        //        //    Author = author,
        //        //    ActiveDays = activeDays,
        //        //    Commits = averageCommitsPerDay,
        //        //    Ptt100 = ptt100
        //        //});
        //    }

        //    return result;
        //}

        //private double Lines_Per_Hour(Author author, int activeDays)
        //{
            //var linesOfChange = Author_Lines_Of_Change(author);
            //var hoursPerDay = AnalysisContext.ReportRange.HoursPerWeek / AnalysisContext.ReportRange.DaysPerWeek;
            //var periodHoursWorked = hoursPerDay / activeDays;
            //var linesPerHour = (linesOfChange / periodHoursWorked);

            //if (double.IsNaN(linesPerHour))
            //{
            //    linesPerHour = 0;
            //}
            //return Math.Round(linesPerHour, 2);
        //}

        //private int Author_Lines_Of_Change(Author author)
        //{
        //    var authorCommits = CommitStats.Where(x => x.Author == author).ToList();
        //    var linesAdded = authorCommits.Sum(x => x.Patch.LinesAdded);
        //    var linesRemoved = authorCommits.Sum(x => x.Patch.LinesRemoved);
        //    var periodLinesOfChange = linesAdded - linesRemoved;

        //    return periodLinesOfChange;
        //}
    }
}