using System;
using System.Collections.Generic;
using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.Team;

namespace Analyzer.Domain.SourceControl
{
    public interface ISourceControlAnalysis : IDisposable
    {
        ReportingPeriod ReportingRange { get; }

        IList<Author> List_Authors();
        int Period_Active_Days(Author author);
        double Active_Days_Per_Week(Author author);
        double Commits_Per_Day(Author author);
        IList<DeveloperStats> Build_Individual_Developer_Stats(IList<Author> authors);
        TeamStatsCollection Build_Team_Stats();
        IList<DailyDeveloperStats> Build_Daily_Individual_Developer_Stats(List<Author> authors);
    }
}