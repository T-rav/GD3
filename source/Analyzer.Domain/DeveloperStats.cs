using System;

namespace Analyzer.Domain
{
    public class DeveloperStats
    {
        public Author Author { get; set; }
        public int PeriodActiveDays { get; set; }
        public double ActiveDaysPerWeek { get; set; } // todo : make this / sprint and allow user to set sprint size (default to week)
        public double CommitsPerDay { get; set; } // do they push often
        public double Impact { get; set; } // congative load
        public double LinesOfChangePerHour { get; set; } // how much of a wake to they cause?
        public double Churn { get; set; } // how much code is deleted?
        public double RiskFactor => Math.Round( (LinesOfChangePerHour / CommitsPerDay),2 ); // do they move quick, make a mess and cause high churn?! (LinesOfChangePerHour/CommitsPerDay)*Impact
        public int LinesAdded { get; set; }
        public int LinesRemoved { get; set; }
        public double Rtt100 { get; set; }
        public double Ptt100 { get; set; }

        public double Dtt100 => Ptt100 - Rtt100;


        // todo : track which lines the developer changed over the period
        // todo: would still like to know what % of recent technical debt they contributed (based on period of reporting)
        public override string ToString()
        {
            return //$"{PaddedPrint(Rank, 7)}" +
                   $"{PaddedPrint(Author.Name, 26)}" +
                   $"{PaddedPrint(PeriodActiveDays, 21)}" +
                   $"{PaddedPrint(ActiveDaysPerWeek, 23)}" +
                   $"{PaddedPrint(CommitsPerDay, 16)}" +
                   $"{PaddedPrint(LinesOfChangePerHour, 27)}" +
                   $"{PaddedPrint(Impact, 9)}" +
                   $"{PaddedPrint(RiskFactor, 14)}" +
                   $"{PaddedPrint(LinesAdded, 14)}" +
                   $"{PaddedPrint(LinesRemoved, 16)}" +
                   $"{PaddedPrint(Churn, 8)}" +
                   $"{PaddedPrint(Rtt100, 9)}" +
                   $"{PaddedPrint(Ptt100, 9)}" +
                   $"{PaddedPrint(Dtt100, 0)}";
        }

        private string PaddedPrint(object value, int fieldWidth)
        {
            return value.ToString().PadRight(fieldWidth,  ' ');
        }
    }
}
