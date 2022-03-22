using EdB.PrepareCarefully;
using SPM1.UI;
using UnityEngine;
using Verse;

namespace SPM1.PrepareCarefully
{
    public class PersonalityEditTab : TabViewBase
    {
        //[TweakValue("__TEMP", 0f, 300f)]
        private static float Height = 100f;
        //[TweakValue("__TEMP", 0f, 100f)]
        private static float Padding = 10f;

        public override string Name => "SP.Personalities".Translate();

        private Vector2 scrollPos;
        private float lastHeight;

        public override void Draw(State state, Rect rect)
        {
            base.Draw(state, rect);


            Widgets.Label(rect, $"<b><size=32>{"SP.EditPersonalities".Translate()}</size></b>");
            rect.height -= 40;
            rect.y += 40;

            float height = 0;
            Widgets.BeginScrollView(rect, ref scrollPos, new Rect(0, 0, rect.width - 20, lastHeight));
            rect.width -= 20f;

            for (int i = 0; i < state.ColonyPawns.Count; i++)
            {
                var pawn = state.ColonyPawns[i];
                Rect area = new Rect(0, height, rect.width, Height);
                height += Height + Padding;

                Widgets.DrawBox(area, 1);
                area = area.ContractedBy(10);
                var oldArea = area;
                Widgets.ThingIcon(new Rect(area.x, area.y + 15, area.height - 20, area.height - 20), pawn.Pawn);
                area.x += area.height + 5;
                area.width -= area.height + 10;

                var comp = pawn.Pawn.TryGetEnneagramComp();
                var gram = comp?.Enneagram;

                if (gram == null)
                {
                    Widgets.Label(area, "SP.DoesNotHavePersonality".Translate());
                    continue;
                }

                Widgets.Label(area, gram.GenerateDescriptionFor(pawn.Pawn, comp.Seed));
                Widgets.DrawHighlightIfMouseover(oldArea);
                if (Widgets.ButtonInvisible(oldArea))
                {
                    Find.WindowStack.Add(new Dialog_PersonalityEditor() {Pawn = pawn.Pawn, ApplyOnClose = true});
                }
            }

            Widgets.EndScrollView();
            lastHeight = height;
        }
    }
}
