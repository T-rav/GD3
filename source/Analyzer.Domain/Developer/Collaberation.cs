using System;
using System.Collections.Generic;
using Analyzer.Domain.Developer;

namespace Analyzer.Data.SourceRepository
{
    public class Collaberation
    {
        public List<Author> Collaberators { get; }
        public DateTime DateOf { get; set; }

        public Collaberation()
        {
            Collaberators = new List<Author>();
        }

        public void AddCollaberator(Author author)
        {
            Collaberators.Add(author);
        }
    }
}