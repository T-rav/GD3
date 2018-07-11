using System;

namespace Analyzer.Domain
{
    public class DeveloperStats
    {
        public Author Author { get; set; }
        public int Rank { get; set; } // a score on Effieiceny, Impact and Bandwidth
        public int PeriodActiveDays { get; set; }
        public double ActiveDaysPerWeek { get; set; } // todo : make this / sprint and allow user to set sprint size (default to week)
        public double CommitsPerDay { get; set; } // do they push often
        public double Impact { get; set; } // congative load
        public double LinesOfChangePerHour { get; set; } // how much of a wake to they cause?
        public double RiskFactor => Math.Round( (LinesOfChangePerHour / CommitsPerDay),2 );// do they move quick, make a mess and cause high churn?! (LinesOfChangePerHour/CommitsPerDay)*Impact

        // todo : track which lines the developer changed over the period
        // todo: would still like to know what % of recent technical debt they contributed (based on period of reporting)

        public override string ToString()
        {
            return $"{PaddedPrint(Author.Name, 26)}" +
                   $"{PaddedPrint(PeriodActiveDays, 21)}" +
                   $"{PaddedPrint(ActiveDaysPerWeek, 23)}" +
                   $"{PaddedPrint(CommitsPerDay, 16)}" +
                   $"{PaddedPrint(LinesOfChangePerHour, 27)}" +
                   $"{PaddedPrint(Impact, 9)}" +
                   $"{PaddedPrint(RiskFactor,0)}";
        }

        private string PaddedPrint(object value, int fieldWidth)
        {
            return value.ToString().PadRight(fieldWidth,  ' ');
        }
    }
}
