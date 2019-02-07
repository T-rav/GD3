using CommandLine;
using System;
using System.Collections.Generic;

namespace Analyzer
{
    [Verb("full", Hidden = false, HelpText = "Git repository settings for full history")]
    public class FullHistory
    {
        [Option('p', "Location of the git repository", Default = "", HelpText = "Path on the filesystem to the repository")]
        public string Path { get; set; }

        [Option('b', "Branch to examine", Default = "master", HelpText = "Which branch to build stats for. Defaults to master")]
        public string Branch { get; set; }

        [Option('i', "Patterns to ignore when building stats", Default = null, Separator = ';', HelpText = "Patterns to ignore when building stats")]
        public IEnumerable<string> IgnorePatterns { get; set; }

        [Option('w', "Weekend days", Default = new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }, Separator = ';', HelpText = "Weekend days")]
        public IEnumerable<DayOfWeek> WeekendDays { get; set; }

        [Option('h', "Working hours per week", Default = 40, HelpText = "Number of working hours per week")]
        public int WorkingHoursPerWeek { get; set; }

        [Option('d', "Working days per week", Default = 5, HelpText = "Number of working days per week")]
        public double WorkingDaysPerWeek { get; set; }

        [Option('a', "Location of the JSON file containing the list of aliases", Default = "", HelpText = "Path on the filesystem to the JSON file containing the list of aliases")]
        public string AliasFile { get; set; }

        [Option('c', "Ignore comments in commit history", Default = false, HelpText = "When set to true ignores single line comments")]
        public bool IgnoreComments { get; set; }

        [Option('m', "When set writes out the data model as json for the angular ui to consume", Default = DisplayModes.Console)]
        public DisplayModes Mode { get; set; }
    }
}