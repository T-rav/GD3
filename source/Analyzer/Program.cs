﻿using System;
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
            
            var startDate = DateTime.Parse("2018-06-25");
            var endDate = DateTime.Now.Date;
            var repo = new SourceControlRepositoryBuilder()
                            .WithPath(args[0])
                            .WithRange(startDate, endDate)
                            //.WithBranch("origin/SindisiweK")
                            .WithWorkingWeekHours(32)
                            .WithWorkingDaysPerWeek(4)
                            .Build();

            var dashboard = new DeveloperStatsDashboard();
            dashboard.RenderDashboard(startDate, endDate, repo);

            Console.ReadKey();
        }

        private static void PrintNoGitRepositoryPathError()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error : Need to enter a git repository path");
            Console.ForegroundColor = DeveloperStatsDashboard.DefaultColor;
            Console.ReadKey();
        }
    }
}
