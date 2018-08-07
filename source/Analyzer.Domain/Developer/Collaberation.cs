using System;
using System.Collections.Generic;

namespace Analyzer.Domain.Developer
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