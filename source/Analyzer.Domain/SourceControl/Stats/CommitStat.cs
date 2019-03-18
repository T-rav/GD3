using System;
using System.Collections.Generic;
using System.Linq;
using Analyzer.Domain.Developer;

namespace Analyzer.Domain.SourceControl.Stats
{
    public class CommitStat
    {
        public Author Author => _commit.Author;
        public DateTime When => _commit.When;
        public int LinesAdded => _commit.Patch.Sum(x=>x.LinesAdded);
        public int LinesRemoved => _commit.Patch.Sum(x=>x.LinesRemoved);
        
        private readonly Commit _commit;

        public CommitStat(Commit commit)
        {
            _commit = commit;
        }

        // todo : take in ignore list and ignoreCommentedOutline
        public double Churn()
        {
            var result = Math.Round((double)LinesRemoved / LinesAdded, 2);
            if (result.Equals(double.NaN) || result.Equals(double.PositiveInfinity))
            {
                return 0.0;
            }

            return result;
        }

        public double Impact(List<string> ignorePatterns, bool ignoreCommentedOutLines)
        {
            var surfaceAreaFactor = 1000.0;

            var excludeTypes = new List<ChangeType>
            {
                ChangeType.Ignored, ChangeType.Unreadable, ChangeType.Untracked, ChangeType.Conflicted,
                ChangeType.Renamed, ChangeType.Renamed
            };

            var patches = _commit.Patch.Where(x => !excludeTypes.Contains(x.ChangeType)).ToList();

            var commentedOutlinesToRemove = Total_Commented_Out_Lines_To_Deduct(patches, ignoreCommentedOutLines);
            var totalEditLines = patches.Sum(x => x.LinesAdded + x.LinesRemoved);
            var totalFiles = patches.Count();
            var totalEditLocations = Total_Edit_Locations(patches);
            var totalLinesOfOldCode = Total_Lines_Of_Old_Code();
            var totalLines = totalEditLines - commentedOutlinesToRemove;

            var percentageOldCode = 1.0;
            var oldMultiplier = 1.5;

            if (totalLinesOfOldCode > 0)
            {
                var percentageOldEdit = ((double)totalLines / totalLinesOfOldCode);
                percentageOldCode = oldMultiplier * percentageOldEdit;
            }
            
            // 1 line in new code = 0.0010 unit
            // 1 line in old code = 0.0015 unit
            var surfaceArea = totalLines / surfaceAreaFactor;
            var rawImpact = ((double)totalEditLocations * totalFiles * surfaceArea);
            var impact = (rawImpact * percentageOldCode);

            if (impact.Equals(double.NaN))
            {
                return 0.0;
            }

            return Math.Round(impact,4);

        }

        // todo : filter this better, I can commit now and amend 1 second later and it is old code. I need a days old filter to better eliminate the false positives happening
        // with small commits!
        private int Total_Lines_Of_Old_Code()
        {
            var result = 0;
            foreach (var patch in _commit.Patch)
            {
                if (patch.ChangeType == ChangeType.Modified)
                {
                   result += patch.LinesAdded + patch.LinesRemoved;
                }
            }

            return result;
        }

        private int Total_Edit_Locations(List<Patch> patches)
        {
            var result = 0;
            foreach (var patch in patches)
            {
                result += (patch.Contents.Split("@@").Length - 1)/2;
            }

            return result;
        }


        private int Total_Commented_Out_Lines_To_Deduct(List<Patch> patches, bool ignoreComments)
        {
            if (!ignoreComments)
            {
                return 0;
            }

            var result = 0;
            foreach (var patch in patches)
            {
                result += patch.Contents.Split("+//").Length - 1;
            }

            return result;
        }

        //private bool FileShouldBeIgnored(PatchEntryChanges file)
        //{
        //    return _context.IgnorePatterns.Any(pattern => file.Path.Contains(pattern));
        //}


    }
}