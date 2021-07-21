using System.Collections.Generic;
using Verse;

namespace SPM1
{
    public class Enneagram : IExposable
    {
        private static HashSet<PersonalityTrait> tempHashset = new HashSet<PersonalityTrait>();

        public static Enneagram Generate()
        {
            var gram = new Enneagram();
            var root = PersonalityRoot.AllHuman.RandomElement();

            gram.Root = root;
            gram.Variant = root.GetRandomVariant();
            gram.MainTrait = root.GetRandomCompatibleTrait(gram.Variant);
            gram.SecondaryTrait = PersonalityRoot.AllHuman.RandomElement().GetRandomCompatibleTrait(gram.Variant, gram.MainTrait);

            if (Rand.Chance(1f / 3f))
            {
                tempHashset.Clear();
                tempHashset.Add(gram.MainTrait);
                tempHashset.Add(gram.SecondaryTrait);
                gram.OptionalTrait = PersonalityRoot.AllHuman.RandomElementExcept(root).GetRandomCompatibleTrait(gram.Variant, tempHashset);
            }

            return gram;
        }

        public virtual bool IsValid => Root != null && Variant != null && MainTrait != null && SecondaryTrait != null;

        // Must always exist:
        public PersonalityRoot Root;
        public PersonalityVariant Variant;
        public PersonalityTrait MainTrait;
        public PersonalityTrait SecondaryTrait;

        // Will optionally exist:
        public PersonalityTrait OptionalTrait;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref Root, "root");
            Scribe_Defs.Look(ref Variant, "variant");
            Scribe_Defs.Look(ref MainTrait, "mainTrait");
            Scribe_Defs.Look(ref SecondaryTrait, "secondaryTrait");
            Scribe_Defs.Look(ref OptionalTrait, "optionalTrait");
        }

        public virtual string GenerateDescriptionFor(Pawn pawn, int? randomSeed)
        {
            if (pawn == null)
                return null;

            return DescriptionGenerator.Generate(SPDefOf.SP_HumanStack, pawn, randomSeed);
        }

        public override string ToString()
        {
            return $"{MainTrait?.LabelCap} {Variant?.LabelCap} - {MainTrait}, {SecondaryTrait}{(OptionalTrait == null ? "" : $", {OptionalTrait.label}")}.";
        }
    }
}
