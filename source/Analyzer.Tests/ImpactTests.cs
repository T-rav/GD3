using Analyzer.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class ImpactTests
    {
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
            actual.Should().Be(0.01);
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
            actual.Should().Be(0.02);
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
            actual.Should().Be(0.04);
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
            actual.Should().Be(0.08);
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
            actual.Should().Be(4.0);
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
