using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
    static class Patch_CharacterCardUtility_DrawCharacterCard
    {
        static void Postfix(Rect rect, Pawn pawn)
        {
            if (Settings.UseWorldviewTab)
                return;

            var comp = pawn?.TryGetEnneagramComp();
            if (comp?.Enneagram == null || !comp.Enneagram.IsValid)
                return;

            var vis = PersonalityUtils.GetPersonalityVisibility(pawn);
            PersonalityUI.DrawPersonalityBio(pawn, comp, new Rect(rect.x, rect.yMax - Settings.ExtraBioHeight, rect.width, rect.height), vis);
        }
    }
}
