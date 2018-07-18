using System;

namespace Analyzer.Domain.Team
{
    public class TeamStats
    {
        public DateTime DateOf { get; set; }

        public int TotalCommits { get; set; }
        public int ActiveDevelopers { get; set; }

        public double Velocity => Math.Round(TotalCommits / (double)ActiveDevelopers,2);

        public override string ToString()
        {
            return $"{PaddedPrint(DateOf.ToString("yyyy-MM-dd"), 18)}" +
                   $"{PaddedPrint(TotalCommits, 17)}" +
                   $"{PaddedPrint(ActiveDevelopers, 21)}" +
                   $"{PaddedPrint(Velocity, 12)}";
        }

        private string PaddedPrint(object value, int fieldWidth)
        {
            return value.ToString().PadRight(fieldWidth, ' ');
        }
    }
}