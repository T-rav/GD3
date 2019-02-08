using System;
using System.Collections.Generic;

namespace Analyzer.Domain.Developer
{
    public class DailyDeveloperStats
    {
        public DateTime Date { get; set; }
        public List<DeveloperStats> Stats { get; set; }
    }
}