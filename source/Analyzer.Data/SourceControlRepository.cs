using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain;
using Analyzer.Tests;
using LibGit2Sharp;

namespace Analyzer.Data
{
    internal class SourceControlRepository : ISourceControlRepository
    {
        private readonly Repository _repository;

        public SourceControlRepository(Repository repository)
        {
            _repository = repository;
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
            return (double)activeDays / weeks;
        }

        public double Commits_Per_Day(Author author)
        {
            var totalCommits = _repository.Head.Commits.Count(x => x.Author.Email == author.Email);
            var totalWorkingDays = Working_Days();

            return (double)totalCommits / totalWorkingDays;
        }

        private int Working_Days()
        {
            var numberOfWeekends = Total_Weeks();
            var weekendDays = numberOfWeekends * 2;
            var workingDays = Total_Days() - weekendDays;
            return workingDays;
        }

        private int Total_Weeks()
        {
            var totalDays = Total_Days();
            var numberOfWeekends = totalDays / 7;
            return numberOfWeekends;
        }

        private int Total_Days()
        {
            var latestCommit = _repository.Head.Commits.Max(x => x.Author.When.UtcDateTime.Date);
            var firstCommit = _repository.Head.Commits.Min(x => x.Author.When.UtcDateTime.Date);
            var difference = latestCommit.Subtract(firstCommit);
            var activeDays = difference.Days;
            return activeDays;
        }
    }
}