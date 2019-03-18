namespace Analyzer.Domain.SourceControl
{
    public class Patch
    {
        public string Contents { get; set; }
        public int LinesAdded { get; set; }
        public int LinesRemoved { get; set; }
        public ChangeType ChangeType { get; set; }
    }
}