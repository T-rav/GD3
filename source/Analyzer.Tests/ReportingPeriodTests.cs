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
        public class Period_Days
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
                var actual = sut.Period_Days();
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
                var actual = sut.Period_Days();
                // assert
                actual.Should().Be(0);
            }
        }

        [TestFixture]
        public class Period_Working_Days
        {
            [Test]
            public void WhenSingleWeek_ExpectOneWeekOfWorkingDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-07"),
                    DaysPerWeek = 4.5
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(4.5);
            }

            [Test]
            public void WhenTwoWeeks_ExpectTwoWeekOfWorkingDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-14"),
                    DaysPerWeek = 4.5
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(9);
            }

            [Test]
            public void WhenThreeAndHalfWeeks_ExpectThreeAndHalfWeeksOfWorkingDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-18"),
                    DaysPerWeek = 4.5
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(13.5);
            }
        }

        [TestFixture]
        public class Period_Weeks
        {
            [TestCase("2018-07-01", "2018-07-07", 1)]
            [TestCase("2018-07-01", "2018-07-18", 3.0)]
            public void WhenFourDayWeek_ExpectNumberOfDaysBetween(DateTime start, DateTime end, double expectedWeeks)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = start,
                    End = end,
                    DaysPerWeek = 4
                };
                // act
                var actual = sut.Period_Weeks();
                // assert
                actual.Should().Be(expectedWeeks);
            }

            [Test]
            public void WhenLessThanOneWeek_ExpectOneWeek()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-13"),
                    End = DateTime.Parse("2018-07-14"),
                    DaysPerWeek = 4.0,
                    HoursPerWeek = 32

                };
                // act
                var actual = sut.Period_Weeks();
                // assert
                var expected = 1;
                actual.Should().Be(expected);
            }
        }

        [TestFixture]
        public class Period_Working_Hours
        {
            [Test]
            public void WhenWholeNumberOfWeeks_ExpectHoursPerWeekTimesWeeks()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-14"),
                    DaysPerWeek = 4.0,
                    HoursPerWeek = 32

                };
                // act
                var actual = sut.Period_Working_Hours();
                // assert
                var expected = 64;
                actual.Should().Be(expected);
            }

            [Test]
            public void WhenFractionalNumberOfWeeks_ExpectHoursForNearestWholeWeekNumber()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-18"),
                    DaysPerWeek = 4.0,
                    HoursPerWeek = 32

                };
                // act
                var actual = sut.Period_Working_Hours();
                // assert
                var expected = 96;
                actual.Should().Be(expected);
            }
        }
    }
}
