using Verse;

namespace SPM1.Comps
{
    public class CompProperties_DamageFactor : HediffCompProperties
    {
        public float factor = 1f;

        public CompProperties_DamageFactor()
        {
            compClass = typeof(CompDamageFactor);
        }
    }
}
