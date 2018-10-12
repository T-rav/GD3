namespace Analyzer.Domain.SourceControl
{
    public class Branches
    {
        public static Branch Master => new Branch("master");

        public static bool MasterNotSelected(string branch)
        {
            return branch != Master.Value;
        }
    }
}
