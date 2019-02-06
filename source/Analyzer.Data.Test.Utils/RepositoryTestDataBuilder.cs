using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analyzer.Data.Test.Utils
{
    public class RepositoryTestDataBuilder
    {
        private readonly List<TestCommitWithBranch> _commits;
        private string _branch;
        private bool _buildMasterFirst;
        private string _path;

        public static string MasterBranch = "master";

        public RepositoryTestDataBuilder()
        {
            _path = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            _commits = new List<TestCommitWithBranch>();
        }

        public RepositoryTestDataBuilder Make_Commit(TestCommit commit)
        {
            _commits.Add(new TestCommitWithBranch
            {
                Commit = commit,
                Branch = _branch
            });
            return this;
        }
        
        public RepositoryTestDataBuilder With_Init_Commit_To_Master()
        {
            _commits.Add(new TestCommitWithBranch
            {
                Commit = new TestCommit
                {
                    Name = "dummy-user",
                    Email = "dummy@idiot.io",
                    FileName = "test-file.txt",
                    Lines = new List<string> { "a line" },
                    CommitMessage = "init commit",
                    TimeStamp = DateTime.Today
                },
                Branch = MasterBranch
            });

            return this;
        }

        public RepositoryTestDataBuilder On_Branch(string branch)
        {
            _branch = branch;
            return this;
        }

        public FileSystemTestArtefact Build()
        {
            var rootedPath = Repository.Init(_path, false);
            var repositoryContext = new FileSystemTestArtefact { Path = _path };

            using (var repo = new Repository(rootedPath))
            {

                foreach (var testCommit in _commits)
                {
                    Create_Branch_If_It_Does_Not_Exist(testCommit, repo);
                    Checkout_Branch(testCommit, repo);
                    Populate_File_For_Commit(testCommit.Commit, repo);
                    Commit_File(repo, testCommit.Commit);
                }
            }
            return repositoryContext;
        }

        private static void Checkout_Branch(TestCommitWithBranch testCommit, Repository repo)
        {
            if (Valid_Branch_Name(testCommit))
            {
                var branch = repo.Branches[testCommit.Branch];
                if (branch != null)
                {
                    Commands.Checkout(repo, branch);
                }
            }
        }

        private void Create_Branch_If_It_Does_Not_Exist(TestCommitWithBranch testCommit, Repository repo)
        {
            if (Valid_Branch_Name(testCommit)
                && Not_Master_Branch(testCommit)
                && Branch_Does_Not_Exist(repo, testCommit))
            {
                repo.CreateBranch(_branch);
            }
        }

        private static bool Branch_Does_Not_Exist(Repository repo, TestCommitWithBranch testCommit)
        {
            return repo.Branches.All(x => x.FriendlyName != testCommit.Branch);
        }

        private static bool Not_Master_Branch(TestCommitWithBranch testCommit)
        {
            return testCommit.Branch != MasterBranch;
        }

        private static bool Valid_Branch_Name(TestCommitWithBranch testCommit)
        {
            return !string.IsNullOrEmpty(testCommit.Branch);
        }

        private static void Commit_File(Repository repo, TestCommit commit)
        {
            Commands.Stage(repo, "*");

            var author = new Signature(commit.Name, commit.Email, commit.TimeStamp);
            repo.Commit(commit.CommitMessage, author, author);
        }

        private static void Populate_File_For_Commit(TestCommit commit, Repository repo)
        {
            var content = string.Join("\n", commit.Lines);
            var filePath = Path.Combine(repo.Info.WorkingDirectory, commit.FileName);
            File.WriteAllText(filePath, content);
        }
    }

    public class TestCommitWithBranch 
    {
        public TestCommit Commit { get; set; }
        public string Branch { get; set; } 
    }
}