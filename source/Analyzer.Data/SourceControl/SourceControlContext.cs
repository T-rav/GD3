using Analyzer.Domain.Reporting;
using System.Collections.Generic;

namespace Analyzer.Data.SourceControl
{
    public class SourceControlContext
    {
        public ReportingPeriod ReportRange { get; set; }
        public string Branch { get; set; }
        public List<string> IgnorePatterns { get; set; }
        public bool IgnoreComments { get; set; }
    }
}
