using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControlV2
{
    public class CodeAnalysis
    {
        public IList<Author> Authors { get; set; }
        public IList<CommitStat> CommitStats { get; set; }

        public IList<IndividualPeriodStats> Individual_Period_Stats()
        {
            var result = new List<IndividualPeriodStats>();
            foreach (var author in Authors)
            {
                var activeDays = CommitStats.Where(x => x.Author == author)
                    .Select(x => new
                    {
                        x.When
                    })
                    .GroupBy(x => x.When)
                    .Count();

                result.Add(new IndividualPeriodStats
                {
                    Author = author,
                    ActiveDays = activeDays
                });
            }

            return result;
        }
    }
}