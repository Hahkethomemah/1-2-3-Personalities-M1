using System.Collections.Generic;
using Verse;

namespace SPM1
{
    public class EnneagramAnimal : Enneagram
    {
        public new static EnneagramAnimal Generate(Pawn forPawn)
        {
            var gram = new EnneagramAnimal();
            var root = AllRootsFor(forPawn).RandomElement();

            gram.Root = root;
            gram.Variant = root.GetRandomVariant();

            return gram;
        }

        public new static IEnumerable<PersonalityRoot> AllRootsFor(Pawn pawn)
        {
            return PersonalityRoot.AllAnimal;
        }

        public override bool IsValid => Root != null && Variant != null;

        public EnneagramAnimal() { }

        public EnneagramAnimal(EnneagramAnimal toCreateCopyOf) : base(toCreateCopyOf)
        {

        }

        public override string GenerateDescriptionFor(Pawn pawn, DescriptionSeed seed)
        {
            if (pawn == null)
                return null;

            return DescriptionGenerator.Generate(SPDefOf.SP_AnimalStack, pawn, seed);
        }

        public override string ToString()
        {
            return $"{MainTrait?.LabelCap} {Variant?.LabelCap}.";
        }
    }
}
