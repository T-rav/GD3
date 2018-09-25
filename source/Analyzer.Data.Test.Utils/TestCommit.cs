using System.Collections.Generic;

namespace Analyzer.Data.Test.Utils
{
    public class TestCommit
    {
        public string FileName { get; set; }
        public List<string> Lines { get; set; }
        public string TimeStamp { get; set; }
    }
}