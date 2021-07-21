using System.Collections.Generic;
using Verse;

namespace SPM1.Comps
{
    public class CompEnneagram : ThingComp
    {
        public virtual bool NeedsNewPersonality => Enneagram == null || !Enneagram.IsValid;

        public Listing_Standard Listing;
        public Enneagram Enneagram;
        public int RandomSeed = -1;

        private string description;
        private float maxResistance = -1;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref Enneagram, "spm_Enneagram");
            Scribe_Values.Look(ref maxResistance, "spm_MaxResistance", -1);
            Scribe_Values.Look(ref RandomSeed, "spm_RandomSeed", -1);

            if (RandomSeed == -1)
                GenerateNewSeed();
        }

        protected virtual void GenerateNewSeed()
        {
            RandomSeed = Rand.Range(1, 1000000);
        }

        public override string GetDescriptionPart()
        {
            return GetDescription().Trim();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (NeedsNewPersonality)
            {
                AssignNewPersonality();
                if (RandomSeed == -1)
                    RandomSeed = Rand.Range(1, 1000000);
            }
            else
            {
                EnsureHediffs();
            }
        }

        public void RegenerateDescription()
        {
            description = null;
            GetDescription(); // Caches the description.
        }

        public string GetDescription()
        {
            if (Enneagram == null || parent is not Pawn pawn)
                return null;

            if (description != null)
                return description;
            description = Enneagram.GenerateDescriptionFor(pawn, RandomSeed);
            return description;
        }

        public override void CompTick()
        {
            base.CompTick();

            if (maxResistance < 0 && parent is Pawn pawn && (pawn.guest?.IsPrisoner ?? false))
            {
                if (this.maxResistance < pawn.guest.Resistance)
                    this.maxResistance = pawn.guest.Resistance;
            }
        }

        public float GetMaxResistance()
        {
            return maxResistance;
        }

        public virtual void AssignNewPersonality()
        {
            GenerateNewSeed();
            Enneagram = GenerateNewEnneagram();
            EnsureHediffs();
        }

        protected virtual Enneagram GenerateNewEnneagram()
        {
            return Enneagram.Generate();
        }

        protected virtual bool UseIfKnownHediffs() => false;

        public virtual void EnsureHediffs()
        {
            if (Enneagram == null)
                return;

            var pawn = parent as Pawn;
            if (pawn?.health?.hediffSet?.hediffs == null)
                return;

            var set = pawn.health.hediffSet;

            bool HasHediff(HediffDef def, out Hediff current)
            {
                current = null;
                if (def == null)
                    return false;

                foreach (var item in set.hediffs)
                {
                    if (item != null && item.def == def)
                    {
                        current = item;
                        return true;
                    }
                }
                return false;
            }

            List<HediffDef> hediffs = new List<HediffDef>();
            List<Hediff> added = new List<Hediff>();

            // Add hediffs required by root.
            hediffs.AddRange(Enneagram.Root.hediffs);

            if(UseIfKnownHediffs())
                hediffs.AddRange(Enneagram.Root.hediffsIfKnown);

            // Add required hediffs.
            foreach (var def in hediffs)
            {
                if (!HasHediff(def, out var current))
                {
                    var created = pawn.health.AddHediff(def);
                    if (created != null)
                        added.Add(created);
                }
                else
                {
                    added.Add(current);
                }
            }

            // Remove old hediffs, which may be left over from an old personality.
            for (int i = 0; i < set.hediffs.Count; i++)
            {
                var diff = set.hediffs[i];
                if (diff == null)
                    continue;

                if (diff.def.tags != null && diff.def.tags.Contains("SP_AddedByPersonality") && !added.Contains(diff))
                {
                    Core.Warn($"Removing hediff {diff.def} from {pawn} because it is not required by current personality.");
                    pawn.health.RemoveHediff(diff);
                    i--;
                }
            }
        }
    }
}
