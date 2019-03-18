using System;
using System.Collections.Generic;
using System.Text;

namespace Analyzer.Domain.SourceControl
{
    public enum ChangeType
    {
        Added,
        Conflicted,
        Copied,
        Deleted,
        Ignored,
        Modified,
        Renamed,
        TypeChanged,
        Unmodified,
        Unreadable,
        Untracked
    }
}
