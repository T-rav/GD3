using Analyzer.Domain.SourceControl;
using TddBuddy.CleanArchitecture.Domain;

namespace Analyzer.Domain.Stats
{
    public interface IRangedStatsUseCase : IUseCase<RangedStatsInput, CodeAnalysis>
    {
    }
}