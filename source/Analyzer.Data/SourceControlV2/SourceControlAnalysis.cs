using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Data.Developer;
using Analyzer.Domain.Developer;
using Analyzer.Domain.SourceControl;
using Analyzer.Domain.SourceControlV2;
using LibGit2Sharp;
using ISourceControlAnalysis = Analyzer.Domain.SourceControlV2.ISourceControlAnalysis;

namespace Analyzer.Data.SourceControlV2
{
    public class SourceControlAnalysis : ISourceControlAnalysis
    {
        private readonly Repository _repository;
        private readonly Aliases _aliases;
        private readonly AnalysisContext _context;

        public SourceControlAnalysis(Repository repository, Aliases aliases, AnalysisContext context)
        {
            _repository = repository;
            _aliases = aliases;
            _context = context;
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }

        public CodeAnalysis Run_Analysis()
        {
            var authors = Map_Aliases_To_Authors();
            var commits = Fetch_Commits(authors);
            return new CodeAnalysis
            {
                Authors = authors,
                CommitStats = commits
            };
        }

        private IList<CommitStat> Fetch_Commits(IList<Author> authors)
        {
            var result = new List<CommitStat>();

            var commits = Get_Commits(_context.ReportRange.Start, _context.ReportRange.End);
            foreach (var commit in commits)
            {
                foreach (var commitParent in commit.Parents)
                {
                    var fileChanges = _repository.Diff.Compare<Patch>(commitParent.Tree, commit.Tree);
                    result.Add(new CommitStat
                    {
                        Author = Find_Commit_Author(authors, commit), 
                        When = commit.Committer.When.DateTime,
                        Patch = Create_Commit_Patch(fileChanges)
                    });
                }
            }

            return result;
        }

        private static CommitPatch Create_Commit_Patch(Patch fileChanges)
        {
            return new CommitPatch
            {
                Contents = fileChanges.Content,
                LinesAdded = fileChanges.LinesAdded,
                LinesRemoved = fileChanges.LinesDeleted
            };
        }

        private static Author Find_Commit_Author(IList<Author> authors, Commit commit)
        {
            return authors.FirstOrDefault(x=>x.Emails.Contains(commit.Author.Email));
        }

        private IList<Author> Map_Aliases_To_Authors()
        {
            var authors = List_Repository_Authors(_context.ReportRange.Start, _context.ReportRange.End);
            authors = _aliases.Map_To_Authors(authors);
            return authors;
        }

        private IList<Author> List_Repository_Authors(DateTime start, DateTime end)
        {
            var authors = Get_Commits(start, end)
                .GroupBy(x => x.Author.Email)
                .Select(x => x.First())
                .Select(x => new Author
                {
                    Name = x.Author.Name,
                    Emails = new List<string> { x.Author.Email }
                }).ToList();
            return authors;
        }

        private IEnumerable<Commit> Get_Commits(DateTime start, DateTime end)
        {
            var filter = new CommitFilter
            {
                IncludeReachableFrom = _repository.Branches[_context.Branch.Value]
            };

            if (Branches.MasterNotSelected(_context.Branch.Value))
            {
                filter.ExcludeReachableFrom = _repository.Branches[Branches.Master.Value];
            }

            var commitLog = _repository.Commits.QueryBy(filter);

            var commits = commitLog
                .Where(x => x.Author.When.Date >= start &&
                            x.Author.When.Date <= end);

            return commits;
        }
    }
}