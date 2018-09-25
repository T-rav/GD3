using System;
using System.IO;

namespace Analyzer.Data.Test.Utils
{
    public class TestRepositoryContext : IDisposable
    {
        public string Path { get; set; }

        public void Dispose()
        {
            Directory.Delete(Path, true);
        }
    }
}