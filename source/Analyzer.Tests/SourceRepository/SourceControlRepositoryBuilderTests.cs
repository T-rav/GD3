using System;
using System.IO;
using Analyzer.Data.SourceRepository;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests.SourceRepository
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
            var repoPath = TestRepoPath();
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
            var repoPath = TestRepoPath();
            var sut = new SourceControlRepositoryBuilder()
                .WithPath(repoPath)
                .WithEntireHistory()
                .Build();
            // act
            var actual = sut.ReportingRange;
            // assert
            actual.Start.Should().Be(DateTime.Parse("2018-06-25"));
            actual.End.Should().Be(DateTime.Parse("2018-07-12"));
        }

        [Test]
        public void WhenNullIgnorePatterns_ShouldNotThrowException()
        {
            // arrange
            var repoPath = TestRepoPath();
            var sut = new SourceControlRepositoryBuilder()
                .WithPath(repoPath)
                .WithIgnorePatterns(null);
            // act
            // assert
            Assert.DoesNotThrow(() => sut.Build());
        }

        private static string TestRepoPath()
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            var repoPath = Path.Combine(basePath, "..", "..", "..", "..", "..", "test-repo");
            return repoPath;
        }
    }
}
