using System.Collections.Generic;
using RimWorld;
using SPM1.Comps;
using Verse;

namespace SPM1
{
    public static class Extensions
    {
        public static T RandomElementExcept<T>(this IEnumerable<T> list, T element) where T : class
        {
            if (list == null || list.EnumerableCount() < 2)
                return null;

            while (true)
            {
                T picked = list.RandomElement();
                if (picked != element)
                    return picked;
            }
        }

        public static bool HasTrait(this Pawn pawn, TraitDef def)
        {
            if (pawn?.story?.traits?.allTraits == null)
                return false;

            foreach (var item in pawn.story.traits.allTraits)
            {
                if (item.def == def)
                    return true;
            }
            return false;
        }

        public static Enneagram TryGetEnneagram(this ThingWithComps thing)
        {
            if (thing == null)
                return null;

            var comp = TryGetEnneagramComp(thing);
            return comp?.Enneagram;
        }

        public static CompEnneagram TryGetEnneagramComp(this ThingWithComps comps)
        {
            if (comps == null)
                return null;
            for (int i = 0; i < comps.AllComps.Count; i++)
            {
                var comp = comps.AllComps[i];
                if (comp is CompEnneagram e)
                    return e;
            }
            return null;
        }
    }
}
