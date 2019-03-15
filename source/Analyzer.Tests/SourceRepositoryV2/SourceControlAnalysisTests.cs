using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzer.Data.SourceControlV2;
using Analyzer.Data.Test.Utils;
using Analyzer.Domain.Developer;
using Analyzer.Domain.SourceControlV2;
using FluentAssertions;
using NUnit.Framework;
using TddBuddy.System.Utils.JsonUtils;

namespace Analyzer.Data.Tests.SourceRepositoryV2
{
    [TestFixture]
    public class SourceControlAnalysisTests
    {
        [TestFixture]
        class Run_Analysis
        {
            [TestFixture]
            class Authors
            {
                [Test]
                public void WhenMaster_ShouldReturnAllActiveDevelopers()
                {
                    // arrange
                    var authorName = "T-rav";

                    var commitBuilder = new CommitTestDataBuilder()
                        .With_Author(authorName, "tmfrisinger@gmail.com");

                    var commit1 = commitBuilder
                        .With_File_Name("file1.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-16 01:01:01")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var context = new RepositoryTestDataBuilder()
                        .Make_Commit(commit1)
                        .Build();

                    var sut = new SourceControlAnalysisBuilder()
                        .WithPath(context.Path)
                        .WithRange(DateTime.Parse("2018-7-16"), DateTime.Parse("2018-07-17"))
                        .Build();
                    // act
                    var actual = sut.Run_Analysis();
                    // assert
                    var expected = new List<Author>
                    {
                        new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}}
                    };
                    actual.Authors.Should().BeEquivalentTo(expected);
                }

                [Test]
                public void WhenDeveloperBranch_ShouldReturnAllActiveDevelopers()
                {
                    // arrange
                    var branchName = "my-branch";
                    var authorName = "Travis";
                    var commitBuilder = new CommitTestDataBuilder()
                        .With_Author(authorName, "travis@frisinger.com");

                    var commit1 = commitBuilder
                        .With_Branch(branchName)
                        .With_File_Name("file1.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-16 01:01:01")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var commit2 = commitBuilder
                        .With_Branch(branchName)
                        .With_Author("T-rav", "tmfrisinger@gmail.com")
                        .With_File_Name("file2.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-17 11:03:02")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var context = new RepositoryTestDataBuilder()
                        .After_Init_Commit_To_Master()
                        .On_Branch(branchName)
                        .Make_Commit(commit1)
                        .Make_Commit(commit2)
                        .Build();

                    var sut = new SourceControlAnalysisBuilder()
                        .WithPath(context.Path)
                        .WithBranch(branchName)
                        .WithRange(DateTime.Parse("2018-7-16"), DateTime.Parse("2018-07-17"))
                        .Build();
                    // act
                    var actual = sut.Run_Analysis();
                    // assert
                    var expected = new List<Author>
                    {
                        new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}},
                        new Author {Name = "Travis", Emails = new List<string> {"travis@frisinger.com"}}
                    };
                    actual.Authors.Should().BeEquivalentTo(expected);
                }


                [Test]
                public void WhenUsingAlias_ShouldReturnSingleDeveloperWithTwoEmails()
                {
                    // arrange
                    var branchName = "my-branch";
                    var authorName = "T-rav";
                    var aliasMap = new List<Alias>
                    {
                        new Alias
                        {
                            Name = authorName,
                            Emails = new List<string> {"tmfrisinger@gmail.com", "travis@frisinger.com"}
                        }
                    };

                    var aliasFile = Create_Alias_File(aliasMap);

                    var commitBuilder = new CommitTestDataBuilder()
                        .With_Author(authorName, "tmfrisinger@gmail.com");

                    var commit1 = commitBuilder
                        .With_Branch(branchName)
                        .With_File_Name("file1.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-16 01:01:01")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var commit2 = commitBuilder
                        .With_Author(authorName, "travis@frisinger.com")
                        .With_Branch(branchName)
                        .With_File_Name("file2.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-17 11:03:02")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var context = new RepositoryTestDataBuilder()
                        .After_Init_Commit_To_Master()
                        .On_Branch(branchName)
                        .Make_Commit(commit1)
                        .Make_Commit(commit2)
                        .Build();

                    var sut = new SourceControlAnalysisBuilder()
                        .WithPath(context.Path)
                        .WithEntireHistory()
                        .WithBranch("my-branch")
                        .WithAliasMapping(aliasFile)
                        .Build();
                    // act
                    var actual = sut.Run_Analysis();
                    // assert
                    var expected = 1;
                    actual.Authors.Count.Should().Be(expected);
                }

                [Test]
                public void WhenNullAliases_ShouldReturnTwoDevelopers()
                {
                    // arrange
                    var branchName = "my-branch";
                    var commitBuilder = new CommitTestDataBuilder()
                        .With_Author("T-rav", "tmfrisinger@gmail.com");

                    var commit1 = commitBuilder
                        .With_Branch(branchName)
                        .With_File_Name("file1.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-16 01:01:01")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var commit2 = commitBuilder
                        .With_Branch(branchName)
                        .With_Author("Travis", "travis@frisinger.com")
                        .With_File_Name("file2.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-17 11:03:02")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var context = new RepositoryTestDataBuilder()
                        .After_Init_Commit_To_Master()
                        .On_Branch(branchName)
                        .Make_Commit(commit1)
                        .Make_Commit(commit2)
                        .Build();

                    var sut = new SourceControlAnalysisBuilder()
                        .WithPath(context.Path)
                        .WithEntireHistory()
                        .WithBranch(branchName)
                        .Build();
                    // act
                    var actual = sut.Run_Analysis();
                    // assert
                    var expected = 2;
                    actual.Authors.Count.Should().Be(expected);
                }

                [Test]
                public void WhenEmptyAliases_ShouldReturnTwoDevelopers()
                {
                    // arrange
                    var branchName = "my-branch";
                    var aliasMap = new List<Alias>();

                    var aliasFile = Create_Alias_File(aliasMap);

                    var commitBuilder = new CommitTestDataBuilder()
                        .With_Author("T-rav", "tmfrisinger@gmail.com");

                    var commit1 = commitBuilder
                        .With_Branch(branchName)
                        .With_File_Name("file1.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-16 01:01:01")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var commit2 = commitBuilder
                        .With_Branch(branchName)
                        .With_Author("Travis", "travis@frisinger.com")
                        .With_File_Name("file2.txt")
                        .With_File_Content("1", "2")
                        .With_Commit_Timestamp("2018-07-17 11:03:02")
                        .With_Commit_Message("it worked!")
                        .Build();

                    var context = new RepositoryTestDataBuilder()
                        .After_Init_Commit_To_Master()
                        .On_Branch(branchName)
                        .Make_Commit(commit1)
                        .Make_Commit(commit2)
                        .Build();

                    var sut = new SourceControlAnalysisBuilder()
                        .WithPath(context.Path)
                        .WithEntireHistory()
                        .WithBranch(branchName)
                        .WithAliasMapping(aliasFile)
                        .Build();
                    // act
                    var actual = sut.Run_Analysis();
                    // assert
                    var expected = 2;
                    actual.Authors.Count.Should().Be(expected);
                }
            }

            [TestFixture]
            class Individual_Period_Stats
            {
                [TestFixture]
                class ActiveDays
                {
                    [TestCase("2018-09-10", "2018-09-14", 3)]
                    [TestCase("2018-09-12", "2018-09-12", 1)]
                    public void WhenMaster_ShouldReturnActiveDays(DateTime start, DateTime end, int days)
                    {
                        // arrange
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file2.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-14 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithRange(start, end)
                            .WithPath(context.Path)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats {Author = author, ActiveDays = days}
                        };
                        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.AverageCommitsPerDay)
                            .Excluding(x => x.Ptt100));
                    }

                    [TestCase("2018-09-10", "2018-09-14", 3)]
                    [TestCase("2018-09-12", "2018-09-12", 1)]
                    public void GivenSingleActiveDeveloperWithAliases_ExpectSinglePeriodStatus(DateTime start, DateTime end, int days)
                    {
                        // arrange
                        var aliases = new List<Alias>
                        {
                            new Alias
                            {
                                Name = "T-rav",
                                Emails = new List<string> {"tmfrisinger@gmail.com", "travis@frisinger.com"}
                            }
                        };
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var aliasFile = Create_Alias_File(aliases);

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_Author(author.Name, author.Emails.Last())
                            .With_File_Name("file2.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_Author(author.Name, author.Emails.First())
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-14 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithRange(start, end)
                            .WithPath(context.Path)
                            .WithAliasMapping(aliasFile)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expectedAuthor = new Author
                        {
                            Name = author.Name,
                            Emails = new List<string> { "tmfrisinger@gmail.com", "travis@frisinger.com" }
                        };
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats {Author = expectedAuthor, ActiveDays = days}
                        };
                        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.AverageCommitsPerDay)
                            .Excluding(x => x.Ptt100));
                    }

                    [TestCase("2018-09-10", "2018-09-14", 3)]
                    [TestCase("2018-09-10", "2018-09-10", 1)]
                    public void GivenBranchIsAnalyzed_ExpectStatsForActiveDevelopers(DateTime start, DateTime end,
                        int days)
                    {
                        // arrange

                        var branchName = "my-branch";
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file2.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-14 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .On_Branch(branchName)
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithRange(start, end)
                            .WithPath(context.Path)
                            .WithBranch(branchName)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats {Author = author, ActiveDays = days}
                        };
                        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.AverageCommitsPerDay)
                            .Excluding(x => x.Ptt100));
                    }

                    [Test]
                    public void GivenNoDevelopersActive_ExpectEmptyList()
                    {
                        // arrange
                        var author = new Author {Name = "no-one", Emails = new List<string> {"solo@nothere.io"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file2.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-14 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        actual.Should().BeEmpty();

                    }
                }

                [TestFixture]
                class AverageCommitsPerDay
                {
                    [Test]
                    public void GivenSingleDeveloperActive_ExpectCommitsPerDayToBePresent()
                    {
                        // arrange
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file4.txt")
                            .With_File_Content("3", "5")
                            .With_Commit_Timestamp("2018-09-11 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file2.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit4 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-14 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit5 = commitBuilder
                            .With_File_Name("file5.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-14 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Make_Commit(commit4)
                            .Make_Commit(commit5)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-14"))
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats {Author = author, AverageCommitsPerDay = 1.25}
                        };
                        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ActiveDays)
                            .Excluding(x => x.Ptt100));
                    }
                }

                [TestFixture]
                class Ptt100
                {
                    [Test]
                    public void WhenNegativePtt100_ShouldReturnAbsOfValue()
                    {
                        // arrange
                        var branchName = "negative-commits";
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("3", "4", "5", "6")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .With_Branch(branchName)
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2", "3")
                            .With_Commit_Timestamp("2018-09-13 01:01:01")
                            .With_Commit_Message("it worked!")
                            .With_Branch(branchName)
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .On_Branch(branchName)
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithRange(DateTime.Parse("2018-09-13"), DateTime.Parse("2018-09-13"))
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .WithBranch(branchName)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats {Author = author, Ptt100 = 833.33}
                        };
                        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ActiveDays)
                            .Excluding(x => x.AverageCommitsPerDay));
                    }

                    [Test]
                    public void WhenDeveloperActiveAcrossEntireRange_ShouldReturnStats()
                    {
                        // arrange
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("3", "4")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2", "5", "7")
                            .With_Commit_Timestamp("2018-09-20 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-20"))
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats {Author = author, Ptt100 = 44.44}
                        };
                        actual.Should().BeEquivalentTo(expected, opt => opt.Excluding(x => x.ActiveDays)
                            .Excluding(x => x.AverageCommitsPerDay));
                    }

                    [Test]
                    public void WhenDeveloperStatsIncludeFirstCommit_ShouldReturnStatsWithoutException()
                    {
                        // arrange
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp(DateTime.Today)
                            .With_Commit_Message("init commit")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file2.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp(DateTime.Today)
                            .With_Commit_Message("second commit")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithRange(DateTime.Today, DateTime.Today)
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        // assert
                        Assert.DoesNotThrow(() => sut.Individual_Period_Stats());
                    }
                }

                [TestFixture]
                class All_Stats
                {
                    // todo : should it count as a period active day if all work is in the ignore list?
                    [Test]
                    public void WhenPatternsIgnored_ShouldIgnoreMatchingFilesWhenCalculatingDeveloperStats()
                    {
                        // arrange
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt.orig")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("3", "4")
                            .With_Commit_Timestamp("2018-09-10 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("5", "6", "7")
                            .With_Commit_Timestamp("2018-09-10 13:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithIgnorePatterns(new[] {".orig"})
                            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-20"))
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats
                                {Author = author, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 53.19}
                        };
                        actual.Should().BeEquivalentTo(expected);
                    }

                    [Test]
                    public void WhenEntireHistory_ShouldReturnDeveloperStatsForLifetime()
                    {
                        // arrange;
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmil.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-08-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("3", "4")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2", "5", "7")
                            .With_Commit_Timestamp("2018-10-20 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithEntireHistory()
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats
                                {Author = author, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 44.44}
                        };
                        actual.Should().BeEquivalentTo(expected);
                    }

                    [Test]
                    public void WhenUsingAliasMapping_ShouldReturnOneDeveloperStats()
                    {
                        // arrange
                        var aliases = new List<Alias>
                        {
                            new Alias
                            {
                                Name = "T-rav",
                                Emails = new List<string> {"tmfrisinger@gmail.com", "travis@frisinger.com"}
                            }
                        };
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};
                        var aliasFile = Create_Alias_File(aliases);

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author.Name, author.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("3", "4")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_Author("Travis", "travis@frisinger.com")
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2", "5", "7")
                            .With_Commit_Timestamp("2018-09-20 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithAliasMapping(aliasFile)
                            .WithEntireHistory()
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expectedAuthor = new Author
                        {
                            Name = author.Name,
                            Emails = new List<string> { "tmfrisinger@gmail.com", "travis@frisinger.com" }
                        };
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats
                                {Author = expectedAuthor, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 44.44}
                        };
                        actual.Should().BeEquivalentTo(expected);
                    }

                    [Test]
                    public void WhenNotUsingAliases_ShouldReturnTwoDeveloperStats()
                    {
                        // arrange
                        var author1 = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com",}};
                        var author2 = new Author {Name = "Travis", Emails = new List<string> {"travis@frisinger.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(author1.Name, author1.Emails.First());

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("3", "4", "99", "1")
                            .With_Commit_Timestamp("2018-09-12 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_Author("Travis", "travis@frisinger.com")
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2", "5", "7")
                            .With_Commit_Timestamp("2018-09-20 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithEntireHistory()
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats
                                {Author = author1, ActiveDays = 2, AverageCommitsPerDay = 1.0, Ptt100 = 100.00},
                            new IndividualPeriodStats
                                {Author = author2, ActiveDays = 1, AverageCommitsPerDay = 1.0, Ptt100 = 200.00}
                        };
                        actual.Should().BeEquivalentTo(expected);
                    }
                }

                [TestFixture]
                class Ignore_Comments
                {
                    [Test]
                    public void WhenFalse_ShouldReturnStatsWithCommentedOutLines()
                    {
                        // arrange
                        var email = "tmfrisinger@gmail.com";
                        var authorName = "T-rav";
                        var author = new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(authorName, email);

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("// 1", "// 2")
                            .With_Commit_Timestamp("2018-09-10 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2", "5", "7")
                            .With_Commit_Timestamp("2018-09-10 13:05:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-10"))
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .WithIgnoreComments(false)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats
                                {Author = author, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 44.44}
                        };
                        actual.Should().BeEquivalentTo(expected);
                    }

                    [Test]
                    public void WhenTrue_ShouldReturnStatsIgnoringCommentedOutLines()
                    {
                        // arrange
                        var email = "tmfrisinger@gmail.com";
                        var authorName = "T-rav";
                        var author = new Author {Name = authorName, Emails = new List<string> {email,}};

                        var commitBuilder = new CommitTestDataBuilder()
                            .With_Author(authorName, email);

                        var commit1 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("1", "2")
                            .With_Commit_Timestamp("2018-09-10 01:01:01")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit2 = commitBuilder
                            .With_File_Name("file1.txt")
                            .With_File_Content("// 1", "// 2")
                            .With_Commit_Timestamp("2018-09-10 11:03:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var commit3 = commitBuilder
                            .With_File_Name("file3.txt")
                            .With_File_Content("1", "2", "5", "7")
                            .With_Commit_Timestamp("2018-09-10 13:05:02")
                            .With_Commit_Message("it worked!")
                            .Build();

                        var context = new RepositoryTestDataBuilder()
                            .After_Init_Commit_To_Master()
                            .Make_Commit(commit1)
                            .Make_Commit(commit2)
                            .Make_Commit(commit3)
                            .Build();

                        var sourceAnalysis = new SourceControlAnalysisBuilder()
                            .WithPath(context.Path)
                            .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-10"))
                            .WithWorkingDaysPerWeek(4)
                            .WithWorkingWeekHours(32)
                            .WithIgnoreComments(true)
                            .Build();

                        var sut = sourceAnalysis.Run_Analysis();
                        // act
                        var actual = sut.Individual_Period_Stats();
                        // assert
                        var expected = new List<IndividualPeriodStats>
                        {
                            new IndividualPeriodStats
                                {Author = author, ActiveDays = 3, AverageCommitsPerDay = 1.0, Ptt100 = 44.44}
                        };
                        actual.Should().BeEquivalentTo(expected);
                    }
                }
            }
        }

        private static string Create_Alias_File(List<Alias> aliases)
        {
            var filePath = Path.GetTempFileName();
            File.WriteAllText(filePath, aliases.Serialize());
            return filePath;
        }
    }
}
