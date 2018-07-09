using System;

namespace Analyzer.Domain
{
    public class ReportingPeriod
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int Total_Days()
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
            var numberOfWeekends = Total_Weeks();
            var weekendDays = numberOfWeekends * 2;
            var workingDays = Total_Days() - weekendDays;
            return workingDays;
        }

        private int Total_Weeks()
        {
            var totalDays = Total_Days();
            var numberOfWeekends = totalDays / 7;
            return numberOfWeekends;
        }
    }
}