using RimWorld;
using Verse;

namespace SPM1
{
    [DefOf]
    public class SPDefOf
    {
        static SPDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SPDefOf));
        }

        public static RulePackDef SP_PersonalityRevealed;
        public static DescStack SP_HumanStack;
        public static DescStack SP_AnimalStack;
    }
}
