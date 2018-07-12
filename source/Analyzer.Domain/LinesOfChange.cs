using System;

namespace Analyzer.Domain
{
    public class LinesOfChange
    {
        public int Added { get; set; }
        public int Removed { get; set; }
        public double ChangePerHour { get; set; }
        public double Rtt100 { get; set; }
        public double Ptt100 { get; set; }

        public double Churn => Math.Round((double)Removed / Added,2);
        public int TotalLines => Added + Removed;
    }
    
}