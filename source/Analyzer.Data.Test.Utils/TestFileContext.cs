using System;
using System.IO;

namespace Analyzer.Data.Test.Utils
{
    public class TestFileContext : IDisposable
    {
        public string Path { get; set; }

        public void Dispose()
        {
            Directory.Delete(Path, true);
        }
    }
}