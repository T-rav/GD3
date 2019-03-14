using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControlV2
{
    public class IndividualPeriodStats
    {
        public Author Author { get; set; }
        public int ActiveDays { get; set; }
    }
}