using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using SPM1.Comps;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(ScenPart_StartingAnimal), "PlayerStartingThings")]
    static class Patch_ScenPart_StartingAnimal_PlayerStartingThings
    {
        static IEnumerable<Thing> Postfix(IEnumerable<Thing> values)
        {
            foreach (var thing in values)
            {
                if (thing is ThingWithComps comps)
                {
                    var comp = comps.TryGetEnneagramComp() as CompEnneagramAnimal;
                    if (comp != null)
                    {
                        comp.FlagAsStartingAnimal();
                    }
                    else
                    {
                        Core.Error($"Failed to find animal enneagram comp for starting animal '{thing.LabelCap}'");
                    }
                }

                yield return thing;
            }
        }
    }
}
