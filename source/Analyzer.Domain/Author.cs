using System.Collections.Generic;

namespace Analyzer.Domain
{
    public class Author
    {
        public string Name { get; set; }
        public List<string> Emails { get; set; }

        public Author()
        {
            Emails = new List<string>();
        }
    }
}