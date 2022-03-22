using System.Collections.Generic;
using EdB.PrepareCarefully;
using HarmonyLib;
using Verse;

namespace SPM1.PrepareCarefully
{
    [HarmonyPatch(typeof(Page_PrepareCarefully), "PreOpen")]
    static class Patch
    {
        static void Prefix(Page_PrepareCarefully __instance, List<TabRecord> ___tabRecords, List<ITabView> ___tabViews)
        {
            var tab = new PersonalityEditTab();
            ___tabViews.Add(tab);

            TabRecord tabRecord = new TabRecord(tab.Name, delegate ()
            {
                if (__instance.State.CurrentTab != null)
                {
                    __instance.State.CurrentTab.TabRecord.selected = false;
                }
                __instance.State.CurrentTab = tab;
                tab.TabRecord.selected = true;
            }, false);
            tab.TabRecord = tabRecord;
            ___tabRecords.Add(tabRecord);
		}
    }
}
