using System;
using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControl.Stats
{
    public class DeveloperStatsForDay
    {
        public int Commits { get; set; }
        public double Impact { get; set; }
        public double Churn { get; set; }
        public double RiskFactor { get; set; }
        public double Ptt100 { get; set; }
        public Author Author { get; set; }
        public DateTime When { get; set; }
    }
}