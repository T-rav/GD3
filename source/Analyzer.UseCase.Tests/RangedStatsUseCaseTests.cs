using System;
using System.Collections.Generic;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.SourceControl;
using Analyzer.Domain.Stats;
using Analyzer.Domain.Team;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Presenters;

namespace Analyzer.UseCase.Tests
{
    [TestFixture]
    public class RangedStatsUseCaseTests
    {
        [Test]
        [Ignore("wip")]
        public void Execute_WhenRepoDetailsValid_ShouldReturnStats()
        {
            //---------------Arrange------------------
            var inputTo = new RangedStatsInput();
            var author = new Author {Name = "test author", Emails = new List<string> {"test@author.com"}};
            var developerStats = new List<TeamStats>
            {
                new TeamStats {ActiveDevelopers = 2, DateOf = DateTime.Parse("2018-10-10"), TotalCommits = 5}
            };
            var weekends = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
            var reportingRange = new ReportingPeriod
            {
                Start = DateTime.Parse("2018-10-05"),
                End = DateTime.Parse("2018-10-15")
            };

            var presenter = new PropertyPresenter<CodeAnalysis, ErrorOutputMessage>();
            var repo = Create_Valid_Repository_Mocks(author, reportingRange);

            var builder = Substitute.For<SourceControlAnalysisBuilder>();
            builder
                .WithPath(Arg.Any<string>())
                .WithRange(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .WithIgnorePatterns(Arg.Any<IEnumerable<string>>())
                .WithBranch(Arg.Any<string>())
                .WithWorkingDaysPerWeek(Arg.Any<double>())
                .WithWorkingWeekHours(Arg.Any<int>())
                .WithIgnoreComments(Arg.Any<bool>())
                .WithAliasMapping(Arg.Any<string>())
                .WithWeekends(Arg.Any<IEnumerable<DayOfWeek>>())
                .Build()
                .Returns(repo);

            var sut = new RangedStatsUseCase(builder);
            //---------------Act----------------------
            sut.Execute(inputTo, presenter);
            //---------------Assert-------------------
            var expected = new StatsOutput
            {
                Authors = new List<Author>{author},
                DeveloperStats = new List<DeveloperStats>
                {
                    new DeveloperStats
                    {
                        ActiveDaysPerWeek = 1.5,
                        Author = author,
                        Churn = 0.9,
                        CommitsPerDay = 1.2,
                        Impact = 9.9,
                        LinesAdded = 10,
                        LinesRemoved = 5,
                        LinesOfChangePerHour = 2.2,
                        PeriodActiveDays = 1,
                        Ptt100 = 4.5,
                        Rtt100 = 6.2
                    }
                },
                DailyDeveloperStats = new List<DailyDeveloperStats>
                {
                    new DailyDeveloperStats
                    {
                        Date = DateTime.Parse("2018-10-05"),
                        Stats = new List<DeveloperStats>
                        {

                        }
                    }
                },
                TeamStats = new TeamStatsCollection(developerStats, weekends),
                ReportingRange = reportingRange
            };
            presenter.SuccessContent.Should().BeEquivalentTo(expected);
        }

        private static SourceControlAnalysis Create_Valid_Repository_Mocks(Author author, ReportingPeriod reportingRange)
        {
            var repo = Substitute.For<SourceControlAnalysis>();

            repo.Run_Analysis().Returns(new CodeAnalysis(new List<Author> { author }, new List<Commit>(), new AnalysisContext { ReportRange = reportingRange }));

            //repo.List_Authors()
            //    .Returns(new List<Author>
            //    {
            //        author
            //    });

            //repo.Build_Individual_Developer_Stats(Arg.Any<List<Author>>())
            //    .Returns(new List<DeveloperStats>
            //    {
            //        new DeveloperStats
            //        {
            //            ActiveDaysPerWeek = 1.5,
            //            Author = author,
            //            Churn = 0.9,
            //            CommitsPerDay = 1.2,
            //            Impact = 9.9,
            //            LinesAdded = 10,
            //            LinesRemoved = 5,
            //            LinesOfChangePerHour = 2.2,
            //            PeriodActiveDays = 1,
            //            Ptt100 = 4.5,
            //            Rtt100 = 6.2
            //        }
            //    });

            //repo.Build_Daily_Individual_Developer_Stats(Arg.Any<IList<Author>>())
            //    .Returns(new List<DailyDeveloperStats>
            //    {
            //        new DailyDeveloperStats
            //        {
            //            Date = DateTime.Parse("2018-10-05"),
            //            Stats = new List<DeveloperStats>()
            //        }
            //    });

            //repo.Build_Team_Stats().Returns(new TeamStatsCollection(developerStats, weekends));

            return repo;
        }
    }
}
