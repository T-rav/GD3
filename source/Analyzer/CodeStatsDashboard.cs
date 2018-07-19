using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.SourceRepository;
using Analyzer.Domain.Team;

namespace Analyzer
{
    public class CodeStatsDashboard
    {
        public static string Version => "0.9.3";
        public static ConsoleColor DefaultColor { get; private set; }

        public CodeStatsDashboard()
        {
            DefaultColor = Console.ForegroundColor;
        }

        public void RenderDashboard(ISourceControlRepository repo)
        {
            var authors = repo.List_Authors();
            var stats = repo.Build_Individual_Developer_Stats(authors);
            var teamStats = repo.Build_Team_Stats();

            PrintApplicationHeader(repo.ReportingRange.Start, repo.ReportingRange.End);
            PrintDeveloperStatsTableHeader();
            PrintDeveloperStatsTable(stats);
            PrintDeveloperAverages(stats, repo.ReportingRange);
            PrintTeamStatsTableHeader();
            PrintTeamStatsTable(teamStats);
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
            Console.WriteLine("Developer               | Period Active Days | Active Days Per Week | Commits / Day | Lines of Change Per Hour | Impact | Risk Factor | Lines Added | Lines Removed | Churn | Rtt100 | Ptt100 | Dtt100");
            PrintDashedLine();
        }

        private void PrintDeveloperAverages(List<DeveloperStats> stats, ReportingPeriod reportingPeriod)
        {
            var totalDevelopers = stats.Count * 1.0;

            var periodActiveDays = Math.Round(stats.Sum(x => x.PeriodActiveDays) / totalDevelopers, 2);
            var activeDays = Math.Round(stats.Sum(x => x.ActiveDaysPerWeek) / totalDevelopers, 2);
            var commitsPerDay = Math.Round(stats.Sum(x => x.CommitsPerDay) / totalDevelopers, 2);
            var linesOfChange = Math.Round(stats.Sum(x => x.LinesOfChangePerHour) / totalDevelopers, 2);
            var impact = Math.Round(stats.Sum(x => x.Impact) / totalDevelopers, 2);
            var riskFactor = Math.Round(stats.Sum(x => x.RiskFactor) / totalDevelopers, 2);
            var linesAdded = Math.Round(stats.Sum(x => x.LinesAdded) / totalDevelopers, 2);
            var linesRemoved = Math.Round(stats.Sum(x => x.LinesRemoved) / totalDevelopers, 2);
            var churn = Math.Round(stats.Sum(x => x.Churn) / totalDevelopers, 2);
            var rtt100 = Math.Round(stats.Sum(x => x.Rtt100) / totalDevelopers, 2);
            var ptt100 = Math.Round(stats.Sum(x => x.Ptt100) / totalDevelopers, 2);
            var dtt100 = Math.Round(stats.Sum(x => x.Dtt100) / totalDevelopers, 2);

            var renderedLine = $"{PaddedPrint("Averages", 26)}" +
                               $"{PaddedPrint($"{periodActiveDays} of {reportingPeriod.Period_Working_Days()}", 21)}" +
                               $"{PaddedPrint($"{activeDays}*", 23)}" +
                               $"{PaddedPrint(commitsPerDay, 16)}" +
                               $"{PaddedPrint(linesOfChange, 27)}" +
                               $"{PaddedPrint($"{impact}^", 9)}" +
                               $"{PaddedPrint(riskFactor, 14)}" +
                               $"{PaddedPrint(linesAdded, 14)}" +
                               $"{PaddedPrint(linesRemoved, 16)}" +
                               $"{PaddedPrint(churn, 8)}" +
                               $"{PaddedPrint(rtt100, 9)}" +
                               $"{PaddedPrint(ptt100, 9)}" +
                               $"{PaddedPrint(dtt100, 0)}";

            Console.WriteLine(renderedLine);
            PrintDashedLine();
            var repoAvg = CalculateRepositoryAverageWorkingDaysPerWeek(reportingPeriod);
            Console.WriteLine($"* Expected average is {repoAvg} days. This is based on global average of 3.2 days per 5 workings days or 5.12 hours per 8 working hours.");
            Console.WriteLine("^ Approximation of congative load carried when contributing");
            PrintDashedLine();
        }

        private static double CalculateRepositoryAverageWorkingDaysPerWeek(ReportingPeriod reportingPeriod)
        {
            var avgHoursPerDay = 5.12;
            var repoAvg = (reportingPeriod.DaysPerWeek * avgHoursPerDay) / reportingPeriod.Period_Weeks();
            return repoAvg;
        }

        private void PrintDeveloperStatsTable(List<DeveloperStats> stats)
        {
            var orderedStats = stats.OrderByDescending(x => x.PeriodActiveDays);
            foreach (var stat in orderedStats)
            {
                Console.WriteLine(stat);
            }
            PrintDashedLine();
        }

        private void PrintTeamStatsTableHeader()
        {
            PrintDashedLine();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Team Stats");
            Console.ForegroundColor = DefaultColor;
            PrintDashedLine();
            Console.WriteLine("Date           | Total Commits | Active Developers | Velocity");
            PrintDashedLine();
        }

        private void PrintTeamStatsTable(List<TeamStats> teamStats)
        {
            var orderedStats = teamStats.OrderBy(x => x.DateOf);
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

        private string PaddedPrint(object value, int fieldWidth)
        {
            return value.ToString().PadRight(fieldWidth, ' ');
        }
    }
}
