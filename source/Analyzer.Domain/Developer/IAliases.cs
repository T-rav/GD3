using System.Collections.Generic;

namespace Analyzer.Domain.Developer
{
    public interface IAliases
    {
        IList<Author> Map_To_Authors(IList<Author> authors);
    }
}