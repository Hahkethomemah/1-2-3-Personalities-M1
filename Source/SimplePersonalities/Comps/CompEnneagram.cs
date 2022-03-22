using System.Collections.Generic;
using System.Text;
using Verse;

namespace SPM1.Comps
{
    public class CompEnneagram : ThingComp
    {
        private static StringBuilder str = new StringBuilder();

        public virtual bool NeedsNewPersonality => Enneagram == null || !Enneagram.IsValid;

        public Listing_Standard Listing;

        public Enneagram Enneagram
        {
            get
            {
                if(_enneagram == null)
                    AssignNewPersonality();
                return _enneagram;
            }
            protected set => _enneagram = value;
        }
        public DescriptionSeed Seed;

        public virtual bool DoHediffs => true;

        private Enneagram _enneagram;
        private string description;
        private float maxResistance = -1;

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Deep.Look(ref _enneagram, "spm_Enneagram");
            Scribe_Values.Look(ref maxResistance, "spm_MaxResistance", -1);
            int oldSeed = -1;
            Scribe_Values.Look(ref oldSeed, "spm_RandomSeed", -1);
            Scribe_Deep.Look(ref Seed, "spm_Seed");

            if (Settings.FreezeDescriptions)
            {
                Scribe_Values.Look(ref description, "sp_cachedDescription");
            }

            if (oldSeed != -1)
            {
                Seed = new DescriptionSeed(5, oldSeed);
                Core.Warn("Converted from old seed to new.");
            }
        }

        /// <summary>
        /// Note: This is not used internally. This is for external mod support,
        /// such as character editor or altered carbon.
        /// </summary>
        public virtual string ExtractSaveString()
        {
            const string NULL = "<NULL>";

            str.Clear();
            str.Append(Enneagram?.Root?.defName ?? NULL).Append('|');
            str.Append(Enneagram?.Variant?.defName ?? NULL).Append('|');
            str.Append(Enneagram?.MainTrait?.defName ?? NULL).Append('|');
            str.Append(Enneagram?.SecondaryTrait?.defName ?? NULL).Append('|');
            str.Append(Enneagram?.OptionalTrait?.defName ?? NULL).Append('|');
            str.Append(Seed?.ToSaveString() ?? NULL);

            return str.ToString();
        }

        /// <summary>
        /// Note: This is not used internally. This is for external mod support,
        /// such as character editor or altered carbon.
        /// </summary>
        public virtual void InsertSaveString(string saveString)
        {
            const string NULL = "<NULL>";
            if (string.IsNullOrWhiteSpace(saveString))
                return;

            string[] split = saveString.Split('|');
            if (split.Length != 6)
                return;

            if(Enneagram == null)
            {
                Core.Error("Enneagram null when trying to read in string save data!");
                return;
            }

            T ReadDef<T>(string name) where T : Def
            {
                if (name == NULL)
                    return null;
                return (T)DefDatabase<T>.GetNamed(name, true);
            }

            // Personality.
            Enneagram.Root = ReadDef<PersonalityRoot>(split[0]);
            Enneagram.Variant = ReadDef<PersonalityVariant>(split[1]);
            Enneagram.MainTrait = ReadDef<PersonalityTrait>(split[2]);
            Enneagram.SecondaryTrait = ReadDef<PersonalityTrait>(split[3]);
            Enneagram.OptionalTrait = ReadDef<PersonalityTrait>(split[4]);

            // Seed.
            if (split[5] != NULL)
                Seed = new DescriptionSeed(split[5]);
            else
                GenerateNewSeed();

            RegenerateDescription();
        }

        public virtual void GenerateNewSeed()
        {
            Seed = new DescriptionSeed();
        }

        public override string GetDescriptionPart()
        {
            return GetDescription().Trim();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (NeedsNewPersonality)
                AssignNewPersonality();
            
            EnsureHediffs();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            int i = 0;
            i++;
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
        }

        public void RegenerateDescription()
        {
            description = null;
            GetDescription(); // Caches the description.
        }

        public string GetDescription()
        {
            if (parent is not Pawn pawn)
                return null;

            if (description != null)
                return description;

            description = Enneagram.GenerateDescriptionFor(pawn, Seed);
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

        public void AssignNewPersonality()
        {
            GenerateNewSeed();
            _enneagram = GenerateNewEnneagram();
            EnsureHediffs();
        }

        protected virtual Enneagram GenerateNewEnneagram()
        {
            return Enneagram.Generate(parent as Pawn);
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

            if (DoHediffs)
            {
                // Add hediffs required by root.
                hediffs.AddRange(Enneagram.Root.hediffs);

                if (UseIfKnownHediffs())
                    hediffs.AddRange(Enneagram.Root.hediffsIfKnown);
            }

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
