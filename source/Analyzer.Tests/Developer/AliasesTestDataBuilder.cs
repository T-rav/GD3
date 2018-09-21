using System.Collections.Generic;
using Analyzer.Domain.Developer;

namespace Analyzer.Data.Tests.Developer
{
    public class AliasesTestDataBuilder
    {
        private readonly List<Alias> _aliases = new List<Alias>();

        public static AliasesTestDataBuilder Create()
        {
            return new AliasesTestDataBuilder();
        }

        public AliasesTestDataBuilder WithAlias(string name, params string[] emails)
        {
            _aliases.Add(new Alias
            {
                Name = name,
                Emails = new List<string>(emails)
            });

            return this;
        }

        public List<Alias> Build()
        {
            return _aliases;
        }
    }
}