using System;
using System.Collections.Generic;

namespace Analyzer.Domain
{
    public interface ISourceControlRepository
    {
        IEnumerable<Author> List_Authors();
        int Period_Active_Days(Author author);
        double Active_Days_Per_Week(Author author);
        double Commits_Per_Day(Author author);
        List<DeveloperStats> Build_Individual_Developer_Stats(IEnumerable<Author> authors);
    }
}