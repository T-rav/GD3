using System;

namespace Analyzer.Domain
{
    public class ReportingPeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int Days()
        {
            var result = End.Subtract(Start).Days+1;
            if (result <= 0)
            {
                return 0;
            }

            return result;
        }

        public int Working_Days()
        {
            var numberOfWeekends = (int)Math.Floor(Weeks());
            var weekendDays = numberOfWeekends * 2;
            var workingDays = Days() - weekendDays;
            return workingDays;
        }

        public double Weeks()
        {
            var totalDays = Days();
            var numberOfWeekends = totalDays / 7.0;
            return numberOfWeekends;
        }
    }
}