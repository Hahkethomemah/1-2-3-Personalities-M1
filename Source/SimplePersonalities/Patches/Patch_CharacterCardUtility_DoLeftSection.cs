using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(CharacterCardUtility), "DoLeftSection")]
    static class Patch_CharacterCardUtility_DoleftSection
    {
        static void Prefix(ref Rect leftRect, Pawn pawn)
        {
            if (Settings.UseWorldviewTab)
                return;

            var enneagram = pawn?.TryGetEnneagram();
            if (enneagram == null || !enneagram.IsValid || PersonalityUtils.GetPersonalityVisibility(pawn) == PersonalityVisibility.Invisible)
                return;

            leftRect.height -= Settings.ExtraBioHeight;
        }
    }
}
