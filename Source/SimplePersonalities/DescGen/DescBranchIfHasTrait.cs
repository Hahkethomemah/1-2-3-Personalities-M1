using System.Collections.Generic;
using RimWorld;
using Verse;

namespace SPM1
{
    public class DescBranchIfHasTrait : DescBranch
    {
        public List<TraitDef> traits = new List<TraitDef>();

        public override bool Matches(Pawn pawn)
        {
            if (pawn == null)
                return false;

            foreach (var trait in traits)
            {
                if (trait == null)
                    continue;
                if (pawn.HasTrait(trait))
                    return true;
            }
            return false;
        }
    }
}
