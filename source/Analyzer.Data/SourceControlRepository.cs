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

        public IEnumerable<object> List_Authors()
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
                .Where(x => x.Author.Email == author.Email)
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
            var weeks = Total_Weeks();
            return Math.Round((double)activeDays / weeks, 2);
        }

        public double Commits_Per_Day(Author author)
        {
            var totalCommits = _repository.Head.Commits.Count(x => x.Author.Email == author.Email);
            var totalWorkingDays = Working_Days();

            return Math.Round((double)totalCommits / totalWorkingDays, 2);
        }

        public List<DeveloperStats> Build_Individual_Developer_Stats()
        {
            throw new NotImplementedException();
        }


        private int Working_Days()
        {
            var numberOfWeekends = Total_Weeks();
            var weekendDays = numberOfWeekends * 2;
            var workingDays = _reportingPeriod.Total_Days() - weekendDays;
            return workingDays;
        }

        private int Total_Weeks()
        {
            var totalDays = _reportingPeriod.Total_Days();
            var numberOfWeekends = totalDays / 7;
            return numberOfWeekends;
        }
    }
}