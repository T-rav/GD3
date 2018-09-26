using System;
using System.IO;

namespace Analyzer.Data.Test.Utils
{
    public class FileSystemTestArtefact : IDisposable
    {
        public string Path { get; set; }

        public void Dispose()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
                return;
            }

            Directory.Delete(Path, true);
        }
    }
}