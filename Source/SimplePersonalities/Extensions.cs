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

        public static string ExtractPersonality(this Pawn p)
        {
            var comp = p?.TryGetEnneagramComp();
            if (comp == null)
                return "<NULL>";

            return comp.ExtractSaveString();
        }

        public static void IntractPersonality(this Pawn p, string data)
        {
            if (p == null || data == null)
                return;

            bool isNull = data == "<NULL>";
            // This means that the original copied pawn did not have a personality comp...
            // Therefore assume that this pawn will not either.
            // If they do have a comp, they will retain whetever personality they already have, since there is nothing to overwrite
            // with and we cannot just remove the comp (it will be added again dynamically on startup).
            if (isNull)
                return;

            var comp = p.TryGetEnneagramComp();
            if(comp == null)
            {
                Core.Warn($"Tried to intract (write from string) personality to pawn '{p.Name}', but that pawn does not have a comp.");
                return;
            }

            comp.InsertSaveString(data);
        }
    }
}
