using System.Collections.Generic;
using Analyzer.Tests;

namespace Analyzer.Domain
{
    public interface ISourceControlRepository
    {
        IEnumerable<object> List_Authors();
        int Period_Active_Days(Author author);
        double Active_Days_Per_Week(Author author);
        double Commits_Per_Day(Author author);
    }
}