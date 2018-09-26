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
        //private readonly string _branch;


        public RepositoryTestDataBuilder()
        {
            //_branch = "master";
            _commits = new List<TestCommit>();
        }


        public RepositoryTestDataBuilder With_Commit(TestCommit commit)
        {
            _commits.Add(commit);

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
                //    repo.CreateBranch(_branch);
                //}

                foreach (var commit in _commits)
                {
                    var content = string.Join("\n", commit.Lines);
                    var filePath = Path.Combine(repo.Info.WorkingDirectory, commit.FileName);
                    File.WriteAllText(filePath, content);

                    Commands.Stage(repo, "*");

                    var author = new Signature("James", "@jugglingnutcase", DateTime.ParseExact(commit.TimeStamp, "yyyy-MM-dd", CultureInfo.CurrentCulture));
                    repo.Commit("yay it works", author, author);
                }
            }
            return repositoryContext;
        }

    }
}