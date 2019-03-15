using System;
using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControl
{
    public class CommitStat
    {
        public Author Author { get; set; }
        public DateTime When { get; set; }
        public CommitPatch Patch { get; set; }
    }
}