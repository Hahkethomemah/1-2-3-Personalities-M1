using System.Text;
using HarmonyLib;
using RimWorld;
using SPM1.Comps;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(MassUtility), "Capacity")]
    static class Patch_MassUtility_Capacity
    {
        static void Postfix(Pawn p, StringBuilder explanation, ref float __result)
        {
            var comps = p?.health?.hediffSet?.GetAllComps();
            if (comps == null)
                return;

            foreach (var comp in comps)
            {
                if (comp is CompCapacityFactor cap)
                {
                    float factor = (cap.props as CompProperties_CapacityFactor)?.factor ?? 1f;
                    __result *= factor;
                    if (explanation != null)
                    {
                        explanation.AppendLine();
                        explanation.Append(" - ");
                        explanation.Append(comp.Def.LabelCap);
                        explanation.Append(" : ");
                        if(factor > 1)
                            explanation.Append('+');
                        explanation.Append(((factor - 1f) * 100f).ToStringPercent());
                    }
                }
            }
        }
    }
}
