using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain;
using LibGit2Sharp;

namespace Analyzer.Data
{
    public class SourceControlRepository : ISourceControlRepository
    {
        private readonly Repository _repository;
        private readonly string _branch;
        private readonly List<string> _ignorePatterns;

        public ReportingPeriod ReportingRange { get; private set; }

        public SourceControlRepository(Repository repository, ReportingPeriod reportingPeriod, string branch, List<string> ignorePatterns)
        {
            _repository = repository;
            _branch = branch;
            _ignorePatterns = ignorePatterns;

            ReportingRange = reportingPeriod;
        }

        public IEnumerable<Author> List_Authors(List<Alias> aliases)
        {
            var authors = List_Authors();

            // todo : if null or empty
            var authorMap = new Dictionary<Guid, List<string>>();
            // iterate over each entry
            // create a dictionay with alias.id and email that matched developer list
            // if not in alias list add as new alias
            // then combine all keys into a new result
            foreach (var author in authors)
            {
                
            }

            return authors;
        }

        public IEnumerable<Author> List_Authors()
        {
            var authors = GetCommits()
                            .GroupBy(x=>x.Author.Email)
                            .Select(x=>x.First())
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
                var stats = new DeveloperStats {Author = developer,
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

        public int Period_Active_Days(Author author)
        {
            var activeDays = GetCommits()
                .Where(x => author.Emails.Contains(x.Author.Email))
                .Select(x => new
                 {
                     x.Author.When.UtcDateTime.Date
                 }).GroupBy(x => x.Date)
                .Select(x => x.First());

            return activeDays.Count();
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
            }
            
            return Math.Round(totalScore/100,2);
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

                result.TotalFiles += 1;
                result.TotalEditLocations += (file.Patch.Split("@@").Length - 1) / 2;
                result.TotalLinesEdited += file.LinesAdded + file.LinesDeleted;
                if (file.Status == ChangeKind.Modified)
                {
                    result.TotalLinesOfOldCode += file.LinesAdded + file.LinesDeleted;
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
                .OrderBy(x=>x.Author.When.Date);
            foreach (var commit in developerCommits)
            {
                if (!commit.Parents.Any())
                {
                    var stats = _repository.Diff.Compare<PatchStats>(null, commit.Tree);
                    result.Added += stats.TotalLinesAdded;
                    result.Removed += stats.TotalLinesDeleted;
                    continue;
                }

                foreach (var parent in commit.Parents)
                {
                    var stats = _repository.Diff.Compare<PatchStats>(parent.Tree, commit.Tree);
                    result.Added += stats.TotalLinesAdded;
                    result.Removed += stats.TotalLinesDeleted;
                }
            }

            var productionLinesPerHour = Calculate_Lines_Per_Hour(developer, result.Added - result.Removed);
            result.ChangePerHour = Calculate_Lines_Per_Hour(developer, result.TotalLines);
            result.Rtt100 = Math.Round(100.0 / result.ChangePerHour,2);
            result.Ptt100 = Math.Round(100.0 / productionLinesPerHour ,2);

            return result;
        }

        private double Calculate_Lines_Per_Hour(Author developer, double linesChanged)
        {
            var periodHoursWorked = ReportingRange.HoursPerWeek * Period_Active_Days(developer);
            var linesPerHour = (linesChanged / periodHoursWorked);
            return Math.Round(linesPerHour,2);
        }

        private IEnumerable<Commit> GetCommits()
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = _repository.Branches[_branch]
            };

            if(_branch != "HEAD")
            {
                filter.ExcludeReachableFrom = _repository.Head;
            }

            var commitLog = _repository.Commits.QueryBy(filter);

            return commitLog
                .Where(x => x.Author.When.Date >= ReportingRange.Start.Date &&
                        x.Author.When.Date <= ReportingRange.End.Date);
        }
    }
}