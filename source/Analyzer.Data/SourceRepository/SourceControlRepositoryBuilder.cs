using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.SourceRepository;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer.Data.SourceRepository
{
    public class SourceControlRepositoryBuilder
    {
        private string _repoPath;
        private DateTime _start;
        private DateTime _end;
        private int _workWeekHours;
        private double _workingDaysPerWeek;
        private string _branch;
        private bool _isEntireHistory;
        private readonly List<string> _ignorePatterns;
        private readonly List<DayOfWeek> _weekends;
        private readonly List<Collaberation> _collaberations;
        private bool _ignoreComments;

        public SourceControlRepositoryBuilder()
        {
            _workWeekHours = 40;
            _workingDaysPerWeek = 5;
            _branch = Branches.Master.Value;
            _ignorePatterns = new List<string>();
            _weekends = new List<DayOfWeek>();
            _collaberations = new List<Collaberation>();
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

        public SourceControlRepositoryBuilder WithWorkingDaysPerWeek(double workingDaysPerWeek)
        {
            _workingDaysPerWeek = workingDaysPerWeek;
            return this;
        }

        public SourceControlRepositoryBuilder WithBranch(string branch)
        {
            _branch = branch;
            return this;
        }

        public SourceControlRepositoryBuilder WithIgnorePatterns(IEnumerable<string> patterns)
        {
            if (patterns == null)
            {
                return this;
            }

            _ignorePatterns.AddRange(patterns);
            return this;
        }

        public SourceControlRepositoryBuilder WithEntireHistory()
        {
            _isEntireHistory = true;
            return this;
        }

        public SourceControlRepositoryBuilder WithWeekends(IEnumerable<DayOfWeek> days)
        {
            _weekends.AddRange(days);
            return this;
        }

        public SourceControlRepositoryBuilder WithCollaberation(DateTime date, params Author[] author)
        {
            _collaberations.Add(new Collaberation());
            return this;
        }

        public object WithAlias(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        public SourceControlRepositoryBuilder WithIgnoreComments(bool ignoreComments)
        {
            _ignoreComments = ignoreComments;
            return this;
        }

        public ISourceControlRepository Build()
        {
            if (NotValidGitRepository(_repoPath))
            {
                throw new Exception($"Invalid path [{_repoPath}]");
            }

            var repository = new Repository(_repoPath);
            if (InvalidBranchName(repository))
            {
                throw new Exception($"Invalid branch [{_branch}]");
            }

            var reportRange = new ReportingPeriod { Start = _start, End = _end, HoursPerWeek = _workWeekHours, DaysPerWeek = _workingDaysPerWeek, Weekends = _weekends };

            if (_isEntireHistory)
            {
                MakeRangeEntireHistory(repository, reportRange);
            }

            return new SourceControlRepository(repository, reportRange, _branch, _ignorePatterns, _collaberations, _ignoreComments);
        }

        private void MakeRangeEntireHistory(Repository repository, ReportingPeriod reportRange)
        {
            var commits = GetCommitsForSelectedBranch(repository);

            reportRange.Start = GetFirstCommit(commits);
            reportRange.End = GetLastCommit(commits);
        }

        private ICommitLog GetCommitsForSelectedBranch(Repository repository)
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = repository.Branches[_branch]
            };

            if (Branches.MasterNotSelected(_branch))
            {
                filter.ExcludeReachableFrom = repository.Branches[Branches.Master.Value];
            }

            var commitLog = repository.Commits.QueryBy(filter);
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
    }
}