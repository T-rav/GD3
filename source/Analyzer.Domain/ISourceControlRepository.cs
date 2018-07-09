using System.Collections.Generic;
using Analyzer.Tests;

namespace Analyzer.Domain
{
    public interface ISourceControlRepository
    {
        IEnumerable<object> ListAuthors();
        int PeriodActiveDays(Author author);
    }
}