using System.Collections.Generic;
using System.Text;
using Verse;

namespace SPM1
{
    public class Enneagram : IExposable
    {
        private static HashSet<PersonalityTrait> tempHashset = new HashSet<PersonalityTrait>();

        public static Enneagram Generate(Pawn forPawn)
        {
            var gram = new Enneagram();
            var root = AllRootsFor(forPawn).RandomElement();

            gram.Root = root;
            gram.Variant = root.GetRandomVariant();
            gram.MainTrait = root.GetRandomCompatibleTrait(gram.Variant);
            gram.SecondaryTrait = AllRootsFor(forPawn).RandomElement().GetRandomCompatibleTrait(gram.Variant, gram.MainTrait);

            if (Rand.Chance(1f / 3f))
            {
                tempHashset.Clear();
                tempHashset.Add(gram.MainTrait);
                tempHashset.Add(gram.SecondaryTrait);
                gram.OptionalTrait = AllRootsFor(forPawn).RandomElementExcept(root).GetRandomCompatibleTrait(gram.Variant, tempHashset);
            }

            return gram;
        }

        public static IEnumerable<PersonalityRoot> AllRootsFor(Pawn pawn)
        {
            return PersonalityRoot.AllHuman;
        }

        public virtual bool IsValid => Root != null && Variant != null && MainTrait != null && SecondaryTrait != null;

        // Must always exist:
        public PersonalityRoot Root;
        public PersonalityVariant Variant;
        public PersonalityTrait MainTrait;
        public PersonalityTrait SecondaryTrait;

        // Will optionally exist:
        public PersonalityTrait OptionalTrait;

        public Enneagram() { }

        public Enneagram(Enneagram toCreateCopyOf)
        {
            if (toCreateCopyOf == null)
                return;

            this.Root = toCreateCopyOf.Root;
            this.Variant = toCreateCopyOf.Variant;
            this.MainTrait = toCreateCopyOf.MainTrait;
            this.SecondaryTrait = toCreateCopyOf.SecondaryTrait;
            this.OptionalTrait = toCreateCopyOf.OptionalTrait;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref Root, "root");
            Scribe_Defs.Look(ref Variant, "variant");
            Scribe_Defs.Look(ref MainTrait, "mainTrait");
            Scribe_Defs.Look(ref SecondaryTrait, "secondaryTrait");
            Scribe_Defs.Look(ref OptionalTrait, "optionalTrait");
        }

        public virtual string GenerateDescriptionFor(Pawn pawn, DescriptionSeed seed)
        {
            if (pawn == null)
                return null;

            return DescriptionGenerator.Generate(SPDefOf.SP_HumanStack, pawn, seed);
        }

        public override string ToString()
        {
            return $"{MainTrait?.LabelCap} {Variant?.LabelCap} - {MainTrait}, {SecondaryTrait}{(OptionalTrait == null ? "" : $", {OptionalTrait.label}")}.";
        }
    }
}
