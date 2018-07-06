using FluentAssertions;
using LibGit2Sharp;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void LearningTest_Goal_ReadGitHistory()
        {
            // arrange
            var repoPath = "C:\\Systems\\pdf-poc";
            var repo = new Repository(repoPath);
            // act
            var commits = repo.Head.Commits;
            // assert
            commits.Should().NotBeNull();
        }
    }
}
