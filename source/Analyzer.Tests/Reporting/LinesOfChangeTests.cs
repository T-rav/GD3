using Analyzer.Domain.Reporting;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests.Reporting
{
    [TestFixture]
    public class LinesOfChangeTests
    {
        [Test]
        public void Churn_WhenRemovedAndAddedZero_ShouldReturnZero()
        {
            // arrange
            var sut = new LinesOfChange { Removed = 0, Added = 0 };
            // act
            var actual = sut.Churn;
            // assert
            actual.Should().Be(0.0);
        }

        [Test]
        public void Churn_WhenAddedZeroRemovedNonZero_ShouldReturnZero()
        {
            // arrange
            var sut = new LinesOfChange { Removed = 10, Added = 0 };
            // act
            var actual = sut.Churn;
            // assert
            actual.Should().Be(0.0);
        }

        [Test]
        public void Churn_WhenRemovedZeroAddedNonZero_ShouldReturnZero()
        {
            // arrange
            var sut = new LinesOfChange { Removed = 0, Added = 10 };
            // act
            var actual = sut.Churn;
            // assert
            actual.Should().Be(0.0);
        }

        [Test]
        public void Churn_WhenRemovedAndAddedNonZero_ShouldReturnRemovedDividedByAddedToTwoDecimals()
        {
            // arrange
            var sut = new LinesOfChange { Removed = 1, Added = 3 };
            // act
            var actual = sut.Churn;
            // assert
            actual.Should().Be(0.33);
        }

        [Test]
        public void TotalLines_ShouldSumOfAddedAndRemoved()
        {
            // arrange
            var sut = new LinesOfChange { Removed = 5, Added = 10 };
            // act
            var actual = sut.TotalLines;
            // assert
            actual.Should().Be(15);
        }
    }
}
