using System;
using System.Collections.Generic;

namespace Analyzer.Domain
{
    public class Alias
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public List<string> Emails { get; set; }

        public Alias()
        {
            Id = Guid.NewGuid();
        }
    }
}