using System.Collections.Generic;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.Team;

namespace Analyzer.Domain.Stats
{
    public class StatsOuput
    {
        public IList<Author> Authors { get; set; }
        public IList<DeveloperStats> DeveloperStats { get; set; }
        public TeamStatsCollection TeamStats { get; set; }
        public ReportingPeriod ReportingRange { get; set; }
    }
}