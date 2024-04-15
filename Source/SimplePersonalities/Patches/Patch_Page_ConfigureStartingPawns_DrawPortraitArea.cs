using HarmonyLib;
using RimWorld;
using SPM1.UI;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(StartingPawnUtility), "DrawPortraitArea")]
    static class Patch_Page_ConfigureStartingPawns_DrawPortraitArea
    {
        static void Postfix(int pawnIndex, Rect rect)
        {
            Pawn pawn = StartingPawnUtility.StartingAndOptionalPawns[pawnIndex];
            var comp = pawn?.TryGetEnneagramComp();
            if (comp == null)
                return;

            //rect = rect.ContractedBy(17f);

            GUI.BeginGroup(rect);

            Rect rect6 = new Rect(rect.width - 24f - 320f, 0f, 120f, 30f);
            if (Widgets.ButtonText(rect6, "SP.EditPersonality".Translate(), true, true, true))
            {
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
                Find.WindowStack.Add(new Dialog_PersonalityEditor(){ Pawn = pawn, ApplyOnClose = true });
            }
            TooltipHandler.TipRegion(rect6, new TipSignal(comp.GetDescription()));
            UIHighlighter.HighlightOpportunity(rect6, "SP.EditPersonality");

            GUI.EndGroup();
        }
    }
}
