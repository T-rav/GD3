using Analyzer.Data.SourceRepository;
using FluentAssertions;
using LibGit2Sharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Analyzer.Data.Tests.SourceRepository
{
    [TestFixture]
    public class SourceControlRepositoryBuilderTests
    {
        [Test]
        public void WhenPathNotValidGitRepo_ShouldThrowException()
        {
            // arrange
            var repoPath = "x:\\invalid_repo";
            var builder = new SourceControlRepositoryBuilder()
                .WithPath(repoPath);
            // act
            var actual = Assert.Throws<Exception>(() => builder.Build());
            // assert
            var expected = "Invalid path [x:\\invalid_repo]";
            actual.Message.Should().Be(expected);
        }

        [Test]
        public void WhenInvalidBranch_ShouldReturnDeveloperList()
        {
            // arrange
            var repoPath = new RepositoryTestDataBuilder().Build();
            //var repoPath = TestRepoPath("git-test-operations");
            var sut = new SourceControlRepositoryBuilder()
                .WithPath(repoPath)
                .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                .WithBranch("--Never-Existed--");
            // act
            var actual = Assert.Throws<Exception>(() => sut.Build());
            // assert
            actual.Message.Should().Be("Invalid branch [--Never-Existed--]");
        }

        [Test]
        public void WhenNoRangeSpecified_ShouldUseRepositorysFirstAndLastCommitDates()
        {
            // arrange
            var repoPath = TestRepoPath("git-test-operations");
            //var repoPath = new RepositoryTestDataBuilder()
            //              .With_Commit(new TestCommit { FileName = "file1.txt", Lines = new List<string> { "1", "2" }, TimeStamp = "2018-07-16" })
            //              .With_Commit(new TestCommit { FileName = "file2.txt", Lines = new List<string> { "3", "4" }, TimeStamp = "2018-09-13" })
            //              .Build();
            var sut = new SourceControlRepositoryBuilder()
                .WithPath(repoPath)
                .WithEntireHistory()
                .Build();
            // act
            var actual = sut.ReportingRange;
            // assert
            actual.Start.Should().Be(DateTime.Parse("2018-07-16"));
            actual.End.Should().Be(DateTime.Parse("2018-09-13"));
        }

        [Test]
        public void WhenNullIgnorePatterns_ShouldNotThrowException()
        {
            // arrange
            var repoPath = TestRepoPath("git-test-operations");
            var sut = new SourceControlRepositoryBuilder()
                .WithPath(repoPath)
                .WithIgnorePatterns(null);
            // act
            // assert
            Assert.DoesNotThrow(() => sut.Build());
        }

        private static string TestRepoPath(string repo)
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            var rootPath = GetRootPath(basePath);
            var repoPath = Path.Combine(rootPath, repo);
            return repoPath;
        }

        private static string GetRootPath(string basePath)
        {
            var source = "source";
            var indexOf = basePath.IndexOf(source, StringComparison.Ordinal);
            var rootPath = basePath.Substring(0, indexOf);
            return rootPath;
        }
    }

    public class RepositoryTestDataBuilder
    {
        private readonly List<TestCommit> _commits;
        private readonly string _branch;


        public RepositoryTestDataBuilder()
        {
            //_branch = "master";
            _commits = new List<TestCommit>();
        }
        //private string _path;
        //private string _branch;

        //public RepositoryTestDataBuilder With_Path(string path)
        //{
        //    _path = path;
        //    return this;
        //}

        //public RepositoryTestDataBuilder With_Branch(string branch)
        //{
        //    _branch = branch;
        //    return this;
        //}


        public RepositoryTestDataBuilder With_Commit(TestCommit commit)
        {
            _commits.Add(commit);

            return this;
        }

        public string Build()
        {
            var path = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
            var rootedPath = Repository.Init(path, false);
            using (var repo = new Repository(rootedPath))
            {
                if (!string.IsNullOrEmpty(_branch))
                {
                    repo.CreateBranch(_branch);
                }

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
            return path;
        }
    }

    public class TestCommit
    {
        public string FileName { get; set; }
        public List<string> Lines { get; set; }
        public string TimeStamp { get; set; }
    }
}
