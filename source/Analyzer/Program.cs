using System;
using System.Linq;
using Analyzer.Data;
using Analyzer.Domain;

namespace Analyzer
{
    /*
     * todo
     * 1) get technical debt per file and see what % author is resonsible for
     *   1a) then narrow into hotspots? - not sure this is applicable. 
     * 2) check to see if file moves = deletes
     * 3) find out how to catalog the '10x' factor (pairing)
     *   3a) Tusani and Siphenathi pair quite a bit. Sindi recieved a lot of help.
     * 4) make churn personal?
     */
    class Program
    {
        static void Main(string[] args)
        {
            var defaultColor = Console.ForegroundColor;
            var version = "0.9.0";
            if (args.Length < 1)
            {
                PrintNoGitRepositoryPathError(defaultColor);
                return;
            }
            
            var startDate = DateTime.Parse("2018-07-10");
            var endDate = DateTime.Now.Date;
            var repo = new SourceControlRepositoryBuilder()
                            .WithPath(args[0])
                            .WithRange(startDate, endDate)
                            .WithBranch("origin/Monique")
                            .WithWorkingWeekHours(32)
                            .WithWorkingDaysPerWeek(4)
                            .Build();

            PrintApplicationHeader(version, startDate, endDate, defaultColor);

            PrintDeveloperStatsTableHeader(defaultColor);
            PrintDeveloperStatsTable(repo);
            PrintDeveloperAverages();

            // todo : notes?
            // wait to exit
            Console.ReadKey();
        }

        private static void PrintDeveloperAverages()
        {
            Console.Write("Averages");
            Console.WriteLine("");
            PrintDashedLine();
        }

        private static void PrintDeveloperStatsTable(ISourceControlRepository repo)
        {
            var authors = repo.List_Authors();
            var stats = repo.Build_Individual_Developer_Stats(authors);
            var orderedStats = stats.OrderByDescending(x=>x.PeriodActiveDays);
            foreach (var stat in orderedStats)
            {
                Console.WriteLine(stat);
            }
            PrintDashedLine();
        }

        private static void PrintDeveloperStatsTableHeader(ConsoleColor defaultColor)
        {
            PrintDashedLine();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Individual Developer Stats");
            Console.ForegroundColor = defaultColor;
            PrintDashedLine();
            Console.WriteLine("Rank | Developer               | Period Active Days | Active Days Per Week | Commits / Day | Lines of Change Per Hour | Impact | Risk Factor | Lines Added | Lines Removed | Churn | Rtt100 | Ptt100 | Dtt100");
            PrintDashedLine();
        }

        private static void PrintDashedLine()
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        }

        private static void PrintNoGitRepositoryPathError(ConsoleColor defaultColor)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error : Need to enter a git repository path");
            Console.ForegroundColor = defaultColor;
            Console.ReadKey();
        }

        private static void PrintApplicationHeader(string version, DateTime start, DateTime end, ConsoleColor defaultColor)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"GD3 Stats - v{version}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"For period {start:yyyy-MM-dd} - {end:yyyy-MM-dd}");
            Console.ForegroundColor = defaultColor;
        }
    }
}
