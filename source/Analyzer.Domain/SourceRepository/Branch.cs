namespace Analyzer.Domain.SourceRepository
{
    public class Branch
    {
        public string Value { get; }

        internal Branch(string branchName)
        {
            Value = branchName;
        }
    }
}