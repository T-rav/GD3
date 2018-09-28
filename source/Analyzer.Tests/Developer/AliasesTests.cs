using Analyzer.Data.Developer;
using Analyzer.Domain.Developer;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Analyzer.Data.Tests.Developer
{
    [TestFixture]
    public class AliasesTests
    {
        [TestFixture]
        public class Map_To_Authors
        {
            [Test]
            public void When_SingleMatchInFile_ShouldReturnAuthorWithUnionOfEmailAddresses()
            {
                // arrange
                var authors = AuthorTestDataBuilder
                    .Create()
                    .WithAuthor("Brendon Page", "brendonp@gmail.com")
                    .Build();
                var repoPath = TestRepoPath("many-unique-aliases.json");
                
                var sut = new Aliases(repoPath);
                // act
                var actual = sut.Map_To_Authors(authors);
                // assert
                var expected = AuthorTestDataBuilder
                    .Create()
                    .WithAuthor("Brendon Page", "brendonp@gmail.com", "brendonpage@live.co.za")
                    .Build();
                actual.Should().BeEquivalentTo(expected);
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase("  ")]
            public void When_AliasFileNullOrWhitespace_ShouldReturnOriginalAuthor(string path)
            {
                // arrange
                var authors = AuthorTestDataBuilder
                    .Create()
                    .WithAuthor("Brendon Page", "brendonp@gmail.com")
                    .Build();

                var sut = new Aliases(path);
                // act
                var actual = sut.Map_To_Authors(authors);
                // assert
                var expected = AuthorTestDataBuilder
                    .Create()
                    .WithAuthor("Brendon Page", "brendonp@gmail.com")
                    .Build();
                actual.Should().BeEquivalentTo(expected);
            }
        }

        // TODO: no aliases
        // TODO: invalid format(s)
        // TODO: multiple aliases
        // TODO: email addresses shared across aliases in file
        // TODO: should use given author name
        // TODO: multiple matches
        // TODO: one match
        // TODO: no matches
        // TODO: match but only 1 email address

        private static string TestRepoPath(string fileName)
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            return Path.Combine(basePath, $"Developer\\AliasesTestData\\{fileName}");
        }
    }
}
