using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer.Domain.Reporting
{
    public class ReportingPeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int HoursPerWeek { get; set; }
        public double DaysPerWeek { get; set; }
        public List<DayOfWeek> Weekends { get; set; }

        private const double DaysInWeek = 7.0;
        private const double WorkDaysInWeek = 5.0;

        public ReportingPeriod()
        {
            Weekends = new List<DayOfWeek>();
        }

        public int Period_Days()
        {
            var result = End.Subtract(Start).Days + 1;
            if (result <= 0)
            {
                return 0;
            }

            return result;
        }

        public double Period_Working_Hours()
        {
            var workingDays = Math.Ceiling(Period_Working_Days_With_Rounding(2));
            var hoursPerDay = HoursPerWeek / DaysPerWeek;
            var result = workingDays * hoursPerDay;

            return result;
        }

        public double Period_Working_Days()
        {
            var result = Period_Working_Days_With_Rounding(0);
            if (result < DaysPerWeek)
            {
                return Math.Ceiling(result);
            }

            return result;
        }

        public double Period_Weeks()
        {
            return Period_Weeks_With_Rounding(0);
        }
        
        public List<DateTime> Generate_Dates_For_Range()
        {
            var result = new List<DateTime>();
            var seed = Start.Date;
            var totalDays = (End - Start).Days;
            for (var i = 0; i <= totalDays; i++)
            {
                var canidateDate = seed.AddDays(i);
                if (WorkDay(canidateDate))
                {
                    result.Add(canidateDate);
                }
            }
            return result;
        }

        private double Period_Weeks_With_Rounding(int decimals)
        {
            var totalDays = End.Subtract(Start).Days + 1;
            var weeks = totalDays / DaysInWeek;
            if (weeks < 1)
            {
                weeks = 1;
            }
            return Math.Round(weeks, decimals);
        }

        private double Period_Working_Days_With_Rounding(int deciamls)
        {
            var totalDays = End.Subtract(Start).Days + 1;
            var dates = Enumerable.Range(0, totalDays)
                .Select(i => Start.AddDays(i))
                .Where(d => !Weekends.Contains(d.DayOfWeek));

            var rawDays = dates.Count();
            var weeks = Period_Weeks_With_Rounding(deciamls);

            var daysToRemove = 0.0;
            if (totalDays >= DaysPerWeek)
            {
                daysToRemove = WorkDaysInWeek - DaysPerWeek;
            }

            return rawDays - (daysToRemove * weeks);
        }

        private bool WorkDay(DateTime canidateDate)
        {
            return !Weekends.Contains(canidateDate.DayOfWeek);
        }
    }
}