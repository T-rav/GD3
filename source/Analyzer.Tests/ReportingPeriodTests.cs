using System;
using Analyzer.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class ReportingPeriodTests
    {
        [TestFixture]
        public class TotalDays
        {
            [Test]
            public void WhenStartBeforeEnd_ExpectNumberOfDaysBetween()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-05")
                };
                // act
                var actual = sut.Total_Days();
                // assert
                actual.Should().Be(5);
            }

            [TestCase("2018-07-05", "2018-07-01")]
            [TestCase("2018-07-05", "2018-07-04")]
            public void WhenEndBeforeStart_ExpectZeroDaysBetween(DateTime start, DateTime end)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = start,
                    End = end
                };
                // act
                var actual = sut.Total_Days();
                // assert
                actual.Should().Be(0);
            }
        }

        [TestFixture]
        public class WorkingDays
        {
            [Test]
            public void WhenSingleWeek_ExpectFiveWorkingDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-07")
                };
                // act
                var actual = sut.Working_Days();
                // assert
                actual.Should().Be(5);
            }

            [TestCase("2018-07-05", "2018-07-01")]
            [TestCase("2018-07-05", "2018-07-04")]
            public void WhenEndBeforeStart_ExpectZeroDaysBetween(DateTime start, DateTime end)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = start,
                    End = end
                };
                // act
                var actual = sut.Total_Days();
                // assert
                actual.Should().Be(0);
            }
        }
    }
}
