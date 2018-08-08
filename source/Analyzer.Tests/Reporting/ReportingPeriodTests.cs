using System;
using System.Collections.Generic;
using Analyzer.Domain.Reporting;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests.Reporting
{
    [TestFixture]
    public class ReportingPeriodTests
    {
        [TestFixture]
        public class Construction
        {
            [Test]
            public void Should_Initalize_Weekends()
            {
                // arrange
                // act
                var sut = new ReportingPeriod();
                // assert
                sut.Weekends.Should().NotBeNull();
            }
        }

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
                    DaysPerWeek = 4.5,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(4.5);
            }

            [TestCase(4.5, 1.0)]
            [TestCase(4.0, 1.0)]
            public void WhenSingleDay_ExpectOneWorkingDay(double daysPerWeek, double expected)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-08-08"),
                    End = DateTime.Parse("2018-08-08"),
                    DaysPerWeek = daysPerWeek,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(expected);
            }

            [TestCase(4.5, 2.0)]
            [TestCase(4.0, 2.0)]
            public void WhenTwoDays_ExpectTwoWorkingDay(double daysPerWeek, double expected)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-08-08"),
                    End = DateTime.Parse("2018-08-09"),
                    DaysPerWeek = daysPerWeek,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(expected);
            }

            [TestCase(4.0, 5.0)]
            [TestCase(4.5, 5.5)]
            public void WhenOneWeekAndOneDay_ExpectDaysPerWeekPlusOneWorkingDays(double daysPerWeek, double expected)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-30"),
                    End = DateTime.Parse("2018-08-06"),
                    DaysPerWeek = daysPerWeek,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday}
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(expected);
            }

            [Test]
            public void WhenTwoWeeks_ExpectTwoWeekOfWorkingDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-14"),
                    DaysPerWeek = 4.5,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(9);
            }

            [Test]
            public void WhenTwoAndHalfWeeks_ExpectThreeWeeksOfWorkingDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-18"),
                    DaysPerWeek = 4.5,
                    Weekends = new List<DayOfWeek> {  DayOfWeek.Saturday, DayOfWeek.Sunday}
                };
                // act
                var actual = sut.Period_Working_Days();
                // assert
                actual.Should().Be(11.5);
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
                    HoursPerWeek = 32,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }

                };
                // act
                var actual = sut.Period_Working_Hours();
                // assert
                var expected = 64;
                actual.Should().Be(expected);
            }

            [Test]
            public void WhenFractionalNumberOfWeeks_ExpectHoursToNearestNumberOfDays()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-18"),
                    DaysPerWeek = 4.0,
                    HoursPerWeek = 32,
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday}

                };
                // act
                var actual = sut.Period_Working_Hours();
                // assert
                var expected = 88;
                actual.Should().Be(expected);
            }
        }

        [TestFixture]
        public class Generate_Dates_For_Range
        {
            [Test]
            public void WhenStartBeforeEnd_ExpectListOfDaysBetweenAndIncludingStartAndEnd()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-02"),
                    End = DateTime.Parse("2018-07-06")
                };
                // act
                var actual = sut.Generate_Dates_For_Range();
                // assert
                var expected = new List<DateTime>
                {
                    DateTime.Parse("2018-07-02"),
                    DateTime.Parse("2018-07-03"),
                    DateTime.Parse("2018-07-04"),
                    DateTime.Parse("2018-07-05"),
                    DateTime.Parse("2018-07-06")
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [Test]
            public void WhenRangeTwoWeeks_ExpectListOfDaysExcludingWeekends()
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = DateTime.Parse("2018-07-01"),
                    End = DateTime.Parse("2018-07-15"),
                    Weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }
                };
                // act
                var actual = sut.Generate_Dates_For_Range();
                // assert
                var expected = new List<DateTime>
                {
                    DateTime.Parse("2018-07-02"),
                    DateTime.Parse("2018-07-03"),
                    DateTime.Parse("2018-07-04"),
                    DateTime.Parse("2018-07-05"),
                    DateTime.Parse("2018-07-06"),
                    DateTime.Parse("2018-07-09"),
                    DateTime.Parse("2018-07-10"),
                    DateTime.Parse("2018-07-11"),
                    DateTime.Parse("2018-07-12"),
                    DateTime.Parse("2018-07-13")
                };
                actual.Should().BeEquivalentTo(expected);
            }

            [TestCase("2018-07-05", "2018-07-01")]
            [TestCase("2018-07-05", "2018-07-04")]
            public void WhenEndBeforeStart_ExpectEmptyList(DateTime start, DateTime end)
            {
                // arrange
                var sut = new ReportingPeriod
                {
                    Start = start,
                    End = end
                };
                // act
                var actual = sut.Generate_Dates_For_Range();
                // assert
                actual.Should().BeEmpty();
            }
        }
    }
}
