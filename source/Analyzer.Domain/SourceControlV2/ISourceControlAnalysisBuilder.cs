using System;
using System.Collections.Generic;

namespace Analyzer.Domain.SourceControlV2
{
    public interface ISourceControlAnalysisBuilder
    {
        ISourceControlAnalysis Build();
        ISourceControlAnalysisBuilder WithAliasMapping(string aliasMapping);
        ISourceControlAnalysisBuilder WithBranch(string branch);
        ISourceControlAnalysisBuilder WithEntireHistory();
        ISourceControlAnalysisBuilder WithIgnoreComments(bool ignoreComments);
        ISourceControlAnalysisBuilder WithIgnorePatterns(IEnumerable<string> patterns);
        ISourceControlAnalysisBuilder WithPath(string repoPath);
        ISourceControlAnalysisBuilder WithRange(DateTime start, DateTime end);
        ISourceControlAnalysisBuilder WithWeekends(IEnumerable<DayOfWeek> days);
        ISourceControlAnalysisBuilder WithWorkingDaysPerWeek(double workingDaysPerWeek);
        ISourceControlAnalysisBuilder WithWorkingWeekHours(int workWeekHours);
    }
}