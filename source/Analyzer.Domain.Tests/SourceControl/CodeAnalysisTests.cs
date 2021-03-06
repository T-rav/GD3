﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzer.Domain.Developer;
using Analyzer.Domain.SourceControl;
using Analyzer.Domain.SourceControl.Stats;
using FluentAssertions;
using NUnit.Framework;
using StoneAge.System.Utils.Json;

namespace Analyzer.Domain.Tests.SourceControl
{
    [TestFixture]
    public class CodeAnalysisTests
    {
        [TestFixture]
        class Build_Stats
        {
            [TestFixture]
            class ReportingPeriod_Property
            {
                [Test]
                public void Given_Valid_Reporting_Context_Expect_Range_To_Transfer_To_Code_Stats()
                {
                    //---------------Arrange------------------
                    var authors = new List<Author>();
                    var commits = new List<Commit>();
                    var context = new AnalysisContext
                    {
                        ReportRange = new Domain.Reporting.ReportingPeriod
                        {
                            Start = DateTime.Today.AddDays(-7),
                            End = DateTime.Now
                        }
                    };
                    var sut = new CodeAnalysis(authors,commits, context);
                    //---------------Act----------------------
                    var actual = sut.Build_Stats();
                    //---------------Assert-------------------
                    actual.ReportingPeriod.Should().Be(context.ReportRange);
                }
            }

            [TestFixture]
            class CommitStats_Property
            {
                [Test]
                public void Given_Valid_Commits_Expect_Transformation_To_Commit_Stats()
                {
                    //---------------Arrange------------------
                    var authors = new List<Author>();
                    var commits = new List<Commit>
                    {
                        new Commit
                        {
                            Author = new Author{Name = "T-rav", Emails = new List<string> {"t@foo.com"}},
                            When = DateTime.Now,
                            Patch = new List<Patch>
                            {
                                new Patch{ChangeType = ChangeType.Modified, LinesAdded = 2, LinesRemoved = 0,Contents = "@@ 2,0 2,0 @@ namespace Test.Namespace \na\nb"}
                            }
                        }
                    };
                    var context = new AnalysisContext
                    {
                        ReportRange = new Domain.Reporting.ReportingPeriod
                        {
                            Start = DateTime.Today.AddDays(-7),
                            End = DateTime.Now
                        }
                    };
                    var sut = new CodeAnalysis(authors, commits, context);
                    //---------------Act----------------------
                    var actual = sut.Build_Stats();
                    //---------------Assert-------------------
                    var expected = new List<CommitStat>
                    {
                        new CommitStat(commits.First())
                    };
                    actual.CommitStats.Should().BeEquivalentTo(expected);
                }

                [Test]
                public void Given_Null_Commits_Expect_Empty_List()
                {
                    //---------------Arrange------------------
                    var authors = new List<Author>();
                    var context = new AnalysisContext
                    {
                        ReportRange = new Domain.Reporting.ReportingPeriod
                        {
                            Start = DateTime.Today.AddDays(-7),
                            End = DateTime.Now
                        }
                    };
                    var sut = new CodeAnalysis(authors, null, context);
                    //---------------Act----------------------
                    var actual = sut.Build_Stats();
                    //---------------Assert-------------------
                    actual.CommitStats.Should().BeEmpty();
                }
            }


            [TestFixture]
            class DeveloperStatsPerDay_Property
            {
                [Test]
                public void Given_Reporting_Range_Of_Two_Days_With_Single_Developer_Expect_Two_Days_Of_Stats()
                {
                    //---------------Arrange------------------
                    var author = new Author { Name = "T-rav", Emails = new List<string> { "t@foo.com" } };
                    var authors = new List<Author>
                    {
                        author
                    };

                    var commits = new List<Commit>
                    {
                        new Commit
                        {
                            Author = author,
                            When = DateTime.Now,
                            Patch = new List<Patch>
                            {
                                new Patch{ChangeType = ChangeType.Modified, LinesAdded = 2, LinesRemoved = 0,Contents = "@@ 2,0 2,0 @@ namespace Test.Namespace \na\nb"}
                            }
                        }
                    };
                    var context = new AnalysisContext
                    {
                        ReportRange = new Domain.Reporting.ReportingPeriod
                        {
                            Start = DateTime.Today.AddDays(-1),
                            End = DateTime.Now
                        }
                    };
                    var sut = new CodeAnalysis(authors, commits, context);
                    //---------------Act----------------------
                    var actual = sut.Build_Stats();
                    //---------------Assert-------------------
                    var expected = new List<DeveloperStatsForDay>
                    {
                        new DeveloperStatsForDay
                        {
                            Author = author,
                            Commits = 0,
                            Impact = 0,
                            Churn = 0,
                            RiskFactor = 0,
                            Ptt100 = 0,
                            When = DateTime.Now.Date.AddDays(-1)
                        },
                        new DeveloperStatsForDay
                        {
                            Author = author,
                            Commits = 1,
                            Impact = 0.003,
                            Churn = 0.0,
                            RiskFactor = 2.0,
                            Ptt100 = 400.00,
                            When = DateTime.Now.Date
                        }
                    };
                    actual.DeveloperStatsPerDay.Should().BeEquivalentTo(expected);
                }

                [Test]
                public void Given_Reporting_Range_Of_Two_Days_With_Multiple_Developers_Expect_Two_Days_Of_Stats()
                {
                    //---------------Arrange------------------
                    var author1 = new Author { Name = "T-rav", Emails = new List<string> { "t@foo.com" } };
                    var author2 = new Author { Name = "Zen", Emails = new List<string> { "z@foo.com" } };
                    var authors = new List<Author>
                    {
                        author1,
                        author2
                    };

                    var commits = new List<Commit>
                    {
                        new Commit
                        {
                            Author = author1,
                            When = DateTime.Now,
                            Patch = new List<Patch>
                            {
                                new Patch{ChangeType = ChangeType.Modified, LinesAdded = 2, LinesRemoved = 0,Contents = "@@ 2,0 2,0 @@ namespace Test.Namespace \na\nb"}
                            }
                        }
                    };
                    var context = new AnalysisContext
                    {
                        ReportRange = new Domain.Reporting.ReportingPeriod
                        {
                            Start = DateTime.Today.AddDays(-1),
                            End = DateTime.Now
                        }
                    };
                    var sut = new CodeAnalysis(authors, commits, context);
                    //---------------Act----------------------
                    var actual = sut.Build_Stats();
                    //---------------Assert-------------------
                    var expected = new List<DeveloperStatsForDay>
                    {
                        new DeveloperStatsForDay
                        {
                            Author = author1,
                            Commits = 0,
                            Impact = 0,
                            Churn = 0,
                            RiskFactor = 0,
                            Ptt100 = 0,
                            When = DateTime.Now.Date.AddDays(-1)
                        },
                        new DeveloperStatsForDay
                        {
                            Author = author1,
                            Commits = 1,
                            Impact = 0.003,
                            Churn = 0.0,
                            RiskFactor = 2.0,
                            Ptt100 = 400.00,
                            When = DateTime.Now.Date
                        },
                        new DeveloperStatsForDay
                        {
                            Author = author2,
                            Commits = 0,
                            Impact = 0,
                            Churn = 0,
                            RiskFactor = 0,
                            Ptt100 = 0,
                            When = DateTime.Now.Date.AddDays(-1)
                        },
                        new DeveloperStatsForDay
                        {
                            Author = author2,
                            Commits = 0,
                            Impact = 0,
                            Churn = 0,
                            RiskFactor = 0,
                            Ptt100 = 0,
                            When = DateTime.Now.Date
                        },
                    };
                    actual.DeveloperStatsPerDay.Should().BeEquivalentTo(expected);
                }
            }


            [TestFixture]
            class TeamStatsPerDay_Property
            {
                [Test]
                public void Given_Reporting_Range_Of_Two_Days_With_MultipleDeveloper_Expect_Two_Days_Of_Stats()
                {
                    //---------------Arrange------------------
                    var author1 = new Author { Name = "T-rav", Emails = new List<string> { "t@foo.com" } };
                    var author2 = new Author { Name = "Zen", Emails = new List<string> { "z@foo.com" } };

                    var authors = new List<Author>
                    {
                        author1,
                        author2
                    };

                    var commits = new List<Commit>
                    {
                        new Commit
                        {
                            Author = author1,
                            When = DateTime.Now,
                            Patch = new List<Patch>
                            {
                                new Patch{ChangeType = ChangeType.Modified, LinesAdded = 2, LinesRemoved = 0, Contents = "@@ 2,0 2,0 @@ namespace Test.Namespace \na\nb"}
                            }
                        },
                        new Commit
                        {
                            Author = author2,
                            When = DateTime.Now,
                            Patch = new List<Patch>
                            {
                                new Patch{ChangeType = ChangeType.Added, LinesAdded = 3, LinesRemoved = 0, Contents = "@@ 3,0 3,0 @@ namespace Test2.Namespace \n1\n2\n3"}
                            }
                        }
                    };
                    var context = new AnalysisContext
                    {
                        ReportRange = new Domain.Reporting.ReportingPeriod
                        {
                            Start = DateTime.Today.AddDays(-1),
                            End = DateTime.Now
                        }
                    };
                    var sut = new CodeAnalysis(authors, commits, context);
                    //---------------Act----------------------
                    var actual = sut.Build_Stats();
                    //---------------Assert-------------------
                    var expected = new List<TeamStatsForDay>
                    {
                        new TeamStatsForDay
                        {
                            ActiveDevelopers = 0,
                            Commits = 0,
                            Impact = 0,
                            Churn = 0,
                            RiskFactor = 0,
                            Ptt100 = 0,
                            When = DateTime.Now.Date.AddDays(-1)
                        },
                        new TeamStatsForDay
                        {
                            ActiveDevelopers = 2,
                            Commits = 2,
                            Impact = 0.006,
                            Churn = 0.0,
                            RiskFactor = 2.5,
                            Ptt100 = 331.58,
                            When = DateTime.Now.Date
                        }
                    };
                    actual.TeamStatsPerDay.Should().BeEquivalentTo(expected);
                }
            }
        }

        //[TestFixture]
        //class Stats
        //{
        //    [TestFixture]
        //    class ActiveDays
        //    {
        //        [TestCase("2018-09-10", "2018-09-14", 3)]
        //        [TestCase("2018-09-12", "2018-09-12", 1)]
        //        public void WhenMaster_ShouldReturnActiveDays(DateTime start, DateTime end, int days)
        //        {
        //            // arrange
        //            var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

        //            var commitBuilder = new CommitTestDataBuilder()
        //                .With_Author(author.Name, author.Emails.First());

        //            var commit1 = commitBuilder
        //                .With_File_Name("file1.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-10 01:01:01")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit2 = commitBuilder
        //                .With_File_Name("file2.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-12 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit3 = commitBuilder
        //                .With_File_Name("file3.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-14 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var context = new RepositoryTestDataBuilder()
        //                .After_Init_Commit_To_Master()
        //                .Make_Commit(commit1)
        //                .Make_Commit(commit2)
        //                .Make_Commit(commit3)
        //                .Build();

        //            var sourceAnalysis = new SourceControlAnalysisBuilder()
        //                .WithRange(start, end)
        //                .WithPath(context.Path)
        //                .Build();

        //            var sut = sourceAnalysis.Run_Analysis();
        //            // act
        //            var actual = sut.Individual_Period_Stats();
        //            // assert
        //            var expected = new List<CommitStat>
        //                {
        //                    new CommitStat {Author = author, ActiveDays = days}
        //                };
        //            actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Commits)
        //                .Excluding(x => x.Ptt100));
        //        }

        //        [TestCase("2018-09-10", "2018-09-14", 3)]
        //        [TestCase("2018-09-12", "2018-09-12", 1)]
        //        public void GivenSingleActiveDeveloperWithAliases_ExpectSinglePeriodStatus(DateTime start, DateTime end, int days)
        //        {
        //            // arrange
        //            var aliases = new List<Alias>
        //                {
        //                    new Alias
        //                    {
        //                        Name = "T-rav",
        //                        Emails = new List<string> {"tmfrisinger@gmail.com", "travis@frisinger.com"}
        //                    }
        //                };
        //            var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

        //            var aliasFile = Create_Alias_File(aliases);

        //            var commitBuilder = new CommitTestDataBuilder()
        //                .With_Author(author.Name, author.Emails.First());

        //            var commit1 = commitBuilder
        //                .With_File_Name("file1.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-10 01:01:01")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit2 = commitBuilder
        //                .With_Author(author.Name, author.Emails.Last())
        //                .With_File_Name("file2.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-12 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit3 = commitBuilder
        //                .With_Author(author.Name, author.Emails.First())
        //                .With_File_Name("file3.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-14 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var context = new RepositoryTestDataBuilder()
        //                .After_Init_Commit_To_Master()
        //                .Make_Commit(commit1)
        //                .Make_Commit(commit2)
        //                .Make_Commit(commit3)
        //                .Build();

        //            var sourceAnalysis = new SourceControlAnalysisBuilder()
        //                .WithRange(start, end)
        //                .WithPath(context.Path)
        //                .WithAliasMapping(aliasFile)
        //                .Build();

        //            var sut = sourceAnalysis.Run_Analysis();
        //            // act
        //            var actual = sut.Individual_Period_Stats();
        //            // assert
        //            var expectedAuthor = new Author
        //            {
        //                Name = author.Name,
        //                Emails = new List<string> { "tmfrisinger@gmail.com", "travis@frisinger.com" }
        //            };
        //            var expected = new List<CommitStat>
        //                {
        //                    new CommitStat {Author = expectedAuthor, ActiveDays = days}
        //                };
        //            actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Commits)
        //                .Excluding(x => x.Ptt100));
        //        }

        //        [TestCase("2018-09-10", "2018-09-14", 3)]
        //        [TestCase("2018-09-10", "2018-09-10", 1)]
        //        public void GivenBranchIsAnalyzed_ExpectStatsForActiveDevelopers(DateTime start, DateTime end,
        //            int days)
        //        {
        //            // arrange

        //            var branchName = "my-branch";
        //            var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

        //            var commitBuilder = new CommitTestDataBuilder()
        //                .With_Author(author.Name, author.Emails.First());

        //            var commit1 = commitBuilder
        //                .With_File_Name("file1.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-10 01:01:01")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit2 = commitBuilder
        //                .With_File_Name("file2.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-12 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit3 = commitBuilder
        //                .With_File_Name("file3.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-14 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var context = new RepositoryTestDataBuilder()
        //                .After_Init_Commit_To_Master()
        //                .On_Branch(branchName)
        //                .Make_Commit(commit1)
        //                .Make_Commit(commit2)
        //                .Make_Commit(commit3)
        //                .Build();

        //            var sourceAnalysis = new SourceControlAnalysisBuilder()
        //                .WithRange(start, end)
        //                .WithPath(context.Path)
        //                .WithBranch(branchName)
        //                .Build();

        //            var sut = sourceAnalysis.Run_Analysis();
        //            // act
        //            var actual = sut.Individual_Period_Stats();
        //            // assert
        //            var expected = new List<CommitStat>
        //                {
        //                    new CommitStat {Author = author, ActiveDays = days}
        //                };
        //            actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.Commits)
        //                .Excluding(x => x.Ptt100));
        //        }

        //        [Test]
        //        public void GivenNoDevelopersActive_ExpectEmptyList()
        //        {
        //            // arrange
        //            var author = new Author { Name = "no-one", Emails = new List<string> { "solo@nothere.io" } };

        //            var commitBuilder = new CommitTestDataBuilder()
        //                .With_Author(author.Name, author.Emails.First());

        //            var commit1 = commitBuilder
        //                .With_File_Name("file1.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-10 01:01:01")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit2 = commitBuilder
        //                .With_File_Name("file2.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-12 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var commit3 = commitBuilder
        //                .With_File_Name("file3.txt")
        //                .With_File_Content("1", "2")
        //                .With_Commit_Timestamp("2018-09-14 11:03:02")
        //                .With_Commit_Message("it worked!")
        //                .Build();

        //            var context = new RepositoryTestDataBuilder()
        //                .After_Init_Commit_To_Master()
        //                .Make_Commit(commit1)
        //                .Make_Commit(commit2)
        //                .Make_Commit(commit3)
        //                .Build();

        //            var sourceAnalysis = new SourceControlAnalysisBuilder()
        //                .WithPath(context.Path)
        //                .Build();

        //            var sut = sourceAnalysis.Run_Analysis();
        //            // act
        //            var actual = sut.Individual_Period_Stats();
        //            // assert
        //            actual.Should().BeEmpty();

        //        }
        //    }

            //[TestFixture]
            //class AverageCommitsPerDay
            //{
            //    [Test]
            //    public void GivenSingleDeveloperActive_ExpectCommitsPerDayToBePresent()
            //    {
            //        // arrange
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-10 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file4.txt")
            //            .With_File_Content("3", "5")
            //            .With_Commit_Timestamp("2018-09-11 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit3 = commitBuilder
            //            .With_File_Name("file2.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-12 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit4 = commitBuilder
            //            .With_File_Name("file3.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-14 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit5 = commitBuilder
            //            .With_File_Name("file5.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-14 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .After_Init_Commit_To_Master()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Make_Commit(commit3)
            //            .Make_Commit(commit4)
            //            .Make_Commit(commit5)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-14"))
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats {Author = author, AverageCommitsPerDay = 1.25}
            //            };
            //        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ActiveDays)
            //            .Excluding(x => x.Ptt100));
            //    }
            //}

            //[TestFixture]
            //class Ptt100
            //{
            //    [Test]
            //    public void WhenNegativePtt100_ShouldReturnAbsOfValue()
            //    {
            //        // arrange
            //        var branchName = "negative-commits";
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("3", "4", "5", "6")
            //            .With_Commit_Timestamp("2018-09-12 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .With_Branch(branchName)
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2", "3")
            //            .With_Commit_Timestamp("2018-09-13 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .With_Branch(branchName)
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .After_Init_Commit_To_Master()
            //            .On_Branch(branchName)
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithRange(DateTime.Parse("2018-09-13"), DateTime.Parse("2018-09-13"))
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .WithBranch(branchName)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats {Author = author, Ptt100 = 833.33}
            //            };
            //        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ActiveDays)
            //            .Excluding(x => x.AverageCommitsPerDay));
            //    }

            //    [Test]
            //    public void WhenDeveloperActiveAcrossEntireRange_ShouldReturnStats()
            //    {
            //        // arrange
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-10 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("3", "4")
            //            .With_Commit_Timestamp("2018-09-12 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit3 = commitBuilder
            //            .With_File_Name("file3.txt")
            //            .With_File_Content("1", "2", "5", "7")
            //            .With_Commit_Timestamp("2018-09-20 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .After_Init_Commit_To_Master()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Make_Commit(commit3)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-20"))
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats {Author = author, Ptt100 = 44.44}
            //            };
            //        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ActiveDays)
            //            .Excluding(x => x.AverageCommitsPerDay));
            //    }

            //    [Test]
            //    public void WhenDeveloperStatsIncludeFirstCommit_ShouldReturnStatsWithoutException()
            //    {
            //        // arrange
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp(DateTime.Today)
            //            .With_Commit_Message("init commit")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file2.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp(DateTime.Today)
            //            .With_Commit_Message("second commit")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithRange(DateTime.Today, DateTime.Today)
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        // assert
            //        Assert.DoesNotThrow(() => sut.Individual_Period_Stats());
            //    }
            //}

            //[TestFixture]
            //class All_Stats
            //{
            //    // todo : should it count as a period active day if all work is in the ignore list?
            //    [Test]
            //    public void WhenPatternsIgnored_ShouldIgnoreMatchingFilesWhenCalculatingDeveloperStats()
            //    {
            //        // arrange
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt.orig")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-10 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("3", "4")
            //            .With_Commit_Timestamp("2018-09-10 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit3 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("5", "6", "7")
            //            .With_Commit_Timestamp("2018-09-10 13:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .After_Init_Commit_To_Master()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Make_Commit(commit3)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithIgnorePatterns(new[] { ".orig" })
            //            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-20"))
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats
            //                    {Author = author, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 53.19}
            //            };
            //        actual.Should().BeEquivalentTo(expected);
            //    }

            //    [Test]
            //    public void WhenEntireHistory_ShouldReturnDeveloperStatsForLifetime()
            //    {
            //        // arrange;
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmil.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-08-10 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("3", "4")
            //            .With_Commit_Timestamp("2018-09-12 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit3 = commitBuilder
            //            .With_File_Name("file3.txt")
            //            .With_File_Content("1", "2", "5", "7")
            //            .With_Commit_Timestamp("2018-10-20 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Make_Commit(commit3)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithEntireHistory()
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats
            //                    {Author = author, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 44.44}
            //            };
            //        actual.Should().BeEquivalentTo(expected);
            //    }

            //    [Test]
            //    public void WhenUsingAliasMapping_ShouldReturnOneDeveloperStats()
            //    {
            //        // arrange
            //        var aliases = new List<Alias>
            //            {
            //                new Alias
            //                {
            //                    Name = "T-rav",
            //                    Emails = new List<string> {"tmfrisinger@gmail.com", "travis@frisinger.com"}
            //                }
            //            };
            //        var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };
            //        var aliasFile = Create_Alias_File(aliases);

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author.Name, author.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-10 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("3", "4")
            //            .With_Commit_Timestamp("2018-09-12 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit3 = commitBuilder
            //            .With_Author("Travis", "travis@frisinger.com")
            //            .With_File_Name("file3.txt")
            //            .With_File_Content("1", "2", "5", "7")
            //            .With_Commit_Timestamp("2018-09-20 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Make_Commit(commit3)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithAliasMapping(aliasFile)
            //            .WithEntireHistory()
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expectedAuthor = new Author
            //        {
            //            Name = author.Name,
            //            Emails = new List<string> { "tmfrisinger@gmail.com", "travis@frisinger.com" }
            //        };
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats
            //                    {Author = expectedAuthor, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 44.44}
            //            };
            //        actual.Should().BeEquivalentTo(expected);
            //    }

            //    [Test]
            //    public void WhenNotUsingAliases_ShouldReturnTwoDeveloperStats()
            //    {
            //        // arrange
            //        var author1 = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com", } };
            //        var author2 = new Author { Name = "Travis", Emails = new List<string> { "travis@frisinger.com" } };

            //        var commitBuilder = new CommitTestDataBuilder()
            //            .With_Author(author1.Name, author1.Emails.First());

            //        var commit1 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("1", "2")
            //            .With_Commit_Timestamp("2018-09-10 01:01:01")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit2 = commitBuilder
            //            .With_File_Name("file1.txt")
            //            .With_File_Content("3", "4", "99", "1")
            //            .With_Commit_Timestamp("2018-09-12 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var commit3 = commitBuilder
            //            .With_Author("Travis", "travis@frisinger.com")
            //            .With_File_Name("file3.txt")
            //            .With_File_Content("1", "2", "5", "7")
            //            .With_Commit_Timestamp("2018-09-20 11:03:02")
            //            .With_Commit_Message("it worked!")
            //            .Build();

            //        var context = new RepositoryTestDataBuilder()
            //            .Make_Commit(commit1)
            //            .Make_Commit(commit2)
            //            .Make_Commit(commit3)
            //            .Build();

            //        var sourceAnalysis = new SourceControlAnalysisBuilder()
            //            .WithPath(context.Path)
            //            .WithEntireHistory()
            //            .WithWorkingDaysPerWeek(4)
            //            .WithWorkingWeekHours(32)
            //            .Build();

            //        var sut = sourceAnalysis.Run_Analysis();
            //        // act
            //        var actual = sut.Individual_Period_Stats();
            //        // assert
            //        var expected = new List<IndividualPeriodStats>
            //            {
            //                new IndividualPeriodStats
            //                    {Author = author1, ActiveDays = 2, AverageCommitsPerDay = 1.0, Ptt100 = 100.00},
            //                new IndividualPeriodStats
            //                    {Author = author2, ActiveDays = 1, AverageCommitsPerDay = 1.0, Ptt100 = 200.00}
            //            };
            //        actual.Should().BeEquivalentTo(expected);
            //    }
            //}

        //}

        private static string Create_Alias_File(List<Alias> aliases)
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, aliases.Serialize());
            return filePath;
        }
    }
}
