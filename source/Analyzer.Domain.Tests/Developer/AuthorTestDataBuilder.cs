using System.Collections.Generic;
using Analyzer.Domain.Developer;

namespace Analyzer.Data.Tests.Developer
{
    public class AuthorTestDataBuilder
    {
        private readonly List<Author> _aliases = new List<Author>();

        public static AuthorTestDataBuilder Create()
        {
            return new AuthorTestDataBuilder();
        }

        public AuthorTestDataBuilder WithAuthor(string name, params string[] emails)
        {
            _aliases.Add(new Author()
            {
                Name = name,
                Emails = new List<string>(emails)
            });

            return this;
        }

        public List<Author> Build()
        {
            return _aliases;
        }
    }
}