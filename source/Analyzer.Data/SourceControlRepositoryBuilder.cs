using System;
using Analyzer.Domain;
using LibGit2Sharp;

namespace Analyzer.Data
{
    public class SourceControlRepositoryBuilder
    {
        private string _repoPath;

        public SourceControlRepositoryBuilder WithPath(string repoPath)
        {
            _repoPath = repoPath;
            return this;
        }

        public ISourceControlRepository Build()
        {
            return NotValidGitRepository(_repoPath) ? 
                throw new Exception($"Invalid path [{_repoPath}]") : 
                new SourceControlRepository(new Repository(_repoPath));
        }

        private bool NotValidGitRepository(string repository)
        {
            return !Repository.IsValid(repository);
        }

    }
}