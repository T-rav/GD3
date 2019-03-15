using System;
using Analyzer.Domain.Reporting;

namespace Analyzer.Domain.SourceControl
{
    public interface ISourceControlAnalysis : IDisposable
    {
        CodeAnalysis Run_Analysis();
    }
}