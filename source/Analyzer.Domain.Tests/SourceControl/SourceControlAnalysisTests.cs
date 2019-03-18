//using System;
//using System.Collections.Generic;
//using System.IO;
//using Analyzer.Domain.Developer;
//using Analyzer.Domain.SourceControl;
//using Analyzer.Test.Utils;
//using FluentAssertions;
//using NUnit.Framework;
//using StoneAge.System.Utils.Json;

//namespace Analyzer.Domain.Tests.SourceControl
//{
//    [TestFixture]
//    public class SourceControlAnalysisTests
//    {
//        [TestFixture]
//        class Run_Analysis
//        {
//            [TestFixture]
//            class Authors
//            {
//                [Test]
//                public void WhenMaster_ShouldReturnAllActiveDevelopers()
//                {
//                    // arrange
//                    var authorName = "T-rav";

//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author(authorName, "tmfrisinger@gmail.com");

//                    var commit1 = commitBuilder
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-16 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .Make_Commit(commit1)
//                        .Build();

//                    var sut = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithRange(DateTime.Parse("2018-7-16"), DateTime.Parse("2018-07-17"))
//                        .Build();
//                    // act
//                    var actual = sut.Run_Analysis();
//                    // assert
//                    var expected = new List<Author>
//                    {
//                        new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}}
//                    };
//                    actual.Authors.Should().BeEquivalentTo(expected);
//                }

//                [Test]
//                public void WhenDeveloperBranch_ShouldReturnAllActiveDevelopers()
//                {
//                    // arrange
//                    var branchName = "my-branch";
//                    var authorName = "Travis";
//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author(authorName, "travis@frisinger.com");

//                    var commit1 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-16 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit2 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_Author("T-rav", "tmfrisinger@gmail.com")
//                        .With_File_Name("file2.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-17 11:03:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .After_Init_Commit_To_Master()
//                        .On_Branch(branchName)
//                        .Make_Commit(commit1)
//                        .Make_Commit(commit2)
//                        .Build();

//                    var sut = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithBranch(branchName)
//                        .WithRange(DateTime.Parse("2018-7-16"), DateTime.Parse("2018-07-17"))
//                        .Build();
//                    // act
//                    var actual = sut.Run_Analysis();
//                    // assert
//                    var expected = new List<Author>
//                    {
//                        new Author {Name = "T-rav", Emails = new List<string> {"tmfrisinger@gmail.com"}},
//                        new Author {Name = "Travis", Emails = new List<string> {"travis@frisinger.com"}}
//                    };
//                    actual.Authors.Should().BeEquivalentTo(expected);
//                }


//                [Test]
//                public void WhenUsingAlias_ShouldReturnSingleDeveloperWithTwoEmails()
//                {
//                    // arrange
//                    var branchName = "my-branch";
//                    var authorName = "T-rav";
//                    var aliasMap = new List<Alias>
//                    {
//                        new Alias
//                        {
//                            Name = authorName,
//                            Emails = new List<string> {"tmfrisinger@gmail.com", "travis@frisinger.com"}
//                        }
//                    };

//                    var aliasFile = Create_Alias_File(aliasMap);

//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author(authorName, "tmfrisinger@gmail.com");

//                    var commit1 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-16 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit2 = commitBuilder
//                        .With_Author(authorName, "travis@frisinger.com")
//                        .With_Branch(branchName)
//                        .With_File_Name("file2.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-17 11:03:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .After_Init_Commit_To_Master()
//                        .On_Branch(branchName)
//                        .Make_Commit(commit1)
//                        .Make_Commit(commit2)
//                        .Build();

//                    var sut = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithEntireHistory()
//                        .WithBranch("my-branch")
//                        .WithAliasMapping(aliasFile)
//                        .Build();
//                    // act
//                    var actual = sut.Run_Analysis();
//                    // assert
//                    var expected = 1;
//                    actual.Authors.Count.Should().Be(expected);
//                }

//                [Test]
//                public void WhenNullAliases_ShouldReturnTwoDevelopers()
//                {
//                    // arrange
//                    var branchName = "my-branch";
//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author("T-rav", "tmfrisinger@gmail.com");

//                    var commit1 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-16 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit2 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_Author("Travis", "travis@frisinger.com")
//                        .With_File_Name("file2.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-17 11:03:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .After_Init_Commit_To_Master()
//                        .On_Branch(branchName)
//                        .Make_Commit(commit1)
//                        .Make_Commit(commit2)
//                        .Build();

//                    var sut = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithEntireHistory()
//                        .WithBranch(branchName)
//                        .Build();
//                    // act
//                    var actual = sut.Run_Analysis();
//                    // assert
//                    var expected = 2;
//                    actual.Authors.Count.Should().Be(expected);
//                }

//                [Test]
//                public void WhenEmptyAliases_ShouldReturnTwoDevelopers()
//                {
//                    // arrange
//                    var branchName = "my-branch";
//                    var aliasMap = new List<Alias>();

//                    var aliasFile = Create_Alias_File(aliasMap);

//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author("T-rav", "tmfrisinger@gmail.com");

//                    var commit1 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-16 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit2 = commitBuilder
//                        .With_Branch(branchName)
//                        .With_Author("Travis", "travis@frisinger.com")
//                        .With_File_Name("file2.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-07-17 11:03:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .After_Init_Commit_To_Master()
//                        .On_Branch(branchName)
//                        .Make_Commit(commit1)
//                        .Make_Commit(commit2)
//                        .Build();

//                    var sut = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithEntireHistory()
//                        .WithBranch(branchName)
//                        .WithAliasMapping(aliasFile)
//                        .Build();
//                    // act
//                    var actual = sut.Run_Analysis();
//                    // assert
//                    var expected = 2;
//                    actual.Authors.Count.Should().Be(expected);
//                }
//            }

//            [TestFixture]
//            class Ignore_Comments
//            {
//                [Test]
//                public void WhenFalse_ShouldReturnStatsWithCommentedOutLines()
//                {
//                    // arrange
//                    var email = "tmfrisinger@gmail.com";
//                    var authorName = "T-rav";
//                    var author = new Author { Name = "T-rav", Emails = new List<string> { "tmfrisinger@gmail.com" } };

//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author(authorName, email);

//                    var commit1 = commitBuilder
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-09-10 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit2 = commitBuilder
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("// 1", "// 2")
//                        .With_Commit_Timestamp("2018-09-10 11:03:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit3 = commitBuilder
//                        .With_File_Name("file3.txt")
//                        .With_File_Content("1", "2", "5", "7")
//                        .With_Commit_Timestamp("2018-09-10 13:05:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .After_Init_Commit_To_Master()
//                        .Make_Commit(commit1)
//                        .Make_Commit(commit2)
//                        .Make_Commit(commit3)
//                        .Build();

//                    var sourceAnalysis = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-10"))
//                        .WithWorkingDaysPerWeek(4)
//                        .WithWorkingWeekHours(32)
//                        .WithIgnoreComments(false)
//                        .Build();

//                    var sut = sourceAnalysis.Run_Analysis();
//                    // act
//                    var actual = sut.Individual_Period_Stats();
//                    // assert
//                    var expected = new List<CommitStat>
//                        {
//                            new CommitStat
//                                {Author = author, ActiveDays = 3, Commits = 1.0, Ptt100 = 44.44}
//                        };
//                    actual.Should().BeEquivalentTo(expected);
//                }

//                [Test]
//                public void WhenTrue_ShouldReturnStatsIgnoringCommentedOutLines()
//                {
//                    // arrange
//                    var email = "tmfrisinger@gmail.com";
//                    var authorName = "T-rav";
//                    var author = new Author { Name = authorName, Emails = new List<string> { email, } };

//                    var commitBuilder = new CommitTestDataBuilder()
//                        .With_Author(authorName, email);

//                    var commit1 = commitBuilder
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("1", "2")
//                        .With_Commit_Timestamp("2018-09-10 01:01:01")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit2 = commitBuilder
//                        .With_File_Name("file1.txt")
//                        .With_File_Content("// 1", "// 2")
//                        .With_Commit_Timestamp("2018-09-10 11:03:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var commit3 = commitBuilder
//                        .With_File_Name("file3.txt")
//                        .With_File_Content("1", "2", "5", "7")
//                        .With_Commit_Timestamp("2018-09-10 13:05:02")
//                        .With_Commit_Message("it worked!")
//                        .Build();

//                    var context = new RepositoryTestDataBuilder()
//                        .After_Init_Commit_To_Master()
//                        .Make_Commit(commit1)
//                        .Make_Commit(commit2)
//                        .Make_Commit(commit3)
//                        .Build();

//                    var sourceAnalysis = new SourceControlAnalysisBuilder()
//                        .WithPath(context.Path)
//                        .WithRange(DateTime.Parse("2018-09-10"), DateTime.Parse("2018-09-10"))
//                        .WithWorkingDaysPerWeek(4)
//                        .WithWorkingWeekHours(32)
//                        .WithIgnoreComments(true)
//                        .Build();

//                    var sut = sourceAnalysis.Run_Analysis();
//                    // act
//                    var actual = sut.Individual_Period_Stats();
//                    // assert
//                    var expected = new List<CommitStat>
//                        {
//                            new CommitStat
//                                {Author = author, ActiveDays = 3, Commits = 1.0, Ptt100 = 44.44}
//                        };
//                    actual.Should().BeEquivalentTo(expected);
//                }
//            }
//        }

//        private static string Create_Alias_File(List<Alias> aliases)
//        {
//            var filePath = Path.GetTempFileName();
//            File.WriteAllText(filePath, aliases.Serialize());
//            return filePath;
//        }
//    }
//}
