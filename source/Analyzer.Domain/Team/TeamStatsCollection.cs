using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer.Domain.Team
{
    public class TeamStatsCollection
    {
        public List<TeamStats> Stats { get; }
        public List<DayOfWeek> Weekends { get; }

        public TeamStatsCollection(List<TeamStats> stats, List<DayOfWeek> weekends)
        {
            Stats = stats;
            Weekends = weekends;
        }

        public IEnumerable<TeamStats> GetWorkDayStats()
        {
            if (Weekends == null)
            {
                return Stats;
            }

            return Stats.Where(x=>!Weekends.Contains(x.DateOf.DayOfWeek));
        }
    }
}
