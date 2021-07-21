﻿using SPM1.Comps;
using UnityEngine;
using Verse;

namespace SPM1
{
    public static class PersonalityUI
    {
        public const float HEIGHT = 120;

        public static void DrawPersonalityBio(Pawn pawn, CompEnneagram comp, Rect rect, PersonalityVisibility visibility)
        {
            if (pawn == null || comp == null || visibility == PersonalityVisibility.Invisible)
                return;

            var listing = comp.Listing ??= new Listing_Standard();

            bool oldEnabled = GUI.enabled;
            GUI.enabled = true;
            listing.Begin(rect);

            listing.Label($"<b>{"SP.PersonalityHeader".Translate(pawn.LabelShortCap)}</b>");
            listing.GapLine();

            bool isAnimal = pawn.RaceProps?.Animal ?? false;

            string trs = isAnimal ? "SP.HiddenAnimalPersonality".Translate() : "SP.HiddenPersonality".Translate();
            string msg = $"<i>{trs}</i>";
            float maxHeight = HEIGHT - listing.CurHeight;

            listing.Label(visibility == PersonalityVisibility.Hidden ? msg : comp.GetDescription(), maxHeight);

            listing.End();
            GUI.enabled = oldEnabled;
        }
    }
}