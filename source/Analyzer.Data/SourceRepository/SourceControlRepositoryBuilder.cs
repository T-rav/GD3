﻿using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.SourceRepository;
using LibGit2Sharp;

namespace Analyzer.Data.SourceRepository
{
    public class SourceControlRepositoryBuilder
    {
        private string _repoPath;
        private DateTime _start;
        private DateTime _end;
        private int _workWeekHours;
        private int _workingDaysPerWeek;
        private string _branch;
        private bool _isEntireHistory;
        private readonly List<string> _ignorePatterns;
        private List<DayOfWeek> _weekends;

        public SourceControlRepositoryBuilder()
        {
            _workWeekHours = 40;
            _workingDaysPerWeek = 5;
            _branch = "HEAD";
            _ignorePatterns = new List<string>();
            _weekends = new List<DayOfWeek>();
        }

        public SourceControlRepositoryBuilder WithPath(string repoPath)
        {
            _repoPath = repoPath;
            return this;
        }

        public SourceControlRepositoryBuilder WithRange(DateTime start, DateTime end)
        {
            _start = start;
            _end = end;
            _isEntireHistory = false;
            return this;
        }

        public SourceControlRepositoryBuilder WithWorkingWeekHours(int workWeekHours)
        {
            _workWeekHours = workWeekHours;
            return this;
        }

        public SourceControlRepositoryBuilder WithWorkingDaysPerWeek(int workingDaysPerWeek)
        {
            _workingDaysPerWeek = workingDaysPerWeek;
            return this;
        }

        public SourceControlRepositoryBuilder WithBranch(string branch)
        {
            _branch = branch;
            return this;
        }

        public SourceControlRepositoryBuilder WithIgnorePattern(string pattern)
        {
            _ignorePatterns.Add(pattern);
            return this;
        }

        public SourceControlRepositoryBuilder WithEntireHistory()
        {
            _isEntireHistory = true;
            return this;
        }

        public SourceControlRepositoryBuilder WithWeekend(DayOfWeek day)
        {
            _weekends.Add(day);
            return this;
        }

        public ISourceControlRepository Build()
        {
            if (NotValidGitRepository(_repoPath))
            {
                throw new Exception($"Invalid path [{_repoPath}]");
            }

            var repository = new LibGit2Sharp.Repository(_repoPath);
            if (InvalidBranchName(repository))
            {
                throw new Exception($"Invalid branch [{_branch}]");
            }

            var reportRange = new ReportingPeriod { Start = _start, End = _end, HoursPerWeek = _workWeekHours, DaysPerWeek = _workingDaysPerWeek };

            if (_isEntireHistory)
            {
                MakeRangeEntireHistory(repository, reportRange);
            }

            return new SourceControlRepository(repository, reportRange, _branch, _ignorePatterns);
        }

        private void MakeRangeEntireHistory(LibGit2Sharp.Repository repository, ReportingPeriod reportRange)
        {
            var commits = GetCommitsForSelectedBranch(repository);

            reportRange.Start = GetFirstCommit(commits);
            reportRange.End = GetLastCommit(commits);
        }

        private ICommitLog GetCommitsForSelectedBranch(LibGit2Sharp.Repository repository)
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = repository.Branches[_branch]
            };

            if (_branch != "HEAD")
            {
                filter.ExcludeReachableFrom = repository.Head;
            }

            var commitLog = repository.Commits.QueryBy(filter);
            commitLog.OrderBy(x => x.Author.When.Date);
            return commitLog;
        }

        private static DateTime GetLastCommit(ICommitLog commitLog)
        {
            return commitLog.First().Author.When.Date;
        }

        private static DateTime GetFirstCommit(ICommitLog commitLog)
        {
            return commitLog.Last().Author.When.Date;
        }

        private bool InvalidBranchName(Repository repository)
        {
            return repository.Branches[_branch] == null;
        }

        private bool NotValidGitRepository(string repository)
        {
            return !Repository.IsValid(repository);
        }

        public object WithAlias(string v1, string v2)
        {
            throw new NotImplementedException();
        }

    }
}