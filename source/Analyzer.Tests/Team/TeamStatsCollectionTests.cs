using System;
using System.Collections.Generic;
using Analyzer.Domain.Team;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests.Team
{
    [TestFixture]
    public class TeamStatsCollectionTests
    {
        [Test]
        public void WorkDays_WhenWeekendsSet_ShouldReturnDaysWithoutWeekends()
        {
            // arrange
            var stats = new List<TeamStats>
            {
                new TeamStats {DateOf = DateTime.Parse("2018-07-22")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-23")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-24")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-25")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-26")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-27")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-28")},
            };
            var weekends = new List<DayOfWeek>
            {
                DayOfWeek.Saturday,
                DayOfWeek.Sunday
            };

            var sut = new TeamStatsCollection(stats, weekends);
            // act
            var actual = sut.GetWorkDayStats();
            // assert
            var expected = new List<TeamStats>
            {
                new TeamStats {DateOf = DateTime.Parse("2018-07-23")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-24")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-25")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-26")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-27")}
            };
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void WorkDays_WhenWeekendsNotSet_ShouldReturnAllDays()
        {
            // arrange
            var stats = new List<TeamStats>
            {
                new TeamStats {DateOf = DateTime.Parse("2018-07-22")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-23")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-24")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-25")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-26")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-27")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-28")}
            };

            var sut = new TeamStatsCollection(stats, null);
            // act
            var actual = sut.GetWorkDayStats();
            // assert
            var expected = new List<TeamStats>
            {
                new TeamStats {DateOf = DateTime.Parse("2018-07-22")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-23")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-24")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-25")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-26")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-27")},
                new TeamStats {DateOf = DateTime.Parse("2018-07-28")}
            };
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
