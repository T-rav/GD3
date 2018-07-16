using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzer.Data;
using Analyzer.Domain;
using FluentAssertions;
using LibGit2Sharp;
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
            public void WhenBranchHEAD_ShouldReturnAllActiveDeveloper()
            {
                // arrange
                var repoPath = TestRepoPath("test-repo");
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

            [Test]
            public void WhenDeveloperBranch_ShouldReturnAllActiveDeveloper()
            {
                // arrange
                var repoPath = TestRepoPath("test-repo");
                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithBranch("origin/TusaniG")
                    .WithRange(DateTime.Parse("2018-07-11"), DateTime.Parse("2018-07-12"))
                    .Build();
                // act
                var actual = sut.List_Authors();
                // assert
                var expected = 2;
                actual.Count().Should().Be(expected);
            }
        }

        [TestFixture]
        public class ListPeriodActivity
        { 
            [TestCase("2018-06-25", "2018-07-09", 8)]
            [TestCase("2018-07-10", "2018-07-12", 1)]
            public void WhenEmailForActiveDeveloper_ShouldReturnActiveDays(DateTime start, DateTime end, int expected)
            {
                // arrange
                var repoPath = TestRepoPath("test-repo");
                var author = new Author { Name = "Monique", Email = "MoniqueG@SAHOMELOANS.COM" };

                var sut = new SourceControlRepositoryBuilder()
                    .WithRange(start, end)
                    .WithPath(repoPath)
                    .Build();
                // act
                var actual = sut.Period_Active_Days(author);
                // assert
                actual.Should().Be(expected);
            }

            [TestCase("2018-06-25", "2018-07-09", 6)]
            [TestCase("2018-07-10", "2018-07-12", 1)]
            public void NotHeadBranch_ShouldReturnActiveDays(DateTime start, DateTime end, int expected)
            {
                // arrange
                var repoPath = TestRepoPath("test-repo");
                var author = new Author { Name = "Thabani", Email = "thabanitembe@hotmail.com" };

                var sut = new SourceControlRepositoryBuilder()
                    .WithRange(start, end)
                    .WithPath(repoPath)
                    .WithBranch("origin/thabani")
                    .Build();
                // act
                var actual = sut.Period_Active_Days(author);
                // assert
                actual.Should().Be(expected);
            }

            [Test]
            public void WhenEmailNotForActiveDeveloper_ShouldReturnZero()
            {
                // arrange
                var repoPath = TestRepoPath("test-repo");
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
                var repoPath = TestRepoPath("test-repo");

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
                var repoPath = TestRepoPath("test-repo");

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
                var repoPath = TestRepoPath("test-repo");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-09"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Commits_Per_Day(author);
                // assert
                var expectedCommitsPerDay = 6.25;
                actual.Should().Be(expectedCommitsPerDay);
            }

            [Test]
            public void WhenDeveloperInactive_ShouldReturnZeroCommitsPerDay()
            {
                // arrange
                var author = new Author { Name = "boo", Email = "noone@moonbase.co" };
                var repoPath = TestRepoPath("test-repo");

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
                var repoPath = TestRepoPath("test-repo");

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
                        CommitsPerDay = 4.12,
                        Impact = 1.86,
                        LinesOfChangePerHour = 16.43,
                        LinesAdded = 3514,
                        LinesRemoved = 693,
                        Rtt100 = 6.09,
                        Ptt100 = 9.07,
                        Churn = 0.2
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenDeveloperActiveAcrossEntireRange_ShouldReturnStats()
            {
                // arrange
                var author = new Author { Name = "Siphenathi", Email = "SiphenathiP@SAHOMELOANS.COM" };
                var repoPath = TestRepoPath("test-repo");

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
                        Impact = 1.68,
                        LinesOfChangePerHour = 24.32,
                        LinesAdded = 5189,
                        LinesRemoved = 1816,
                        Churn = 0.35,
                        Rtt100 = 4.11,
                        Ptt100 = 8.54
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenDeveloperMadeFirstCommit_ShouldReturnStats()
            {
                // arrange
                var author = new Author { Name = "T-rav", Email = "tmfrisinger@gmail.com" };
                var repoPath = TestRepoPath("gd3-testoperations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-07-16"), DateTime.Parse("2018-07-16"))
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
                        ActiveDaysPerWeek = 1.0,
                        PeriodActiveDays = 1,
                        CommitsPerDay = 3.0,
                        Impact = 0.0,
                        LinesOfChangePerHour = 0.06,
                        LinesAdded = 2,
                        LinesRemoved = 0,
                        Churn = 0.0,
                        Rtt100 = 1666.67,
                        Ptt100 = 1666.67
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenBranch_ShouldReturnAllActiveDeveloperForBranch()
            {
                // arrange
                var author = new Author { Name = "Tusani", Email = "tusanig@sahomeloans.com" };
                var repoPath = TestRepoPath("test-repo");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithBranch("origin/TusaniG")
                    .WithRange(DateTime.Parse("2018-07-11"), DateTime.Parse("2018-07-12"))
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author>{author});
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 1.0,
                        PeriodActiveDays = 1,
                        CommitsPerDay = 1.0,
                        Impact = 0.01,
                        LinesOfChangePerHour = 0.25,
                        LinesAdded = 9,
                        LinesRemoved = 1,
                        Churn = 0.11,
                        Rtt100 = 400,
                        Ptt100 = 500
                    }
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [TestFixture]
            public class With_Ignored_Directory
            {
                [Test]
                public void WhenFolderIgnored_ShouldIgnoreFilesInFolderWhenCalculatingDeveloperStats()
                {
                    // arrange
                    var author = new Author { Name = "T-rav", Email = "tmfrisinger@gmail.com" };
                    var repoPath = TestRepoPath("test-repo");

                    var sut = new SourceControlRepositoryBuilder()
                        .WithPath(repoPath)
                        .WithIgnoredDirectory("documents")
                        .WithRange(DateTime.Parse("2018-06-25"), DateTime.Parse("2018-07-12"))
                        .Build();
                    // act
                    var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                    // assert
                    var expected = new List<DeveloperStats>
                    {
                        new DeveloperStats
                        {
                            Author = author,
                            ActiveDaysPerWeek = 1.0,
                            PeriodActiveDays = 1,
                            CommitsPerDay = 1.0,
                            Impact = 0.01,
                            LinesOfChangePerHour = 0.25,
                            LinesAdded = 9,
                            LinesRemoved = 1,
                            Churn = 0.11,
                            Rtt100 = 400,
                            Ptt100 = 500
                        }
                    };
                    actual.Should().BeEquivalentTo(expected);
                }
            }
        }

        private static string TestRepoPath(string repo)
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            var repoPath = Path.Combine(basePath, "..", "..", "..", "..","..", repo);
            return repoPath;
        }
    }
}
