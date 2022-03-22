using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(CharacterCardUtility), "PawnCardSize")]
    static class Patch_CharacterCardUtility_PawnCardSize
    {
        static void Postfix(ref Vector2 __result, Pawn pawn)
        {
            if (Settings.UseWorldviewTab)
                return;

            var enneagram = pawn?.TryGetEnneagram();
            if (enneagram == null || !enneagram.IsValid || PersonalityUtils.GetPersonalityVisibility(pawn) == PersonalityVisibility.Invisible)
                return;

            __result += new Vector2(0f, Settings.ExtraBioHeight);
        }
    }
}
