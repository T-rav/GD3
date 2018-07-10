using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Domain
{
    public class DeveloperStats
    {
        public Author Author { get; set; }
        public int Rank { get; set; } // a score on Effieiceny, Impact and Bandwidth
        public int PeriodActiveDays { get; set; }
        public double ActiveDaysPerWeek { get; set; } // todo : make this / sprint and allow user to set sprint size (default to week)
        public double CommitsPerDay { get; set; } // person velocity (Bandwidth)
        public double Efficiency { get; set; } // how much re-work to they produce
        public double Impact { get; set; } // congative load
        public double Ptt100 { get; set; } // how quick are they to add value 
        public double Sptt100 { get; set; } // how much gap between raw and production
        
        // todo: would still like to know what % of recent technical debt they contributed (based on period of reporting)

        public override string ToString()
        {
            // 25
            return $"{PaddedPrint(Author.Name,26)}" +
                   $"{PaddedPrint(PeriodActiveDays,21)}" +
                   $"{PaddedPrint(ActiveDaysPerWeek,23)}" +
                   $"{PaddedPrint(CommitsPerDay,16)}" +
                   $"{PaddedPrint(Efficiency,13)}" +
                   $"{PaddedPrint(Impact,9)}" +
                   $"{PaddedPrint(Ptt100,9)}" +
                   $"{PaddedPrint(Sptt100,0)}";
        }

        private string PaddedPrint(object value, int fieldWidth)
        {
            return value.ToString().PadRight(fieldWidth,  ' ');
        }
    }
}
