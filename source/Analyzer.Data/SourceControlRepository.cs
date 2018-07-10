using System;
using System.Collections.Generic;
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
            var weeks = _reportingPeriod.Weeks();
            return Math.Round(activeDays / weeks, 2);
        }

        public double Commits_Per_Day(Author author)
        {
            var totalCommits = _repository.Head.Commits.Count(x => x.Author.Email == author.Email);
            var totalWorkingDays = _reportingPeriod.Working_Days();

            return Math.Round((double)totalCommits / totalWorkingDays, 2);
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
                                                Efficiency = 0.0,
                                                Impact = 0.0,
                                                Ptt100 = 0
                };
                result.Add(stats);
            }

            return result;
        }
    }
}