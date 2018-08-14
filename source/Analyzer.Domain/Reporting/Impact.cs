using System;

namespace Analyzer.Domain.Reporting
{
    public class Impact
    {
        public int TotalFiles { get; set; }
        public int TotalEditLocations { get; set; }
        public int TotalLinesEdited { get; set; }
        public int TotalLinesOfOldCode { get; set; }

        private double SurfaceAreaFactor = 1000.0;

        public double Calculate()
        {
            var percentageOldCode = 1.0;
            var oldMultiplier = 1.5;
            if (TotalLinesOfOldCode > 0)
            {
                var percentageOldEdit = ((double)TotalLinesEdited / TotalLinesOfOldCode);
                percentageOldCode = oldMultiplier * percentageOldEdit;
            }

            // 1 line in new code = 0.0010 unit
            // 1 line in old code = 0.0015 unit
            var surfaceArea = TotalLinesEdited / SurfaceAreaFactor;
            var rawImpact = ((double)TotalEditLocations * TotalFiles * surfaceArea);
            var impact = (rawImpact * percentageOldCode);

            if (impact.Equals(Double.NaN))
            {
                return 0.0;
            }

            return Math.Round(impact,4);
        }
    }
}