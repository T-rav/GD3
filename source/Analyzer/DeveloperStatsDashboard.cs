using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Analyzer.Domain;

namespace Analyzer
{
    public class DeveloperStatsDashboard
    {
        public static string Version => "0.9.1";
        public static ConsoleColor DefaultColor { get; private set; }

        public DeveloperStatsDashboard()
        {
            DefaultColor = Console.ForegroundColor;
        }

        public void RenderDashboard(DateTime startDate, DateTime endDate, ISourceControlRepository repo)
        {

            PrintApplicationHeader(startDate, endDate);

            PrintDeveloperStatsTableHeader();
            PrintDeveloperStatsTable(repo);
            PrintDeveloperAverages();
        }

        private void PrintApplicationHeader(DateTime start, DateTime end)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"GD3 Stats - v{Version}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"For period {start:yyyy-MM-dd} - {end:yyyy-MM-dd}");
            Console.ForegroundColor = DefaultColor;
        }
        
        private void PrintDeveloperStatsTableHeader()
        {
            PrintDashedLine();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Individual Developer Stats");
            Console.ForegroundColor = DefaultColor;
            PrintDashedLine();
            Console.WriteLine("Rank | Developer               | Period Active Days | Active Days Per Week | Commits / Day | Lines of Change Per Hour | Impact | Risk Factor | Lines Added | Lines Removed | Churn | Rtt100 | Ptt100 | Dtt100");
            PrintDashedLine();
        }

        private void PrintDeveloperAverages()
        {
            Console.Write("Averages");
            Console.WriteLine("");
            PrintDashedLine();
        }

        private void PrintDeveloperStatsTable(ISourceControlRepository repo)
        {
            var authors = repo.List_Authors();
            var stats = repo.Build_Individual_Developer_Stats(authors);
            var orderedStats = stats.OrderByDescending(x => x.PeriodActiveDays);
            foreach (var stat in orderedStats)
            {
                Console.WriteLine(stat);
            }
            PrintDashedLine();
        }

        private void PrintDashedLine()
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        }
    }
}
