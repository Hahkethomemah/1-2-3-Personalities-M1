using Verse;

namespace SPM1
{
    public class EnneagramAnimal : Enneagram
    {
        public new static EnneagramAnimal Generate()
        {
            var gram = new EnneagramAnimal();
            var root = PersonalityRoot.AllAnimal.RandomElement();

            gram.Root = root;
            gram.Variant = root.GetRandomVariant();

            return gram;
        }

        public override bool IsValid => Root != null && Variant != null;

        public override string GenerateDescriptionFor(Pawn pawn, int? randomSeed)
        {
            if (pawn == null)
                return null;

            return DescriptionGenerator.Generate(SPDefOf.SP_AnimalStack, pawn, randomSeed);
        }

        public override string ToString()
        {
            return $"{MainTrait?.LabelCap} {Variant?.LabelCap}.";
        }
    }
}
