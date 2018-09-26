using Analyzer.Domain.Reporting;
using System.Collections.Generic;
using Analyzer.Domain.SourceRepository;

namespace Analyzer.Data.SourceControl
{
    public class AnalysisContext
    {
        public ReportingPeriod ReportRange { get; set; }
        public Branch Branch { get; set; }
        public List<string> IgnorePatterns { get; set; }
        public bool IgnoreComments { get; set; }
    }
}
