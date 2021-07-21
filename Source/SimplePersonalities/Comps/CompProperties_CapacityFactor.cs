using Verse;

namespace SPM1.Comps
{
    public class CompProperties_CapacityFactor : HediffCompProperties
    {
        public float factor = 1f;

        public CompProperties_CapacityFactor()
        {
            compClass = typeof(CompCapacityFactor);
        }
    }
}
