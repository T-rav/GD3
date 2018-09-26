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
            public void When_AliasFileHasOneAlias()
            {
                // arrange
                var authors = new List<Author> { new Author { Name = "Brendon Page", Emails = new List<string> { "brendonp@gmail.com" } } };
                var repoPath = TestRepoPath("one-alias.json");

                var sut = new Aliases(repoPath);
                // act
                var actual = sut.Map_To_Authors(authors);
                // assert
                var expected = AliasesTestDataBuilder
                    .Create()
                    .WithAlias("Brendon Page", "brendonp@gmail.com", "brendonpage@live.co.za")
                    .Build();
                actual.Should().BeEquivalentTo(expected, o => o.Excluding(alias => alias.Id));
            }

            [TestCase(null)]
            [TestCase("")]
            [TestCase("  ")]
            public void When_AliasFileNullOrWhitespace(string path)
            {
                // arrange
                var authors = new List<Author> { new Author { Name = "Brendon Page", Emails = new List<string> { "brendonp@gmail.com" } } };

                var sut = new Aliases(path);
                // act
                var actual = sut.Map_To_Authors(authors);
                // assert
                var expected = AliasesTestDataBuilder
                    .Create()
                    .WithAlias("Brendon Page", "brendonp@gmail.com")
                    .Build();
                actual.Should().BeEquivalentTo(expected, o => o.Excluding(alias => alias.Id));
            }
        }

        // TODO: no aliases
        // TODO: invalid format(s)
        // TODO: multiple aliases

        private static string TestRepoPath(string repo)
        {
            var basePath = TestContext.CurrentContext.TestDirectory;
            return Path.Combine(basePath, $"Developer\\AliasRepositoryTestData\\{repo}");
        }
    }
}
