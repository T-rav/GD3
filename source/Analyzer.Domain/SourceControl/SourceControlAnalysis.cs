using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Data.Developer;
using Analyzer.Domain.Developer;
using LibGit2Sharp;

namespace Analyzer.Domain.SourceControl
{
    // todo : this object sits in a funny place - ideally we have this data in a DB and can get a repo to create it
    // but for now it shall sit in the domain
    public class SourceControlAnalysis : IDisposable
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

            return new CodeAnalysis(authors, commits, _context);
        }

        private IList<Commit> Fetch_Commits(IList<Author> authors)
        {
            var result = new List<Commit>();

            var commits = Get_Commits(_context.ReportRange.Start, _context.ReportRange.End);
            foreach (var commit in commits)
            {
                if (First_Commit(commit))
                {
                    var firstCommit = Convert_First_Commit(commit,authors);
                    result.Add(firstCommit);
                    continue;
                }

                var convertedCommits = Convert_Commit(authors, commit);
                result.Add(convertedCommits);
            }

            return result;
        }

        private Commit Convert_Commit(IList<Author> authors, LibGit2Sharp.Commit commit)
        {
            var result = new Commit
            {
                Author = Find_Commit_Author(authors, commit),
                When = commit.Committer.When.DateTime,
            };

            foreach (var commitParent in commit.Parents)
            {
                var fileChanges = _repository.Diff.Compare<LibGit2Sharp.Patch>(commitParent.Tree, commit.Tree);
                foreach (var fileChange in fileChanges)
                {
                    result.Patch.Add(new Patch
                    {
                        LinesAdded = fileChange.LinesAdded,
                        LinesRemoved = fileChange.LinesDeleted,
                        Contents = fileChange.Patch,
                        ChangeType = Set_Status(fileChange.Status)
                    });
                }
            }

            return result;
        }

        private ChangeType Set_Status(ChangeKind fileChangeStatus)
        {
            var mapping = new Dictionary<ChangeKind, ChangeType>
            {
                {ChangeKind.Added,ChangeType.Added },
                {ChangeKind.Modified, ChangeType.Modified},
                {ChangeKind.Conflicted,ChangeType.Conflicted},
                {ChangeKind.Copied, ChangeType.Copied },
                {ChangeKind.Deleted, ChangeType.Deleted },
                {ChangeKind.Ignored, ChangeType.Ignored},
                {ChangeKind.Renamed, ChangeType.Renamed },
                {ChangeKind.Unmodified, ChangeType.Unmodified},
                {ChangeKind.Unreadable, ChangeType.Unreadable },
                {ChangeKind.Untracked,ChangeType.Untracked}
            };

            return mapping[fileChangeStatus];
        }

        private Commit Convert_First_Commit(LibGit2Sharp.Commit commit, IList<Author> authors)
        {
            var result = new Commit();

            var fileChanges = _repository.Diff.Compare<PatchStats>(null, commit.Tree);

            foreach (var fileChange in fileChanges)
            {
                result.Patch.Add(new Patch
                {
                    LinesRemoved = fileChange.LinesDeleted,
                    LinesAdded = fileChange.LinesAdded,
                    Contents = string.Empty, // todo : get the contents properly,
                    ChangeType = ChangeType.Added
                });
            }
            
            return result;
        }

        private bool First_Commit(LibGit2Sharp.Commit commit)
        {
            return !commit.Parents.Any();
        }

        private static Author Find_Commit_Author(IList<Author> authors, LibGit2Sharp.Commit commit)
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

        private IEnumerable<LibGit2Sharp.Commit> Get_Commits(DateTime start, DateTime end)
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