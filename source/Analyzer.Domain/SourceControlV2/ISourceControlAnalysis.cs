using System;

namespace Analyzer.Domain.SourceControlV2
{
    public interface ISourceControlAnalysis : IDisposable
    {
        CodeAnalysis Run_Analysis();
    }
}