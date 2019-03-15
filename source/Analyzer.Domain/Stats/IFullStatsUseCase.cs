using Analyzer.Domain.SourceControl;
using TddBuddy.CleanArchitecture.Domain;

namespace Analyzer.Domain.Stats
{
    public interface IFullStatsUseCase : IUseCase<FullStatsInput, CodeAnalysis>
    {
    }
}