using Analyzer.Data.Developer;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

namespace Analyzer.Data.Tests.Developer
{
    [TestFixture]
    public class AliasRepositoryTests
    {
        [TestFixture]
        public class Load
        {
            [Test]
            public void When_AliasFileHasOneAlias()
            {
                // arrange
                var repoPath = TestRepoPath("one-alias.json");
                var sut = new AliasRepository(repoPath);
                // act
                var actual = sut.Load();
                // assert
                var expected = AliasesTestDataBuilder
                    .Create()
                    .WithAlias("Brendon Page", "brendonp@gmail.com", "brendonpage@live.co.za")
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
