using System;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using SPM1.Comps;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(StockGenerator_Animals), "GenerateThings")]
    static class Patch_StockGenerator_Animals_GenerateThings
    {
        static IEnumerable<Thing> Postfix(IEnumerable<Thing> values)
        {
            foreach (var thing in values)
            {
                yield return thing;

                try
                {
                    var comp = (thing as ThingWithComps)?.TryGetEnneagramComp() as CompEnneagramAnimal;
                    if (comp == null)
                        continue;

                    comp.FlagAsTradeAnimal();
                }
                catch (Exception e)
                {
                    Core.Error("Exception in animal stock generation patch", e);
                }
            }
        }
    }
}
