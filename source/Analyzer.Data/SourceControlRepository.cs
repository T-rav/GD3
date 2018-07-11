using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Analyzer.Domain;
using LibGit2Sharp;

namespace Analyzer.Data
{
    internal class SourceControlRepository : ISourceControlRepository
    {
        private readonly Repository _repository;
        private readonly ReportingPeriod _reportingPeriod;

        public SourceControlRepository(Repository repository, ReportingPeriod reportingPeriod)
        {
            _repository = repository;
            _reportingPeriod = reportingPeriod;
        }

        public IEnumerable<Author> List_Authors()
        {
            var commits = _repository.Head.Commits;
            var authors = commits.Select(x => new Author
            {
                Name = x.Author.Name,
                Email = x.Author.Email
            }).GroupBy(x => x.Name).Select(x => x.First());
            return authors;
        }

        public int Period_Active_Days(Author author)
        {
            var activeDays = _repository.Head.Commits
                .Where(x => x.Author.Email == author.Email 
                            && (x.Author.When.Date >= _reportingPeriod.Start.Date && x.Author.When.Date <= _reportingPeriod.End.Date))
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
            var weeks = _reportingPeriod.Period_Weeks();
            return Math.Round(activeDays / weeks, 2);
        }

        public double Commits_Per_Day(Author author)
        {
            var totalCommits = _repository.Head.Commits.Count(x => x.Author.Email == author.Email);
            var totalWorkingDays = _reportingPeriod.Period_Working_Days();

            return Math.Round(totalCommits / totalWorkingDays, 2);
        }

        public List<DeveloperStats> Build_Individual_Developer_Stats(IEnumerable<Author> authors)
        {
            var result = new List<DeveloperStats>();
            foreach (var developer in authors)
            {
                var stats = new DeveloperStats {Author = developer,
                                                PeriodActiveDays = Period_Active_Days(developer),
                                                ActiveDaysPerWeek = Active_Days_Per_Week(developer),
                                                CommitsPerDay = Commits_Per_Day(developer),
                                                Impact = Impact(developer),
                                                LinesOfChangePerHour = Lines_Of_Change_Per_Hour(developer)
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
        private double Impact(Author developer)
        {
            var totalScore = 0.0;
            var developerCommits = _repository.Commits
                .Where(x => x.Author.Email == developer.Email
                            && x.Author.When > _reportingPeriod.Start.Date
                            && x.Author.When < _reportingPeriod.End.Date).OrderBy(x => x.Author.When.Date);
            foreach (var commit in developerCommits)
            {
                foreach (var parent in commit.Parents)
                {
                    var fileChanges = _repository.Diff.Compare<Patch>(parent.Tree, commit.Tree);
                    foreach (var file in fileChanges)
                    {
                        var linesChanged = file.LinesDeleted + file.LinesAdded;
                        var changeLocations = (file.Patch.Split("@@").Length-1)/2;

                        var impactScore = ((double)changeLocations/linesChanged);
                        if (!impactScore.Equals(Double.NaN))
                        {
                            var multiplier = 1.0;
                            if (file.Status == ChangeKind.Modified)
                            {
                                multiplier = 1.5;
                            }
                            totalScore += impactScore * multiplier;
                        }
                    }
                }
            }
            
            return Math.Round(totalScore/100,2);
        }

        private double Lines_Of_Change_Per_Hour(Author developer)
        {
            var linesChanged = 0.0;

            var developerCommits = _repository.Commits
                .Where(x => x.Author.Email == developer.Email
                            && x.Author.When > _reportingPeriod.Start.Date
                            && x.Author.When < _reportingPeriod.End.Date).OrderBy(x=>x.Author.When.Date);
            foreach (var commit in developerCommits)
            {
                foreach (var parent in commit.Parents)
                {
                    var stats = _repository.Diff.Compare<PatchStats>(parent.Tree, commit.Tree);
                    linesChanged += stats.TotalLinesAdded + stats.TotalLinesDeleted;
                }
            }
            
            var periodHoursWorked = _reportingPeriod.HoursPerWeek * Period_Active_Days(developer);
            var linesPerHour = (linesChanged / periodHoursWorked);

            return Math.Round(linesPerHour, 2);
        }
    }
}