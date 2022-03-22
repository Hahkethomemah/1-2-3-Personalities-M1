using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace SPM1.UI
{
    public class Dialog_SeedEditor : Window
    {
        public DescriptionSeed Seed;

        public override Vector2 InitialSize => new Vector2(350, 350);

        private Listing_Standard std = new Listing_Standard();
        private List<string> texts;
        private Vector2 scrollPos;
        private Rect scrollRect;

        public Dialog_SeedEditor(DescriptionSeed seed)
        {
            doCloseX = true;
            draggable = true;
            resizeable = false;
            layer = WindowLayer.Super;

            this.Seed = seed;
            texts = new List<string>();
            foreach (var s in seed.Seeds)
            {
                texts.Add(s.ToString());
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            DrawUI(std, texts, Seed, inRect, ref scrollPos, ref scrollRect);
        }

        private static void DrawUI(Listing_Standard std, List<string> texts, DescriptionSeed seed, Rect inRect, ref Vector2 scrollPos, ref Rect scrollRect)
        {
            std.Begin(inRect);

            std.Label($"<i>{"SP.SeedExplaination".Translate()}</i>");
            std.GapLine();

            if (std.ButtonText("SP.AddSeed".Translate()))
            {
                int s = Rand.Range(0, 10_000);
                texts.Add(s.ToString());
                seed.Seeds.Add(s);
            }

            GUI.enabled = seed.Seeds.Count > 0;
            if (std.ButtonText("SP.RemoveSeed".Translate()))
            {
                texts.RemoveAt(texts.Count - 1);
                seed.Seeds.RemoveAt(seed.Seeds.Count - 1);
            }
            GUI.enabled = true;
            std.GapLine();

            std.Label("SP.Seeds".Translate());

            // TODO replace with widgets scroll.
            //std.BeginScrollView(new Rect(inRect.x, inRect.y + std.CurHeight, inRect.width, inRect.height - std.CurHeight), ref scrollPos, ref scrollRect);
            for (int i = 0; i < seed.Seeds.Count; i++)
            {
                string item = texts[i];
                int value = seed.Seeds[i];
                std.IntEntry(ref value, ref item);
                seed.Seeds[i] = value;
                texts[i] = value.ToString();
            }

            //std.EndScrollView(ref scrollRect);
            std.End();
        }
    }
}
