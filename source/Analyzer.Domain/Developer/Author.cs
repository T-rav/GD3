using System.Collections.Generic;
using System.Linq;

namespace Analyzer.Domain.Developer
{
    public class Author
    {
        public string Name { get; set; }
        public List<string> Emails { get; set; }

        public Author()
        {
            Emails = new List<string>();
        }

        public override string ToString()
        {
            if (Name.ToLower() == "unknown")
            {
                return Emails.First();
            }

            return Name;
        }
    }
}