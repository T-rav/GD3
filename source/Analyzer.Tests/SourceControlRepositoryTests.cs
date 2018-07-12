using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzer.Data;
using Analyzer.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class SourceControlRepositoryTests
    {
        [TestFixture]
        public class ListAuthors
        {
            [Test]
            public void WhenValidRepositoryPath_ShouldReturnDeveloperList()
            {
                // arrange
                var repoPath = TestRepoPath();
                var sut = new SourceControlRepositoryBuilder()
                             .WithPath(repoPath)
                             .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                             .Build();
                // act
                var actual = sut.List_Authors();
                // assert
                var expected = 7;
                actual.Count().Should().Be(expected);
            }

            // todo : pull into ctor test or builder test and add test for WithRange
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
                var expected = "Invalid path [x:\\invalid_repo]";
                actual.Message.Should().Be(expected);
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
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                    .WithPath(repoPath)
                    .Build();
                // act
                var actual = sut.Period_Active_Days(author);
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
                var actual = sut.Period_Active_Days(author);
                // assert
                var expected = 0;
                actual.Should().Be(expected);
            }
        }

        [TestFixture]
        public class TotalWorkingDays
        {
            [Test]
            public void WhenDeveloperActiveDuringPeriod_ShouldReturnTotalWorkingDays()
            {
                // arrange
                var author = new Author { Name = "Siphenathi", Email = "SiphenathiP@SAHOMELOANS.COM" };
                var repoPath = TestRepoPath();

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Active_Days_Per_Week(author);
                // assert
                var expectedActiveDaysPerWeek = 4.0;
                actual.Should().Be(expectedActiveDaysPerWeek);
            }

            [Test]
            public void WhenDeveloperNotActiveDuringPeriod_ShouldReturnZero()
            {
                // arrange
                var author = new Author { Name = "Moo", Email = "invalid@buddy.io" };
                var repoPath = TestRepoPath();

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                    .Build();
                // act
                var actual = sut.Active_Days_Per_Week(author);
                // assert
                var expectedActiveDaysPerWeek = 0.0;
                actual.Should().Be(expectedActiveDaysPerWeek);
            }
        }

        [TestFixture]
        public class CommitsPerDay
        {
            [Test]
            public void WhenDeveloperActive_ShouldReturnCommitsPerDay()
            {
                // arrange
                var author = new Author {Name = "Siphenathi", Email = "SiphenathiP@SAHOMELOANS.COM"};
                var repoPath = TestRepoPath();

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Commits_Per_Day(author);
                // assert
                var expectedCommitsPerDay = 6.75;
                actual.Should().Be(expectedCommitsPerDay);
            }

            [Test]
            public void WhenDeveloperInactive_ShouldReturnZeroCommitsPerDay()
            {
                // arrange
                var author = new Author { Name = "boo", Email = "noone@moonbase.co" };
                var repoPath = TestRepoPath();

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                    .Build();
                // act
                var actual = sut.Commits_Per_Day(author);
                // assert
                var expectedCommitsPerDay = 0.0;
                actual.Should().Be(expectedCommitsPerDay);
            }
        }

        [TestFixture]
        public class Build_Individual_Developer_Stats
        {
            [Test]
            public void WhenRangeEntireProjectHistory_ShouldReturnStats()
            {
                // arrange
                var author = new Author { Name = "Sinothilem", Email = "sinothilem@D987321" };
                var repoPath = TestRepoPath();

                var sut = new SourceControlRepositoryBuilder()
                                .WithPath(repoPath)
                                .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-10"))
                                .WithWorkingDaysPerWeek(4)
                                .WithWorkingWeekHours(32)
                                .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author>{author});
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 4.0,
                        PeriodActiveDays = 8,
                        CommitsPerDay = 4.0,
                        Impact = 1.81,
                        LinesOfChangePerHour = 15.75,
                        Rank = 0
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenDeveloperActiveAcrossEntireRange_ShouldReturnStats()
            {
                // arrange
                var author = new Author { Name = "Siphenathi", Email = "SiphenathiP@SAHOMELOANS.COM" };
                var repoPath = TestRepoPath();

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-10"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 4.5,
                        PeriodActiveDays = 9,
                        CommitsPerDay = 6.0,
                        Impact = 1.49,
                        LinesOfChangePerHour = 23.68,
                        Rank = 0
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }

        private static string TestRepoPath()
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            var repoPath = Path.Combine(basePath, "..", "..", "..", "..","..", "test-repo");
            return repoPath;
        }
    }
}
