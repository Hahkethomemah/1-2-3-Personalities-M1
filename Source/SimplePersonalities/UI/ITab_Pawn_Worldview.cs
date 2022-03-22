using RimWorld;
using UnityEngine;

namespace SPM1.UI
{
    public class ITab_Pawn_Worldview : ITab
    {
        public override bool IsVisible => true;

        public ITab_Pawn_Worldview()
        {
            this.size = new Vector2(460f, 450f);
            this.labelKey = "SP.TabLabel";
        }

        protected override void FillTab()
        {
            var rect = new Rect(10, 30, size.x - 20, size.y - 30);
            var pawn = base.SelPawn;

            var comp = pawn?.TryGetEnneagramComp();
            if (comp?.Enneagram == null || !comp.Enneagram.IsValid)
                return;

            var vis = PersonalityUtils.GetPersonalityVisibility(pawn);
            PersonalityUI.DrawPersonalityBio(pawn, comp, rect, vis);
        }
    }
}
