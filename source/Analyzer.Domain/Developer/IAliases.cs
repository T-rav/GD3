using System.Collections.Generic;

namespace Analyzer.Domain.Developer
{
    public interface IAliases
    {
        IEnumerable<Author> Map_To_Authors(IEnumerable<Author> authors);
    }
}