using System;
using System.IO;
using System.Linq;
using Analyzer.Data;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class SourceControlRepositoryTests
    {
        [TestFixture]
        public class ListDevelopers
        {
            [Test]
            public void WhenValidRepositoryPath_ShouldReturnDeveloperList()
            {
                // arrange
                var repoPath = TestRepoPath();
                var sut = new SourceControlRepositoryBuilder()
                             .WithPath(repoPath)
                             .Build();
                // act
                var actual = sut.ListAuthors();
                // assert
                var expected = 7;
                actual.Count().Should().Be(expected);
            }

            // todo : pull into ctor test or builder test
            [Test]
            public void WhenInvalidRepositoryPath_ShouldThrowException()
            {
                // arrange
                var repoPath = "x:\\invalid_repo";
                var builder = new SourceControlRepositoryBuilder()
                                  .WithPath(repoPath);
                // act
                var actual = Assert.Throws<Exception>(() => builder.Build());
                // assert
                actual.Message.Should().Be("Invalid path [x:\\invalid_repo]");
            }
        }

        [TestFixture]
        public class ListPeriodActivity
        {
            [Test]
            public void WhenEmailForActiveDeveloper_ShouldReturnActiveDays()
            {
                // arrange
                var repoPath = TestRepoPath();
                var author = new Author { Name = "Thabani", Email = "thabanitembe@hotmail.com" };
                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .Build();
                // act
                var actual = sut.PeriodActiveDays(author);
                // assert
                var expected = 6;
                actual.Should().Be(expected);
            }

            [Test]
            public void WhenEmailNotForActiveDeveloper_ShouldReturnZero()
            {
                // arrange
                var repoPath = TestRepoPath();
                var author = new Author { Name = "no-one", Email = "solo@nothere.io" };
                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .Build();
                // act
                var actual = sut.PeriodActiveDays(author);
                // assert
                var expected = 0;
                actual.Should().Be(expected);
            }
        }

        private static string TestRepoPath()
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            var repoPath = Path.Combine(basePath, "..", "..", "..", "test-repo");
            return repoPath;
        }
    }
}
