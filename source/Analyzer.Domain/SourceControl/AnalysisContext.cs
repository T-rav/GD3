using System.Collections.Generic;
using Analyzer.Domain.Reporting;

namespace Analyzer.Domain.SourceControl
{
    public class AnalysisContext
    {
        public ReportingPeriod ReportRange { get; set; }
        public Branch Branch { get; set; }
        public List<string> IgnorePatterns { get; set; }
        public bool IgnoreComments { get; set; }
    }
}
