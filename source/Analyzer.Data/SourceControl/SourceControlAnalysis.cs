using Analyzer.Data.Developer;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.Team;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.SourceControl;

namespace Analyzer.Data.SourceControl
{
    public class SourceControlAnalysis : ISourceControlAnalysis
    {
        private readonly Repository _repository;
        private readonly Aliases _aliases;
        private readonly AnalysisContext _context;

        public ReportingPeriod ReportingRange => _context.ReportRange;

        public SourceControlAnalysis(Repository repository, Aliases aliases, AnalysisContext context)
        {
            _repository = repository;
            _context = context;
            _aliases = aliases;
        }

        public IList<Author> List_Authors()
        {
            var authors = List_Repository_Authors(_context.ReportRange.Start, _context.ReportRange.End);
            return _aliases.Map_To_Authors(authors);
        }

        private IList<Author> List_Repository_Authors(DateTime start, DateTime end)
        {
            var authors = GetCommits(start, end)
                .GroupBy(x => x.Author.Email)
                .Select(x => x.First())
                .Select(x => new Author
                {
                    Name = x.Author.Name,
                    Emails = new List<string> { x.Author.Email }
                }).ToList();
            return authors;
        }

        public IList<DeveloperStats> Build_Individual_Developer_Stats(IList<Author> authors)
        {
            var result = Build_Stats_For_Range(authors, _context.ReportRange.Start, _context.ReportRange.End);

            return result;
        }
        
        public TeamStatsCollection Build_Team_Stats()
        {
            var teamStats = new List<TeamStats>();

            var dateRange = _context.ReportRange.Generate_Dates_For_Range();
            var commits = GetCommits(_context.ReportRange.Start, _context.ReportRange.End);

            foreach (var date in dateRange)
            {
                var daysCommits = commits
                                  .Where(x => x.Author.When.Date == date.Date);
                var developers = daysCommits
                                .GroupBy(x => x.Author.Email)
                                .Select(x => x.First());
                teamStats.Add(new TeamStats
                {
                    DateOf = date.Date,
                    TotalCommits = daysCommits.Count(),
                    ActiveDevelopers = developers.Count()
                });
            }

            return new TeamStatsCollection(teamStats, _context.ReportRange.Weekends);
        }

        public IList<DailyDeveloperStats> Build_Daily_Individual_Developer_Stats(List<Author> authors)
        {
            var result = new List<DailyDeveloperStats>();
            var days  = _context.ReportRange.Generate_Dates_For_Range();

            foreach (var day in days)
            {
                var individualDeveloperStats = Build_Stats_For_Range(authors, day.Date, day.Date.Date);
               result.Add(new DailyDeveloperStats
               {
                   Date = day.Date,
                   Stats = individualDeveloperStats
               });
            }
           
            return result;
        }

        public int Period_Active_Days(Author author)
        {
            return Period_Active_Days_For_Range(author, _context.ReportRange.Start, _context.ReportRange.End);
        }

        public double Active_Days_Per_Week(Author author)
        {
            return Active_Days_Per_Week_For_Range(author, _context.ReportRange.Start, _context.ReportRange.End);
        }

        public double Commits_Per_Day(Author author)
        {
            return Commits_Per_Day_For_Range(author, _context.ReportRange.Start, _context.ReportRange.End);
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
        
        private double Commits_Per_Day_For_Range(Author author, DateTime start, DateTime end)
        {
            var periodActiveDays = (double)Period_Active_Days_For_Range(author, start, end);
            var totalCommits = GetCommits(start, end)
                .Count(x => author.Emails.Contains(x.Author.Email));

            if (periodActiveDays == 0 || totalCommits == 0)
            {
                return 0.0;
            }

            return Math.Round(totalCommits / periodActiveDays, 2);
        }

        private double Active_Days_Per_Week_For_Range(Author author, DateTime start, DateTime end)
        {
            var activeDays = Period_Active_Days_For_Range(author, start, end);
            var weeks = _context.ReportRange.Period_Weeks();
            return Math.Round(activeDays / weeks, 2);
        }

        private int Period_Active_Days_For_Range(Author author, DateTime start, DateTime end)
        {
            // todo : this needs to account for aliases when combining, and only count one for the group?
            var activeDays = GetCommits(start, end)
                .Where(x => author.Emails.Contains(x.Author.Email))
                .Select(x => new
                {
                    x.Author.When.Date
                }).GroupBy(x => x.Date)
                .Select(x => x.First());

            var activeDaysCount = activeDays.Count();

            return activeDaysCount;
        }

        private List<DeveloperStats> Build_Stats_For_Range(IList<Author> authors, DateTime start, DateTime end)
        {
            var result = new List<DeveloperStats>();
            foreach (var developer in authors)
            {
                var changeStats = Change_Stats(developer, start, end);
                var stats = new DeveloperStats
                {
                    Author = developer,
                    PeriodActiveDays = Period_Active_Days_For_Range(developer, start, end),
                    ActiveDaysPerWeek = Active_Days_Per_Week_For_Range(developer, start, end),
                    CommitsPerDay = Commits_Per_Day_For_Range(developer, start, end),
                    Impact = Impact(developer, start, end),
                    LinesOfChangePerHour = changeStats.ChangePerHour,
                    LinesAdded = changeStats.Added,
                    LinesRemoved = changeStats.Removed,
                    Churn = changeStats.Churn,
                    Rtt100 = changeStats.Rtt100,
                    Ptt100 = changeStats.Ptt100
                };
                result.Add(stats);
            }

            return result;
        }

        /*
         *  The amount of code in the change
            What percentage of the work is edits to old code
            The surface area of the change (think ‘number of edit locations’)
            The number of files affected
            The severity of changes when old code is modified   
         */

        // 13-08 Tusani v Sindi stats
        private double Impact(Author developer, DateTime start, DateTime end)
        {
            var totalScore = 0.0;
            var developerCommits = GetCommits(start, end)
                                   .Where(x => developer.Emails.Contains(x.Author.Email));
            foreach (var commit in developerCommits)
            {
                foreach (var parent in commit.Parents)
                {
                    var fileChanges = _repository.Diff.Compare<Patch>(parent.Tree, commit.Tree);
                    var changeImpactData = CalculateImpactStats(fileChanges);
                    totalScore += changeImpactData.Calculate();
                }

                // todo : debugging log for high impact scoring
                //if (developer.Emails.Contains("thabanitembe@hotmail.com"))
                //{
                //    //todo : write out debugging data
                //    var lineToWrite = $"{developer.Name}|{commit.Author.When.DateTime}|{totalScore}" + Environment.NewLine;
                //    File.AppendAllText("D:\\Systems\\debug.txt", lineToWrite);
                //}

            }

            return Math.Round(totalScore, 3);
        }

        private Impact CalculateImpactStats(Patch fileChanges)
        {
            var result = new Impact();
            foreach (var file in fileChanges)
            {
                if (FileShouldBeIgnored(file))
                {
                    continue;
                }

                var totalCommentedOutlinesToRemove = TotalCommentedOutLinesToDeduct(fileChanges);
                var totalLinesEdited = ((file.LinesAdded - totalCommentedOutlinesToRemove) + (file.LinesDeleted - totalCommentedOutlinesToRemove));

                result.TotalFiles += 1;
                result.TotalEditLocations += (file.Patch.Split("@@").Length - 1) / 2;

                result.TotalLinesEdited += totalLinesEdited;
                if (file.Status == ChangeKind.Modified)
                {
                    result.TotalLinesOfOldCode += totalLinesEdited;
                }
            }

            return result;
        }

        private bool FileShouldBeIgnored(PatchEntryChanges file)
        {
            return _context.IgnorePatterns.Any(pattern => file.Path.Contains(pattern));
        }

        private LinesOfChange Change_Stats(Author developer, DateTime start, DateTime end)
        {
            var stats = new LinesOfChange();

            var developerCommits = GetCommits(start, end)
                .Where(x => developer.Emails.Contains(x.Author.Email))
                .OrderBy(x => x.Author.When.Date);

            foreach (var commit in developerCommits)
            {
                if (FirstCommit(commit))
                {
                    BuildFirstCommitStats(commit, stats);
                    continue;
                }

                BuildCommitStats(commit, stats);
            }

            var hundredLines = 100.00;
            var productionLinesPerHour = Calculate_Lines_Per_Hour(developer, stats.Added - stats.Removed, start, end);
            stats.ChangePerHour = Calculate_Lines_Per_Hour(developer, stats.TotalLines, start, end);
            stats.Rtt100 = Calculate_Rtt100(hundredLines, stats.ChangePerHour);
            stats.Ptt100 = Calculate_Ptt100(hundredLines, productionLinesPerHour);


            return stats;
        }

        private static double Calculate_Ptt100(double hundredLines, double productionLinesPerHour)
        {
            var result = Math.Abs(Math.Round(hundredLines / productionLinesPerHour, 2));
            if (double.IsPositiveInfinity(result))
            {
                return 0;
            }

            return result;
        }

        private static double Calculate_Rtt100(double hundredLines, double changePerHour)
        {
            var result = Math.Round(hundredLines / changePerHour, 2);
            if (double.IsPositiveInfinity(result))
            {
                return 0;
            }

            return result;
        }

        private void BuildCommitStats(Commit commit, LinesOfChange result)
        {
            foreach (var parent in commit.Parents)
            {
                var fileChanges = _repository.Diff.Compare<Patch>(parent.Tree, commit.Tree);
                var totalCommentedOutlinesToRemove = TotalCommentedOutLinesToDeduct(fileChanges);

                foreach (var file in fileChanges)
                {
                    if (FileShouldBeIgnored(file))
                    {
                        continue;
                    }

                    result.Added += file.LinesAdded - totalCommentedOutlinesToRemove;
                    result.Removed += file.LinesDeleted;
                }
            }
        }

        private int TotalCommentedOutLinesToDeduct(Patch fileChanges)
        {
            if (!_context.IgnoreComments)
            {
                return 0;
            }

            var commentedOutLines = fileChanges.Content.Split("+//"); // todo : make comment char configurable
            return commentedOutLines.Length - 1;
        }

        private void BuildFirstCommitStats(Commit commit, LinesOfChange result)
        {
            var stats = _repository.Diff.Compare<PatchStats>(null, commit.Tree);
            result.Added += stats.TotalLinesAdded;
            result.Removed += stats.TotalLinesDeleted;
        }

        private bool FirstCommit(Commit commit)
        {
            return !commit.Parents.Any();
        }

        private double Calculate_Lines_Per_Hour(Author developer, double linesChanged, DateTime start, DateTime end)
        {
            var hoursPerDay = _context.ReportRange.HoursPerWeek / _context.ReportRange.DaysPerWeek;
            var periodHoursWorked = hoursPerDay * Period_Active_Days_For_Range(developer, start, end);
            var linesPerHour = (linesChanged / periodHoursWorked);
            if (double.IsNaN(linesPerHour))
            {
                linesPerHour = 0;
            }
            return Math.Round(linesPerHour, 2);
        }

        private IEnumerable<Commit> GetCommits(DateTime start, DateTime end)
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = _repository.Branches[_context.Branch.Value]
            };

            if (Branches.MasterNotSelected(_context.Branch.Value))
            {
                filter.ExcludeReachableFrom = _repository.Branches[Branches.Master.Value];
            }

            var commitLog = _repository.Commits.QueryBy(filter);

            var commits = commitLog
                .Where(x => x.Author.When.Date >= start &&
                        x.Author.When.Date <= end);

            return commits;
        }
    }
}