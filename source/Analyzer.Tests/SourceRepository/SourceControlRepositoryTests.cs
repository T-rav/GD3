using Analyzer.Data.SourceRepository;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Team;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analyzer.Data.Tests.SourceRepository
{
    [TestFixture]
    public class SourceControlRepositoryTests
    {
        [TestFixture]
        public class ListAuthors
        {
            [Test]
            public void WhenMaster_ShouldReturnAllActiveDevelopers()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var sut = new SourceControlRepositoryBuilder()
                             .WithPath(repoPath)
                             .WithRange(DateTime.Parse("2018-7-16"), DateTime.Parse("2018-07-17"))
                             .Build();
                // act
                var actual = sut.List_Authors();
                // assert
                var expected = new List<Author>
                {
                    new Author{Name = "T-rav", Emails = new List<string>{"tmfrisinger@gmail.com"}}
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenDeveloperBranch_ShouldReturnAllActiveDevelopers()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithBranch("origin/my-branch")
                    .WithRange(DateTime.Parse("2018-7-16"), DateTime.Parse("2018-07-17"))
                    .Build();
                // act
                var actual = sut.List_Authors();
                // assert
                var expected = new List<Author>
                {
                    new Author{Name = "T-rav", Emails = new List<string>{"tmfrisinger@gmail.com"}},
                    new Author{Name = "Travis", Emails = new List<string>{"travisf@sahomeloans.com"}}
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenUsingAlias_ShouldReturnSingleDeveloperWithTwoEmails()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var aliasMap = new List<Alias>
                {
                    new Alias {Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com", "travisf@sahomeloans.com" } }
                };

                var sut = new SourceControlRepositoryBuilder()
                             .WithPath(repoPath)
                             .WithEntireHistory()
                             .WithBranch("origin/my-branch")
                             .Build();
                // act
                var actual = sut.List_Authors(aliasMap);
                // assert
                var expected = 1;
                actual.Count().Should().Be(expected);
            }

            [Test]
            public void WhenNullAliases_ShouldReturnTwoDevelopers()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithEntireHistory()
                    .WithBranch("origin/my-branch")
                    .Build();
                // act
                var actual = sut.List_Authors(null);
                // assert
                var expected = 2;
                actual.Count().Should().Be(expected);
            }

            [Test]
            public void WhenEmpyAliases_ShouldReturnTwoDevelopers()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var aliasMap = new List<Alias>();

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithEntireHistory()
                    .WithBranch("origin/my-branch")
                    .Build();
                // act
                var actual = sut.List_Authors(aliasMap);
                // assert
                var expected = 2;
                actual.Count().Should().Be(expected);
            }
        }

        [TestFixture]
        public class ListPeriodActivity
        {
            [TestCase("2018-09-10", "2018-09-14", 3)]
            [TestCase("2018-09-12", "2018-09-12", 1)]
            public void WhenMaster_ShouldReturnActiveDays(DateTime start, DateTime end, int expected)
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                    .WithRange(start, end)
                    .WithPath(repoPath)
                    .Build();
                // act
                var actual = sut.Period_Active_Days(author);
                // assert
                actual.Should().Be(expected);
            }

            [TestCase("2018-09-11", "2018-09-11", 0)]
            [TestCase("2018-09-10", "2018-09-10", 1)]
            public void WhenNotMaster_ShouldReturnActiveDays(DateTime start, DateTime end, int expected)
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                    .WithRange(start, end)
                    .WithPath(repoPath)
                    .WithBranch("origin/my-branch")
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
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "no-one", Emails = new List<string> { "solo@nothere.io" } };

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
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-14"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Active_Days_Per_Week(author);
                // assert
                var expectedActiveDaysPerWeek = 3.0;
                actual.Should().Be(expectedActiveDaysPerWeek);
            }

            [Test]
            public void WhenDeveloperNotActiveDuringPeriod_ShouldReturnZero()
            {
                // arrange
                var author = new Author { Name = "Moo", Emails = new List<string> { "invalid@buddy.io" } };
                var repoPath = TestRepoPath("git-test-operations");

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
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-14"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Commits_Per_Day(author);
                // assert
                var expectedCommitsPerDay = 1.67;
                actual.Should().Be(expectedCommitsPerDay);
            }

            [Test]
            public void WhenDeveloperInactive_ShouldReturnZeroCommitsPerDay()
            {
                // arrange
                var author = new Author { Name = "boo", Emails = new List<string> { "noone@moonbase.co" } };
                var repoPath = TestRepoPath("git-test-operations");

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
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                                .WithPath(repoPath)
                                .WithRange(DateTime.Parse("2018-07-16"), DateTime.Parse("2018-09-12"))
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
                        ActiveDaysPerWeek = 0.5,
                        PeriodActiveDays = 4,
                        CommitsPerDay = 2.0,
                        Impact = 0.02,
                        LinesOfChangePerHour = 0.13,
                        LinesAdded = 13,
                        LinesRemoved = 4,
                        Rtt100 = 769.23,
                        Ptt100 = 1428.57,
                        Churn = 0.31
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            //[Ignore("Need to find if this tested else where")]
            public void WhenNegativePtt100_ShouldReturnAbsOfValue()
            {
                // arrange
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-09-13"), DateTime.Parse("2018-09-13"))
                    .WithBranch("origin/negative-commits")
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = 81.97;
                actual.FirstOrDefault().Ptt100.Should().Be(expected);
            }

            [Test]
            public void WhenDeveloperActiveAcrossEntireRange_ShouldReturnStats()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-07-16"), DateTime.Parse("2018-09-12"))
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
                        ActiveDaysPerWeek = 0.5,
                        PeriodActiveDays = 4,
                        CommitsPerDay = 2.0,
                        Impact = 0.02,
                        LinesOfChangePerHour = 0.13,
                        LinesAdded = 13,
                        LinesRemoved = 4,
                        Churn = 0.31,
                        Rtt100 = 769.23,
                        Ptt100 = 1428.57
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenDeveloperMadeFirstCommit_ShouldReturnStats()
            {
                // arrange
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };
                var repoPath = TestRepoPath("git-test-operations");

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
            public void WhenBranchSelected_ShouldReturnAllActiveDeveloperForBranch()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithBranch("origin/my-branch")
                    .WithRange(DateTime.Parse("2018-07-16"), DateTime.Parse("2018-09-10"))
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 0.25,
                        PeriodActiveDays = 2,
                        CommitsPerDay = 2.5,
                        Impact = 0.03,
                        LinesOfChangePerHour = 0.31,
                        LinesAdded = 20,
                        LinesRemoved = 5,
                        Churn = 0.25,
                        Rtt100 = 322.58,
                        Ptt100 = 526.32
                    }
                };
                actual.Should().BeEquivalentTo(expected);
            }


            [Test]
            public void WhenFolderIgnored_ShouldIgnoreFilesInFolderWhenCalculatingDeveloperStats()
            {
                // arrange
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithIgnorePatterns(new[] { "documents", ".orig", "BASE", "LOCAL", "REMOTE" })
                    .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-13"))
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 3.0,
                        PeriodActiveDays = 3,
                        CommitsPerDay = 1.67,
                        Impact = 0.02,
                        LinesOfChangePerHour = 0.12,
                        LinesAdded = 10,
                        LinesRemoved = 4,
                        Churn = 0.4,
                        Rtt100 = 833.33,
                        Ptt100 = 2000.0
                    }
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenEntireHistory_ShouldReturnDeveloperStatsForLifetimeOfBranch()
            {
                // arrange
                var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithEntireHistory()
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 0.56,
                        PeriodActiveDays = 5,
                        CommitsPerDay = 1.8,
                        Impact = 1.6,
                        LinesOfChangePerHour = 0.3,
                        LinesAdded = 57,
                        LinesRemoved = 4,
                        Churn = 0.07,
                        Rtt100 = 333.33,
                        Ptt100 = 384.62
                    }
                };
                actual.Should().BeEquivalentTo(expected);
            }

            // todo: with help and my churn calculations
            [Test]
            public void WhenUsingAliasMapping_ShouldReturnOneDeveloperStats()
            {
                // arrange
                var author = new Author
                {
                    Name = "T-rav",
                    Emails = new List<string> { "tmfrisinger@gmail.com", "travisf@sahomeloans.com" }
                };
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithBranch("origin/my-branch")
                    .WithEntireHistory()
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        ActiveDaysPerWeek = 0.25,
                        PeriodActiveDays = 2,
                        CommitsPerDay = 3.0,
                        Impact = 0.04,
                        LinesOfChangePerHour = 0.36,
                        LinesAdded = 24,
                        LinesRemoved = 5,
                        Churn = 0.21,
                        Rtt100 = 277.78,
                        Ptt100 = 416.67
                    }
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            [Ignore("wip : flipping hard problem to solve well. might need alias work first")]
            public void WhenCollaberating_ShouldSplitActivityBetweenCollaberators()
            {
                // arrange
                var author1 = new Author { Name = "Thabani", Emails = new List<string> { "thabanitembe@hotmail.com" } };
                var author2 = new Author { Name = "MCEBISI", Emails = new List<string> { "mcebisimkhohliwe@gmail.com" } };
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-08-01"), DateTime.Parse("2018-08-02"))
                    .WithIgnorePatterns(new[] { ".orig" })
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .WithCollaberation(DateTime.Parse("2018-08-01"), author1, author2)
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author2 });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author2,
                        ActiveDaysPerWeek = 2.0,
                        PeriodActiveDays = 1,
                        CommitsPerDay = 6.0,
                        Impact = 0.23,
                        LinesOfChangePerHour = 5.75,
                        LinesAdded = 174,
                        LinesRemoved = 10,
                        Churn = 0.06,
                        Rtt100 = 17.39,
                        Ptt100 = 19.53
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
        }

        [TestFixture]
        public class Build_Team_Stats
        {
            [Test]
            public void WhenRangeOneDay_ShouldReturnStats()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-07-16"), DateTime.Parse("2018-07-16"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Build_Team_Stats();
                // assert
                var stats = new List<TeamStats>
                {
                    new TeamStats
                    {
                        DateOf = DateTime.Parse("2018-07-16"),
                        ActiveDevelopers = 1,
                        TotalCommits = 3
                    }
                };
                var expected = new TeamStatsCollection(stats, new List<DayOfWeek>());

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenRangeOneWeek_ShouldReturnStats()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-07-16"), DateTime.Parse("2018-07-20"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .Build();
                // act
                var actual = sut.Build_Team_Stats();
                // assert
                var stats = new List<TeamStats>
                {
                    new TeamStats
                    {
                        DateOf = DateTime.Parse("2018-07-16"),
                        ActiveDevelopers = 1,
                        TotalCommits = 3
                    },
                    new TeamStats
                    {
                        DateOf = DateTime.Parse("2018-07-17"),
                        ActiveDevelopers = 1,
                        TotalCommits = 1
                    },
                    new TeamStats
                    {
                        DateOf = DateTime.Parse("2018-07-18"),
                        ActiveDevelopers = 0,
                        TotalCommits = 0
                    },
                    new TeamStats
                    {
                        DateOf = DateTime.Parse("2018-07-19"),
                        ActiveDevelopers = 0,
                        TotalCommits = 0
                    },
                    new TeamStats
                    {
                        DateOf = DateTime.Parse("2018-07-20"),
                        ActiveDevelopers = 0,
                        TotalCommits = 0
                    }
                };

                var expected = new TeamStatsCollection(stats, new List<DayOfWeek>());

                actual.Should().BeEquivalentTo(expected);
            }

        }

        // todo : for team stats as well
        [TestFixture]
        public class Ignore_Comments
        {
            [Test]
            public void WhenFalse_ShouldReturnStatsWithCommentedOutLines()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author
                {
                    Name = "T-rav",
                    Emails = new List<string> { "tmfrisinger@gmail.com", "travisf@sahomeloans.com" }
                };

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-09-25"), DateTime.Parse("2018-09-25"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .WithIgnoreComments(false)
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        PeriodActiveDays = 1,
                        Impact = 0.02,
                        Churn = 0.5,
                        LinesAdded = 12,
                        LinesRemoved = 6,
                        ActiveDaysPerWeek = 1,
                        CommitsPerDay = 2,
                        Rtt100 = 178.57,
                        Ptt100 = 526.32,
                        LinesOfChangePerHour = 0.56
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenTrue_ShouldReturnStatsIgnoringCommentedOutLines()
            {
                // arrange
                var repoPath = TestRepoPath("git-test-operations");
                var author = new Author
                {
                    Name = "T-rav",
                    Emails = new List<string> { "tmfrisinger@gmail.com", "travisf@sahomeloans.com" }
                };

                var sut = new SourceControlRepositoryBuilder()
                    .WithPath(repoPath)
                    .WithRange(DateTime.Parse("2018-09-25"), DateTime.Parse("2018-09-25"))
                    .WithWorkingDaysPerWeek(4)
                    .WithWorkingWeekHours(32)
                    .WithIgnoreComments(true)
                    .Build();
                // act
                var actual = sut.Build_Individual_Developer_Stats(new List<Author> { author });
                // assert
                var expected = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        Author = author,
                        PeriodActiveDays = 1,
                        Impact = 0.02,
                        Churn = 1.0,
                        LinesAdded = 6,
                        LinesRemoved = 6,
                        ActiveDaysPerWeek = 1,
                        CommitsPerDay = 2,
                        Rtt100 = 263.16,
                        Ptt100 = Double.PositiveInfinity,
                        LinesOfChangePerHour = 0.38
                    }
                };

                actual.Should().BeEquivalentTo(expected);
            }
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
}
