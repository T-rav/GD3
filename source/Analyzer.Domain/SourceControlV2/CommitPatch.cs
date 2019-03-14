namespace Analyzer.Domain.SourceControlV2
{
    public class CommitPatch
    {
        public string Contents { get; set; }
        public int LinesAdded { get; set; }
        public int LinesRemoved { get; set; }
    }
}