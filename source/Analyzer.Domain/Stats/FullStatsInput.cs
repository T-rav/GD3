using System;
using System.Collections.Generic;
using Analyzer.Domain.Reporting;

namespace Analyzer.Domain.Stats
{
    public class FullStatsInput
    {
        public ReportingPeriod AnalysisPeriod { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> IgnorePatterns { get; set; }
        public string Branch { get; set; }
        public IEnumerable<DayOfWeek> WeekDays { get; set; }
        public double DaysPerWeek { get; set; }
        public int HoursPerWeek { get; set; }
        public bool IgnoreComments { get; set; }
        public string AliasFile { get; set; }
        public IEnumerable<DayOfWeek> WeekendDays { get; set; }
    }
}