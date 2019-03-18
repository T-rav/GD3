using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControl
{
    public class Commit
    {
        public Author Author { get; set; }
        public DateTime When { get; set; }
        public List<Patch> Patch { get; set; }

        public int LinesAdded
        {
            get { return Patch.Sum(x => x.LinesAdded); }
        }

        public int LinesRemoved
        {
            get { return Patch.Sum(x => x.LinesRemoved); }
        }
    }
}