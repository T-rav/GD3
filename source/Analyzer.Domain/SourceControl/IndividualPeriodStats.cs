using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControl
{
    public class IndividualPeriodStats
    {
        public Author Author { get; set; }
        public int ActiveDays { get; set; }
        public double AverageCommitsPerDay { get; set; }
        public double Ptt100 { get; set; }
    }
}