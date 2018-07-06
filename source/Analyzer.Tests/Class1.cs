using FluentAssertions;
using LibGit2Sharp;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class Class1
    {
        // https://stackoverflow.com/questions/13122138/what-is-the-libgit2sharp-equivalent-of-git-log-path
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
