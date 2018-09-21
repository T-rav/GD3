using Analyzer.Domain.Team;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Domain.Tests.Team
{
    [TestFixture]
    public class TeamStatsTests
    {
        [Test]
        public void Velocity_WhenTotalCommitsAndActiveDevelopersNonZero_ShouldReturnDivisionToTwoDecimalPlaces()
        {
            // arrange
            var sut = new TeamStats { TotalCommits = 1, ActiveDevelopers = 3 };
            // act
            var actual = sut.Velocity;
            // assert
            actual.Should().Be(0.33);
        }
    }
}