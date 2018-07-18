using System;

namespace Analyzer.Domain.Team
{
    public class TeamStats
    {
        public DateTime DateOf { get; set; }

        public int TotalCommits { get; set; }
        public int ActiveDevelopers { get; set; }

        public double Velocity => Math.Round(TotalCommits / (double)ActiveDevelopers,2);
    }
}