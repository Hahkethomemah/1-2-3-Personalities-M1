using HarmonyLib;
using SPM1.Comps;
using Verse;
using Verse.AI;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(ThinkNode_ChancePerHour_Nuzzle), "MtbHours")]
    public class Patch_ThinkNode_ChancePerHour_Nuzzle_MtbHours
    {
        static void Postfix(Pawn pawn, ref float __result)
        {
            var comps = pawn.health?.hediffSet?.GetAllComps();
            if (comps == null)
                return;

            foreach (var item in comps)
            {
                if (item == null)
                    continue;

                if (item is CompNuzzleFactor nuzzleComp)
                {
                    var props = nuzzleComp.props as CompProperties_NuzzleFactor;
                    __result *= props.intervalFactor;
                }
            }
        }
    }
}
