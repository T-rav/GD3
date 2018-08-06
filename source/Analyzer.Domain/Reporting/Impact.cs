using System;

namespace Analyzer.Domain.Reporting
{
    public class Impact
    {
        public int TotalFiles { get; set; }
        public int TotalEditLocations { get; set; }
        public int TotalLinesEdited { get; set; }
        public int TotalLinesOfOldCode { get; set; }

        public double Calculate()
        {
            var percentageOldCode = 1.0;
            var oldMultiplier = 2;
            if (TotalLinesOfOldCode > 0)
            {
                var percentageOldEdit = ((double)TotalLinesEdited / TotalLinesOfOldCode);
                percentageOldCode = oldMultiplier * percentageOldEdit;
            }

            var rawImpact = ((double)TotalEditLocations * TotalFiles / TotalLinesEdited);
            var impact = rawImpact * percentageOldCode;

            if (impact.Equals(Double.NaN))
            {
                return 0.0;
            }

            return impact;
        }
    }
}