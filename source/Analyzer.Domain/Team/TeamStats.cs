using System;

namespace Analyzer.Domain.Team
{
    public class TeamStats
    {
        public int TotalCommits { get; set; }
        public int ActiveDevelopers { get; set; }

        public double Velocity => Math.Round(TotalCommits / (double)ActiveDevelopers,2);
    }
}