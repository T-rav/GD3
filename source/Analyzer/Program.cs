using Analyzer.Data.SourceControl;
using CommandLine;
using System;

namespace Analyzer
{
    /*
     * todo
     * 1) get technical debt per file and see what % author is resonsible for
     *   1a) then narrow into hotspots? - not sure this is applicable. 
     * 2) check to see if file moves = deletes
     * 3) find out how to catalog the '10x' factor (pairing)
     *   3a) Tusani and Siphenathi pair quite a bit. Sindi recieved a lot of help.
     * 4) make churn personal? - Not sure this encurrages collective code ownership
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
            using (var repo = new SourceControlRepositoryBuilder()
                .WithPath(opts.Path)
                .WithEntireHistory()
                .WithIgnorePatterns(opts.IgnorePatterns)
                .WithBranch(opts.Branch)
                .WithWeekends(opts.WeekendDays)
                .WithWorkingDaysPerWeek(opts.WorkingDaysPerWeek)
                .WithWorkingWeekHours(opts.WorkingHoursPerWeek)
                .WithIgnoreComments(opts.IgnoreComments)
                .Build())
            {

                // todo : make configurable
                //var aliasMap = new List<Alias>
                //{
                //    new Alias{
                //        Name = "T-rav",
                //        Emails = new List<string>{
                //            "tmfrisinger@gmail.com",
                //            "tmfirsinger@gmail.com",
                //            "travisf@stoneage1.bizvoip.co.za"}
                //    }
                //};

                // todo : wip, first attempt to make aliases confiurable
                //var aliasRepository = new AliasRepository(opts.AliasFile);
                //var aliasMap = aliasRepository.Load();

                var dashboard = new CodeStatsDashboard();
                var authors = repo.List_Authors();
                var stats = repo.Build_Individual_Developer_Stats(authors);
                var teamStats = repo.Build_Team_Stats();
                dashboard.RenderDashboard(stats, teamStats, repo.ReportingRange);
            }

            return 1;
        }

        private static int DisplayRangedHistory(RangedHistory opts)
        {
            using (var repo = new SourceControlRepositoryBuilder()
                .WithPath(opts.Path)
                .WithRange(opts.StartDate, opts.EndDate)
                .WithIgnorePatterns(opts.IgnorePatterns)
                .WithBranch(opts.Branch)
                .WithWeekends(opts.WeekendDays)
                .WithWorkingDaysPerWeek(opts.WorkingDaysPerWeek)
                .WithWorkingWeekHours(opts.WorkingHoursPerWeek)
                .WithIgnoreComments(opts.IgnoreComments)
                .Build())
            {

                var dashboard = new CodeStatsDashboard();
                var authors = repo.List_Authors();
                var stats = repo.Build_Individual_Developer_Stats(authors);
                var teamStats = repo.Build_Team_Stats();
                dashboard.RenderDashboard(stats, teamStats, repo.ReportingRange);

                return 1;
            }
        }
    }
}
