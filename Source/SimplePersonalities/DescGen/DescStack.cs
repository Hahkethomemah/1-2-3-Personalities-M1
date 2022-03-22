using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SPM1
{
    public class DescStack : Def
    {
        public List<DescBranch> branches = new List<DescBranch>();

        public string MakeString(Pawn pawn, DescriptionSeed seed)
        {
            var branch = ResolveBranch(pawn);
            if (branch == null)
            {
                Core.Error($"Failed to find compatible description branch for pawn {pawn?.ToString() ?? "<null>"}");
                return null;
            }

            return branch.MakeString(pawn, seed)?.Trim();
        }

        public DescBranch ResolveBranch(Pawn pawn)
        {
            if (pawn == null)
                return null;

            return branches.FirstOrDefault(branch => branch.Matches(pawn));
        }
    }
}
