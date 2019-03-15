using Analyzer.Domain.SourceControl;
using Analyzer.Domain.SourceControlV2;
using Analyzer.Domain.Stats;
using TddBuddy.CleanArchitecture.Domain.Messages;
using TddBuddy.CleanArchitecture.Domain.Output;

namespace Analyzer.UseCase
{
    public class RangedStatsUseCase : IRangedStatsUseCase
    {
        private readonly ISourceControlAnalysisBuilder _builder;

        public RangedStatsUseCase(ISourceControlAnalysisBuilder builder)
        {
            _builder = builder;
        }

        public void Execute(RangedStatsInput inputTo, IRespondWithSuccessOrError<CodeAnalysis, ErrorOutputMessage> presenter)
        {
            using (var repo = _builder
                .WithPath(inputTo.Path)
                .WithRange(inputTo.RangeState, inputTo.RangeEnd)
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

                //var result = new StatsOutput
                //{
                //    Authors = authors,
                //    DeveloperStats = stats,
                //    DailyDeveloperStats = dailyDeveloperStats,
                //    TeamStats = teamStats,
                //    ReportingRange = repo.ReportingRange
                //};

                var codeAnalysis = repo.Run_Analysis();

                presenter.Respond(codeAnalysis);
            }
        }
    }
}
