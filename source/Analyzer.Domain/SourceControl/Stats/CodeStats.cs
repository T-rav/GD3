using System.Collections.Generic;
using Analyzer.Domain.Reporting;

namespace Analyzer.Domain.SourceControl.Stats
{
    public class CodeStats
    {
        public ReportingPeriod ReportingPeriod { get; set; }

        public List<CommitStat> CommitStats { get; set; }

        public List<DeveloperStatsForPeriod> DeveloperStatsForPeriod { get; set; }
        public List<DeveloperStatsForDay> DeveloperStatsPerDay { get; set; }

        public List<TeamStatsForPeriod> TeamStatsForPeriod { get; set; }
        public List<TeamStatsForDay> TeamStatsPerDay { get; set; }
    }
}