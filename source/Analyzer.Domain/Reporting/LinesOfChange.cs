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
        public int TotalLines => Added + Removed;

        public double Churn
        {
            get
            {
                var result = Math.Round((double) Removed / Added, 2);
                if (result.Equals(Double.NaN) || result.Equals(Double.PositiveInfinity))
                {
                    return 0.0;
                }

                return result;
            }
        }
    }
}