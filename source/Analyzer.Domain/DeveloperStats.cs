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
        
        // todo: would still like to know what % of recent technical debt they contributed (based on period of reporting)

        public override string ToString()
        {
            return null;
        }
    }
}
