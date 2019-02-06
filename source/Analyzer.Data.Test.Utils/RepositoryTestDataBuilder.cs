using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Analyzer.Data.Test.Utils
{
    public class RepositoryTestDataBuilder
    {
        private readonly List<TestCommit> _commits;
        private readonly string _branch;


        public RepositoryTestDataBuilder()
        {
            _branch = "master";
            _commits = new List<TestCommit>();
        }


        public RepositoryTestDataBuilder With_Commit(TestCommit commit)
        {
            _commits.Add(commit);

            return this;
        }
        
        public RepositoryTestDataBuilder With_Default_Commit_To_Build_Master()
        {
            _commits.Add(new TestCommit
            {
                Name = "dummy-user",
                Email = "dummy@idiot.io",
                FileName = "test-file.txt",
                Lines = new List<string> { "a line" },
                CommitMessage = "a message of commit",
                TimeStamp = DateTime.Today

            });

            return this;
        }

        public FileSystemTestArtefact Build()
        {
            var path = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());


            var rootedPath = Repository.Init(path, false);
            var repositoryContext = new FileSystemTestArtefact { Path = path };

            using (var repo = new Repository(rootedPath))
            {
                //if (!string.IsNullOrEmpty(_branch))
                //{
                //    //repo.CreateBranch(_branch);
                //    repo.Branches.Add("develop", "HEAD");
                //}

                foreach (var commit in _commits)
                {
                    var content = string.Join("\n", commit.Lines);
                    var filePath = Path.Combine(repo.Info.WorkingDirectory, commit.FileName);
                    File.WriteAllText(filePath, content);

                    Commands.Stage(repo, "*");

                    var author = new Signature(commit.Name, commit.Email, commit.TimeStamp);
                    repo.Commit(commit.CommitMessage, author, author);

                    //File.Delete(filePath);
                }
            }
            return repositoryContext;
        }

    }
}