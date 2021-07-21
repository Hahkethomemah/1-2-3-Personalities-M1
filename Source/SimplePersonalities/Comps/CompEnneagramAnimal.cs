using RimWorld;
using Verse;

namespace SPM1.Comps
{
    public class CompEnneagramAnimal : CompEnneagram
    {
        public bool IsPersonalityKnown;
        public int TicksSinceJoin = -1;

        private int tickCounter;

        protected override Enneagram GenerateNewEnneagram()
        {
            return EnneagramAnimal.Generate();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref IsPersonalityKnown, "spm_animalIsKnown", false);
            Scribe_Values.Look(ref TicksSinceJoin, "spm_ticksSinceJoin", -1);
        }

        public override void CompTick()
        {
            base.CompTick();

            if (IsPersonalityKnown)
                return;

            const int INTERVAL = 120;

            tickCounter++;
            if (tickCounter % INTERVAL == 0)
            {
                var pawn = parent as Pawn;
                if (pawn?.Faction?.IsPlayer ?? false)
                {
                    TicksSinceJoin += INTERVAL;
                    if(TicksSinceJoin >= Settings.AnimalTicksToReveal)
                    {
                        IsPersonalityKnown = true;
                        OnPersonalityDiscovered(DiscoveryType.FromTimer);
                    }
                }
            }
        }

        public void OnPersonalityDiscovered(DiscoveryType type, Pawn trainer = null)
        {
            EnsureHediffs();

            switch (type)
            {
                case DiscoveryType.FromTimer:
                    Messages.Message("SP.AnimalReveal.Time".Translate(parent.LabelShortCap, parent.Faction.Name), MessageTypeDefOf.PositiveEvent);
                    break;

                case DiscoveryType.FromTrainAttempt:
                    Messages.Message("SP.AnimalReveal.Training".Translate(trainer.LabelShortCap, parent.LabelShortCap), MessageTypeDefOf.PositiveEvent);
                    break;
            }
        }

        public override string GetDescriptionPart()
        {
            string root = null;
            if (!IsPersonalityKnown)
            {
                if (parent?.Faction?.IsPlayer ?? false)
                    root = "SP.HiddenAnimalOfFactionPersonality".Translate();
                else
                    root = "SP.HiddenAnimalPersonality".Translate();
            }
            if (root != null)
                return root;

            return GetDescription().Trim();
        }

        protected override bool UseIfKnownHediffs()
        {
            return IsPersonalityKnown;
        }

        public override string CompInspectStringExtra()
        {
            string root;
            if (!IsPersonalityKnown)
            {
                if (parent?.Faction?.IsPlayer ?? false)
                    root = "SP.HiddenAnimalOfFactionPersonality".Translate();
                else
                    root = "SP.HiddenAnimalPersonality".Translate();
            }
            else
                root = GetDescription().Trim();

            //if (Prefs.DevMode)
            //    root += $"\nIsKnown: {IsPersonalityKnown}, TicksSinceJoin: {TicksSinceJoin}";

            return root;
        }

        public enum DiscoveryType
        {
            FromTimer,
            FromTrainAttempt
        }
    }
}
