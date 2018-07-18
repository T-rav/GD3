using System;
using System.Collections.Generic;

namespace Analyzer.Domain.Developer
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