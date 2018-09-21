using Analyzer.Domain.Reporting;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Domain.Tests.Reporting
{
    [TestFixture]
    public class ImpactTests
    {
        [Test]
        public void When1NewLinesIn1NewFile_ShouldReturn100thOfBaseLineImpact()
        {
            // arrange
            var sut = new Impact
            {
                TotalLinesEdited = 1,
                TotalLinesOfOldCode = 0,
                TotalFiles = 1,
                TotalEditLocations = 1
            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.001);
        }

        [Test]
        public void When1NewLinesIn1OldFile_ShouldReturn50thOfBaseLineImpact()
        {
            // arrange
            var sut = new Impact
            {
                TotalLinesEdited = 1,
                TotalLinesOfOldCode = 1,
                TotalFiles = 1,
                TotalEditLocations = 1
            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.0015);
        }

        [Test]
        public void When100NewLinesIn1NewFile_ShouldReturnBaseLineImpact()
        {
            // arrange
            var sut = new Impact
            {
                TotalLinesEdited = 100,
                TotalLinesOfOldCode = 0,
                TotalFiles = 1,
                TotalEditLocations = 1
            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.1);
        }

        [Test]
        public void When100NewLinesIn2NewFile_ShouldReturnBaseLineImpactTimeTwo()
        {
            var sut = new Impact
            {
                TotalLinesEdited = 100,
                TotalLinesOfOldCode = 0,
                TotalFiles = 1,
                TotalEditLocations = 2

            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.2);
        }

        [Test]
        public void When100NewLinesIn1OldFile_ShouldReturnModerateImpact()
        {
            var sut = new Impact
            {
                TotalLinesEdited = 100,
                TotalLinesOfOldCode = 100,
                TotalFiles = 1,
                TotalEditLocations = 2

            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.3);
        }

        [Test]
        public void When100LinesIn2OldFile_ShouldReturnModeratImpact()
        {
            var sut = new Impact
            {
                TotalLinesEdited = 100,
                TotalLinesOfOldCode = 100,
                TotalFiles = 2,
                TotalEditLocations = 2

            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.6);
        }

        [Test]
        public void When100LinesIn10OldFilesWith20Locations_ShouldReturnLargeImpact()
        {
            var sut = new Impact
            {
                TotalLinesEdited = 100,
                TotalLinesOfOldCode = 100,
                TotalFiles = 10,
                TotalEditLocations = 20

            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(30.0);
        }

        [Test]
        public void WhenNoChanges_ShouldReturnZeroImpact()
        {
            // arrange
            var sut = new Impact
            {
                TotalLinesEdited = 0,
                TotalLinesOfOldCode = 0,
                TotalFiles = 0,
                TotalEditLocations = 0
            };
            // act
            var actual = sut.Calculate();
            // assert
            actual.Should().Be(0.0);
        }
    }
}
