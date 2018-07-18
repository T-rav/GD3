using System;
using System.Collections.Generic;

namespace Analyzer.Domain.Reporting
{
    public class ReportingPeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int HoursPerWeek { get; set; }
        public double DaysPerWeek { get; set; }

        private const double DaysInWeek = 7.0;

        public int Period_Days()
        {
            var result = End.Subtract(Start).Days + 1;
            if (result <= 0)
            {
                return 0;
            }

            return result;
        }

        public double Period_Working_Days()
        {
            var weeks = Period_Weeks();
            var result = DaysPerWeek * weeks;
            if (NoPartialWeek(weeks))
            {
                return Math.Round(result, 1);
            }

            return Math.Ceiling(result);
        }

        public double Period_Weeks()
        {
            var totalDays = End.Subtract(Start).Days + 1;
            var weeks = totalDays / DaysInWeek;
            if (weeks < 1)
            {
                weeks = 1;
            }
            return Math.Round(weeks,0);
        }

        public double Period_Working_Hours()
        {
            var weeks = Period_Weeks();
            return weeks * HoursPerWeek;
        }

        private static bool NoPartialWeek(double weeks)
        {
            return (int)weeks == weeks;
        }

        public List<DateTime> Generate_Dates_For_Range()
        {
            var result = new List<DateTime>();
            var seed = Start.Date;
            var totalDays = (End - Start).Days;
            for (var i = 0; i <= totalDays; i++)
            {
                result.Add(seed.AddDays(i));
            }
            return result;
        }
    }
}