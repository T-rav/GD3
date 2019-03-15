using Analyzer.Domain.SourceControl;
using Analyzer.Domain.SourceControlV2;
using Analyzer.Domain.Stats;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Output;

namespace Analyzer.UseCase
{
    public class FullStatsUseCase : IFullStatsUseCase
    {
        private readonly ISourceControlAnalysisBuilder _builder;

        public FullStatsUseCase(ISourceControlAnalysisBuilder builder)
        {
            _builder = builder;
        }

        public void Execute(FullStatsInput inputTo, IRespondWithSuccessOrError<CodeAnalysis, ErrorOutputMessage> presenter)
        {
            using (var repo = _builder
                .WithPath(inputTo.Path)
                .WithEntireHistory()
                .WithIgnorePatterns(inputTo.IgnorePatterns)
                .WithBranch(inputTo.Branch)
                .WithWorkingDaysPerWeek(inputTo.DaysPerWeek)
                .WithWorkingWeekHours(inputTo.HoursPerWeek)
                .WithIgnoreComments(inputTo.IgnoreComments)
                .WithAliasMapping(inputTo.AliasFile)
                .WithWeekends(inputTo.WeekendDays)
                .Build())
            {
                //var authors = repo.List_Authors();
                //var stats = repo.Build_Individual_Developer_Stats(authors);
                //var dailyDeveloperStats = repo.Build_Daily_Individual_Developer_Stats(authors);
                //var teamStats = repo.Build_Team_Stats();

                var codeAnalysis = repo.Run_Analysis();

                // todo : use presenter to convert for output

                //var result = new StatsOutput
                //{
                //    Authors = authors,
                //    DeveloperStats = stats,
                //    DailyDeveloperStats = dailyDeveloperStats,
                //    TeamStats = teamStats,
                //    ReportingRange = repo.ReportingRange
                //};

                presenter.Respond(codeAnalysis);
            }
        }
    }
}
