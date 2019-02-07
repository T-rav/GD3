using CommandLine;
using System;
using Analyzer.Data.SourceControl;
using Analyzer.Domain.Stats;
using Analyzer.Presenter;
using Analyzer.UseCase;

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
                    (FullHistory opts) => Display_Full_History(opts),
                    (RangedHistory opts) => Display_Ranged_History(opts),
                     errors => -1);

            Console.ReadKey();
        }

        private static int Display_Full_History(FullHistory opts)
        {
            var presenter = Create_Presenter(opts.Mode);
            var builder = new SourceControlAnalysisBuilder();
            var statsUseCase = new FullStatsUseCase(builder);

            var inputTo = new FullStatsInput
            {
                Path = opts.Path,
                IgnorePatterns = opts.IgnorePatterns,
                Branch = opts.Branch,
                WeekDays = opts.WeekendDays,
                DaysPerWeek = opts.WorkingDaysPerWeek,
                HoursPerWeek = opts.WorkingHoursPerWeek,
                IgnoreComments = opts.IgnoreComments,
                AliasFile = opts.AliasFile,
                WeekendDays = opts.WeekendDays
            };

            statsUseCase.Execute(inputTo, presenter);
            presenter.Render();
            
            return 1;
        }

        private static IPresenter Create_Presenter(DisplayModes optsMode)
        {
            if (optsMode == DisplayModes.Web)
            {
                return new JsonPresenter();
            }

            var consolePresenter = new ConsolePresenter();

            return consolePresenter;
        }

        private static int Display_Ranged_History(RangedHistory opts)
        {
            var presenter = Create_Presenter(opts.Mode);
            var builder = new SourceControlAnalysisBuilder();
            var statsUseCase = new RangedStatsUseCase(builder);
            var inputTo = new RangedStatsInput
            {
                Path = opts.Path,
                IgnorePatterns = opts.IgnorePatterns,
                Branch = opts.Branch,
                WeekDays = opts.WeekendDays,
                DaysPerWeek = opts.WorkingDaysPerWeek,
                HoursPerWeek = opts.WorkingHoursPerWeek,
                IgnoreComments = opts.IgnoreComments,
                AliasFile = opts.AliasFile,
                RangeState = opts.StartDate,
                RangeEnd = opts.EndDate,
                WeekendDays = opts.WeekendDays
            };

            statsUseCase.Execute(inputTo, presenter);
            presenter.Render();

            return 1;
        }
    }
}
