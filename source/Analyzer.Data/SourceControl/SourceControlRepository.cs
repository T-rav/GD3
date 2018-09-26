using Analyzer.Data.Developer;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.SourceRepository;
using Analyzer.Domain.Team;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer.Data.SourceControl
{
    public class SourceControlRepository : ISourceControlRepository
    {
        private readonly Repository _repository;
        private readonly string _branch;
        private readonly List<string> _ignorePatterns;
        private readonly bool _ignoreComments;
        private readonly Aliases _aliases;

        public ReportingPeriod ReportingRange { get; }

        public SourceControlRepository(Repository repository, ReportingPeriod reportingPeriod, string branch, List<string> ignorePatterns, bool ignoreComments, Aliases aliases)
        {
            _repository = repository;
            _branch = branch;
            _ignorePatterns = ignorePatterns;
            _ignoreComments = ignoreComments;
            _aliases = aliases;

            ReportingRange = reportingPeriod;
        }

        public IEnumerable<Author> List_Authors()
        {
            var authors = List_Repository_Authors();
            return _aliases.Map_To_Authors(authors);
        }

        private IEnumerable<Author> List_Repository_Authors()
        {
            var authors = GetCommits()
                .GroupBy(x => x.Author.Email)
                .Select(x => x.First())
                .Select(x => new Author
                {
                    Name = x.Author.Name,
                    Emails = new List<string> { x.Author.Email }
                });
            return authors;
        }

        public List<DeveloperStats> Build_Individual_Developer_Stats(IEnumerable<Author> authors)
        {
            var result = new List<DeveloperStats>();
            foreach (var developer in authors)
            {
                var changeStats = Change_Stats(developer);
                var stats = new DeveloperStats
                {
                    Author = developer,
                    PeriodActiveDays = Period_Active_Days(developer),
                    ActiveDaysPerWeek = Active_Days_Per_Week(developer),
                    CommitsPerDay = Commits_Per_Day(developer),
                    Impact = Impact(developer),
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

        public TeamStatsCollection Build_Team_Stats()
        {
            var teamStats = new List<TeamStats>();

            var dateRange = ReportingRange.Generate_Dates_For_Range();
            var commits = GetCommits();

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

            return new TeamStatsCollection(teamStats, ReportingRange.Weekends);
        }

        public int Period_Active_Days(Author author)
        {
            var activeDays = GetCommits()
                .Where(x => author.Emails.Contains(x.Author.Email))
                .Select(x => new
                {
                    x.Author.When.UtcDateTime.Date
                }).GroupBy(x => x.Date)
                .Select(x => x.First());

            var activeDaysCount = activeDays.Count();

            return activeDaysCount;
        }

        public double Active_Days_Per_Week(Author author)
        {
            var activeDays = Period_Active_Days(author);
            var weeks = ReportingRange.Period_Weeks();
            return Math.Round(activeDays / weeks, 2);
        }

        public double Commits_Per_Day(Author author)
        {
            var periodActiveDays = (double)Period_Active_Days(author);
            var totalCommits = GetCommits()
                              .Count(x => author.Emails.Contains(x.Author.Email));

            if (periodActiveDays == 0 || totalCommits == 0)
            {
                return 0.0;
            }

            return Math.Round(totalCommits / periodActiveDays, 2);
        }

        /*
         *  The amount of code in the change
            What percentage of the work is edits to old code
            The surface area of the change (think ‘number of edit locations’)
            The number of files affected
            The severity of changes when old code is modified   
         */

        // 13-08 Tusani v Sindi stats
        private double Impact(Author developer)
        {
            var totalScore = 0.0;
            var developerCommits = GetCommits()
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

            return Math.Round(totalScore, 2);
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
            return _ignorePatterns.Any(pattern => file.Path.Contains(pattern));
        }

        private LinesOfChange Change_Stats(Author developer)
        {
            var result = new LinesOfChange();

            var developerCommits = GetCommits()
                .Where(x => developer.Emails.Contains(x.Author.Email))
                .OrderBy(x => x.Author.When.Date);

            foreach (var commit in developerCommits)
            {
                if (FirstCommit(commit))
                {
                    BuildFirstCommitStats(commit, result);
                    continue;
                }

                BuildCommitStats(commit, result);
            }

            var productionLinesPerHour = Calculate_Lines_Per_Hour(developer, result.Added - result.Removed);
            result.ChangePerHour = Calculate_Lines_Per_Hour(developer, result.TotalLines);
            result.Rtt100 = Math.Round(100.0 / result.ChangePerHour, 2);
            result.Ptt100 = Math.Abs(Math.Round(100.0 / productionLinesPerHour, 2));

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
            if (!_ignoreComments)
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

        private double Calculate_Lines_Per_Hour(Author developer, double linesChanged)
        {
            var periodHoursWorked = ReportingRange.HoursPerWeek * Period_Active_Days(developer);
            var linesPerHour = (linesChanged / periodHoursWorked);
            return Math.Round(linesPerHour, 2);
        }

        private IEnumerable<Commit> GetCommits()
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = _repository.Branches[_branch]
            };

            if (Branches.MasterNotSelected(_branch))
            {
                filter.ExcludeReachableFrom = _repository.Branches[Branches.Master.Value];
            }

            var commitLog = _repository.Commits.QueryBy(filter);

            return commitLog
                .Where(x => x.Author.When.Date >= ReportingRange.Start.Date &&
                        x.Author.When.Date <= ReportingRange.End.Date);
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}