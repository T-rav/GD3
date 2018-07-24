﻿using System;
using Analyzer.Data.SourceRepository;

namespace Analyzer
{
    /*
     * todo
     * 1) get technical debt per file and see what % author is resonsible for
     *   1a) then narrow into hotspots? - not sure this is applicable. 
     * 2) check to see if file moves = deletes
     * 3) find out how to catalog the '10x' factor (pairing)
     *   3a) Tusani and Siphenathi pair quite a bit. Sindi recieved a lot of help.
     * 4) make churn personal? - Not sure this encurabes collective code ownership
     */
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 1)
            {
                PrintNoGitRepositoryPathError();
                return;
            }

            var startDate = DateTime.Parse("2018-07-23");
            var endDate = DateTime.Parse("2018-07-23");
            var repo = new SourceControlRepositoryBuilder()
                            .WithPath(args[0])
                            .WithEntireHistory()
                            //.WithRange(startDate, endDate)
                            //.WithBranch("origin/SindisiweK")
                            .WithIgnorePattern("node_modules")
                            .WithIgnorePattern("packages")
                            .WithIgnorePattern(".gitignore")
                            .WithWeekend(DayOfWeek.Saturday)
                            .WithWeekend(DayOfWeek.Sunday)
                            .WithWorkingWeekHours(32)
                            .WithWorkingDaysPerWeek(4)
                            .Build();

            var dashboard = new CodeStatsDashboard();
            dashboard.RenderDashboard(repo);

            Console.ReadKey();
        }

        private static void PrintNoGitRepositoryPathError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error : Need to enter a git repository path");
            Console.ForegroundColor = CodeStatsDashboard.DefaultColor;
            Console.ReadKey();
        }
    }
}
