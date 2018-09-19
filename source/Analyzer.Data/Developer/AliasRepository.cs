using System.Collections.Generic;
using System.IO;
using Analyzer.Domain.Developer;
using Newtonsoft.Json;

namespace Analyzer.Data.Developer
{
    public class AliasRepository
    {
        private readonly string _repoPath;

        public AliasRepository(string repoPath)
        {
            _repoPath = repoPath;
        }

        public List<Alias> Load()
        {
            return JsonConvert.DeserializeObject<List<Alias>>(File.ReadAllText(_repoPath));
        }
    }
}