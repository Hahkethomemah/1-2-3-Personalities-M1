using HarmonyLib;
using SPM1.Comps;
using System;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(VerbProperties), "GetDamageFactorFor", typeof(Tool), typeof(Pawn), typeof(HediffComp_VerbGiver))]
    static class Patch_VerbProperties_GetDamageFactorFor
    {
        private static readonly Type hediffType = typeof(CompDamageFactor);

        static void Postfix(Pawn attacker, ref float __result)
        {
            var comp = attacker?.TryGetEnneagramComp();
            if (comp?.Enneagram == null || !comp.Enneagram.IsValid)
                return;

            var hediffs = attacker.health?.hediffSet?.hediffs;
            if (hediffs == null)
                return;

            foreach (var h in hediffs)
            {
                if (h.def.CompPropsFor(hediffType) is CompProperties_DamageFactor props)
                {
                    __result *= props.factor;
                }
            }
        }
    }
}
