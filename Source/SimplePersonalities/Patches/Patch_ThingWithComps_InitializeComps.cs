using System;
using System.Collections.Generic;
using HarmonyLib;
using SPM1.Comps;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(ThingWithComps), "InitializeComps")]
    static class Patch_ThingWithComps_InitializeComps
    {
        static void Postfix(ThingWithComps __instance, ref List<ThingComp> ___comps)
        {
            if (!PersonalityUtils.ShouldHavePersonality(__instance, out Type compType))
                return;

            ___comps ??= new List<ThingComp>();

            var newComp = Activator.CreateInstance(compType) as CompEnneagram;
            newComp.parent = __instance;
            ___comps.Add(newComp);

            var props = new CompProperties();
            props.compClass = compType;
            newComp.Initialize(props); // The comp properties are not used.
        }
    }
}
