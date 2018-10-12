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

        public IList<Author> Map_To_Authors(IList<Author> authors)
        {
            var aliases = Load();

            EnsureEmailAddressesAreNotSharedByAliases(aliases);

            return NoAliases(aliases) ? authors : MapDevelopersToAliases(aliases, authors);
        }

        private static void EnsureEmailAddressesAreNotSharedByAliases(List<Alias> aliases)
        {
            var allEmailAddresses = aliases.SelectMany(alias => alias.Emails.Distinct());
            var distinctEmailAddresses = aliases.SelectMany(alias => alias.Emails).Distinct();
            if (allEmailAddresses.Count() != distinctEmailAddresses.Count())
            {
                // TODO: throw better exception?
                throw new Exception("Aliases can't share an email address.");
            }
        }

        private List<Alias> Load()
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

        private IList<Author> MapDevelopersToAliases(IList<Alias> aliases, IList<Author> authors)
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
            var emails = author.Emails.Union(alias.Emails).ToList();
            author.Emails.Clear();
            author.Emails.AddRange(emails);
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
            return aliases.FirstOrDefault(x => x.Emails.Intersect(author.Emails).Any());
        }
    }
}