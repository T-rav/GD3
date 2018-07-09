using System;
using Analyzer.Domain;
using LibGit2Sharp;

namespace Analyzer.Data
{
    public class SourceControlRepositoryBuilder
    {
        private string _repoPath;
        private DateTime _start;
        private DateTime _end;

        public SourceControlRepositoryBuilder WithPath(string repoPath)
        {
            _repoPath = repoPath;
            return this;
        }

        public SourceControlRepositoryBuilder WithRange(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
            return this;
        }

        public ISourceControlRepository Build()
        {
            if (NotValidGitRepository(_repoPath))
            {
                throw new Exception($"Invalid path [{_repoPath}]");
            }

            var reportRange = new ReportingPeriod {Start = _start, End = _end};
            var repository = new Repository(_repoPath);

            return new SourceControlRepository(repository, reportRange);
        }

        private bool NotValidGitRepository(string repository)
        {
            return !Repository.IsValid(repository);
        }

    }
}