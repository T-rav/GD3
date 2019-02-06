using System;
using System.Collections.Generic;

namespace Analyzer.Data.Test.Utils
{
    public class TestCommit
    {
        public string FileName { get; set; }
        public List<string> Lines { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CommitMessage { get; set; }
    }
}