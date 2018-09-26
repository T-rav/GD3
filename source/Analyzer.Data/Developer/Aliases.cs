using Analyzer.Domain.Developer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analyzer.Data.Developer
{
    public class Aliases : IAliases
    {
        private readonly string _aliasFilePath;

        public Aliases(string aliasFilePath)
        {
            _aliasFilePath = aliasFilePath;
        }

        public IEnumerable<Author> Map_To_Authors(IEnumerable<Author> authors)
        {
            var aliases = Load();

            return NoAliases(aliases) ? authors : MapDevelopersToAliases(aliases, authors);
        }

        private IEnumerable<Alias> Load()
        {
            if (string.IsNullOrWhiteSpace(_aliasFilePath))
            {
                return new List<Alias>();
            }

            return JsonConvert.DeserializeObject<List<Alias>>(File.ReadAllText(_aliasFilePath));
        }

        private bool NoAliases(IEnumerable<Alias> aliases)
        {
            return aliases == null;
        }

        private IEnumerable<Author> MapDevelopersToAliases(IEnumerable<Alias> aliases, IEnumerable<Author> authors)
        {
            var authorMap = new Dictionary<Guid, Author>();

            foreach (var author in authors)
            {
                var alias = FindAlias(aliases, author);
                if (AliasFoundWithNoDeveloperMapping(alias, authorMap))
                {
                    AddMapping(author, alias, authorMap);
                }
                else if (NoAliasFoundWithNoDeveloperMapping(alias, authorMap))
                {
                    AddDefaultMapping(authorMap, author);
                }
            }

            var result = authorMap.Values.ToList();
            return result;
        }

        private void AddDefaultMapping(Dictionary<Guid, Author> authorMap, Author author)
        {
            authorMap[Guid.NewGuid()] = author;
        }

        private void AddMapping(Author author, Alias alias, Dictionary<Guid, Author> authorMap)
        {
            author.Emails.Clear();
            author.Emails.AddRange(alias.Emails);
            authorMap[alias.Id] = author;
        }

        private bool NoAliasFoundWithNoDeveloperMapping(Alias alias, Dictionary<Guid, Author> authorMap)
        {
            return alias == null || !authorMap.ContainsKey(alias.Id);
        }

        private bool AliasFoundWithNoDeveloperMapping(Alias alias, Dictionary<Guid, Author> authorMap)
        {
            return alias != null && !authorMap.ContainsKey(alias.Id);
        }

        private Alias FindAlias(IEnumerable<Alias> aliases, Author author)
        {
            var alias = aliases.FirstOrDefault(x => x.Emails.Contains(author.Emails.FirstOrDefault()));
            return alias;
        }
    }
}