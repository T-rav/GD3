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

        public IEnumerable<object> ListAuthors()
        {
            var commits = _repository.Head.Commits;
            var authors = commits.Select(x => new Author
            {
                Name = x.Author.Name,
                Email = x.Author.Email
            }).GroupBy(x => x.Name).Select(x => x.First());
            return authors;
        }

        public int PeriodActiveDays(Author author)
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
    }
}