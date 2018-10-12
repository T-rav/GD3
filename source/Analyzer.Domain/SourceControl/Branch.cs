namespace Analyzer.Domain.SourceControl
{
    public class Branch
    {
        public string Value { get; }

        internal Branch(string branchName)
        {
            Value = branchName;
        }

        public static Branch Create(string branch)
        {
            return new Branch(branch);
        }
    }
}