using Analyzer.Domain.Developer;
using Analyzer.Domain.Reporting;
using Analyzer.Domain.Team;
using System;
using System.Collections.Generic;

namespace Analyzer.Domain.SourceRepository
{
    public interface ISourceControlRepository : IDisposable
    {
        ReportingPeriod ReportingRange { get; }

        IEnumerable<Author> List_Authors();
        //IEnumerable<Author> List_Authors(IEnumerable<Alias> aliases);
        int Period_Active_Days(Author author);
        double Active_Days_Per_Week(Author author);
        double Commits_Per_Day(Author author);
        List<DeveloperStats> Build_Individual_Developer_Stats(IEnumerable<Author> authors);
        TeamStatsCollection Build_Team_Stats();
    }
}