using Verse;

namespace SPM1.Comps
{
    public class CompProperties_NuzzleFactor : HediffCompProperties
    {
        public float intervalFactor = 1f;

        public CompProperties_NuzzleFactor()
        {
            compClass = typeof(CompNuzzleFactor);
        }
    }
}
