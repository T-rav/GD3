using System;
using Analyzer.Data.SourceRepository;
using CommandLine;

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
            Parser.Default.ParseArguments<FullHistory, RangedHistory>(args)
                .MapResult(
                    (FullHistory opts) => DisplayFullHistory(opts),
                    (RangedHistory opts) => DisplayRangedHistory(opts),
                     errors => -1);

            Console.ReadKey();
        }

        private static int DisplayFullHistory(FullHistory opts)
        {
            var repo = new SourceControlRepositoryBuilder()
                            .WithPath(opts.Path)
                            .WithEntireHistory()
                            .WithIgnorePatterns(opts.IgnorePatterns)
                            .WithBranch(opts.Branch)
                            .WithWeekends(opts.WeekendDays)
                            .WithWorkingDaysPerWeek(opts.WorkingDaysPerWeek)
                            .WithWorkingWeekHours(opts.WorkingHoursPerWeek)
                            .Build();

            var dashboard = new CodeStatsDashboard();
            dashboard.RenderDashboard(repo);

            return 1;
        }

        private static int DisplayRangedHistory(RangedHistory opts)
        {
            var repo = new SourceControlRepositoryBuilder()
                            .WithPath(opts.Path)
                            .WithRange(opts.StartDate, opts.EndDate)
                            .WithIgnorePatterns(opts.IgnorePatterns)
                            .WithBranch(opts.Branch)
                            .WithWeekends(opts.WeekendDays)
                            .WithWorkingDaysPerWeek(opts.WorkingDaysPerWeek)
                            .WithWorkingWeekHours(opts.WorkingHoursPerWeek)
                            .Build();

            var dashboard = new CodeStatsDashboard();
            dashboard.RenderDashboard(repo);
            return 1;
        }
    }
}
