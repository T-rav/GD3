using System;

namespace Analyzer.Domain.SourceControl.Stats
{
    public class TeamStatsForDay
    {
        public int Commits { get; set; }
        public double Impact { get; set; }
        public double Churn { get; set; }
        public double RiskFactor { get; set; }
        public double Ptt100 { get; set; }
        public DateTime When { get; set; }
        public int ActiveDevelopers { get; set; }
    }
}