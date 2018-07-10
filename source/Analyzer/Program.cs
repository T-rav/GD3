using System;
using Analyzer.Data;

namespace Analyzer
{
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

            

            var startDate = DateTime.Parse("2018-06-25");
            var endDate = DateTime.Now.Date;
            var repo = new SourceControlRepositoryBuilder()
                            .WithPath(args[0])
                            .WithRange(startDate, endDate)
                            .Build();

            PrintApplicationHeader(version, startDate, endDate, defaultColor);

            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Individual Developer Stats");
            Console.ForegroundColor = defaultColor;
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Developer               | Period Active Days | Active Days Per Week | Commits / Day | Efficiency | Impact ");
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            //todo : developer list here
            var lineItems = repo.Build_Individual_Developer_Stats();
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            Console.Write("Avarages");
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------------------------------------------------------------------------");
            // todo : notes?


            

            // wait to exit
            Console.ReadKey();
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
