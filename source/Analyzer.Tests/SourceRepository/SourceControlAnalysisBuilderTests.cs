using Analyzer.Data.SourceControl;
using Analyzer.Data.Test.Utils;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

namespace Analyzer.Data.Tests.SourceRepository
{
    [TestFixture]
    public class SourceControlAnalysisBuilderTests
    {
        [Test]
        public void WhenPathNotValidGitRepo_ShouldThrowException()
        {
            // arrange
            var repoPath = "x:\\invalid_repo";
            var builder = new SourceControlAnalysisBuilder()
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
            var context = new RepositoryTestDataBuilder()
                        .Build();

            var sut = new SourceControlAnalysisBuilder()
                .WithPath(context.Path)
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
            var commitBuilder = new CommitTestDataBuilder()
                .With_Author("bob", "bob@shucks.io");

            var commit1 = commitBuilder
                .With_File_Name("file1.txt")
                .With_File_Content("1", "2")
                .With_Commit_Timestamp("2018-07-16 01:01:01")
                .With_Commit_Message("it worked!")
                .Build();

            var commit2 = commitBuilder
                .With_File_Name("file2.txt")
                .With_File_Content("3", "4")
                .With_Commit_Timestamp("2018-09-13 12:12:12")
                .With_Commit_Message("it worked again!")
                .Build();

            var context = new RepositoryTestDataBuilder()
                          .Make_Commit(commit1)
                          .Make_Commit(commit2)
                          .Build();
            var sut = new SourceControlAnalysisBuilder()
                .WithPath(context.Path)
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
            var context = new RepositoryTestDataBuilder()
                          .After_Init_Commit_To_Master()
                          .Build();
            var sut = new SourceControlAnalysisBuilder()
                .WithPath(context.Path)
                .WithIgnorePatterns(null);
            // act
            // assert
            Assert.DoesNotThrow(() => sut.Build());
        }
    }
}
