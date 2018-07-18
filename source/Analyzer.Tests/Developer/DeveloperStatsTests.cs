using Analyzer.Domain.Developer;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests.Developer
{
    [TestFixture]
    public class DeveloperStatsTests
    {
        [Test]
        public void RiskFactor_WhenLinesOfchangePerHourAndCommitsPerDayZero_ShouldReturnZero()
        {
            // arragne
            var sut = new DeveloperStats {LinesOfChangePerHour = 0, CommitsPerDay = 0};
            // act
            var actual = sut.RiskFactor;
            // assert
            actual.Should().Be(0.0);
        }

        [Test]
        public void RiskFactor_WhenLinesOfchangePerHourZeroAndCommitsPerDayNonZero_ShouldReturnZero()
        {
            // arragne
            var sut = new DeveloperStats { LinesOfChangePerHour = 0, CommitsPerDay = 10 };
            // act
            var actual = sut.RiskFactor;
            // assert
            actual.Should().Be(0.0);
        }

        [Test]
        public void RiskFactor_WhenLinesOfchangePerHourNonZeroAndCommitsPerDayZero_ShouldReturnZero()
        {
            // arragne
            var sut = new DeveloperStats { LinesOfChangePerHour = 10, CommitsPerDay = 0 };
            // act
            var actual = sut.RiskFactor;
            // assert
            actual.Should().Be(0.0);
        }

        [Test]
        public void RiskFactor_WhenLinesOfchangePerHourAndCommitsPerDayNonZero_ShouldReturnDivisionToTwoDecimalPlaces()
        {
            // arragne
            var sut = new DeveloperStats { LinesOfChangePerHour = 1, CommitsPerDay = 3 };
            // act
            var actual = sut.RiskFactor;
            // assert
            actual.Should().Be(0.33);
        }
    }
}
