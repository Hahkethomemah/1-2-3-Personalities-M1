using System;
using HarmonyLib;
using Verse;

namespace SPM1.PrepareCarefully
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            LongEventHandler.QueueLongEvent(DoPatch, "SP.PrepareCarefullyPatchLabel", false, null);
        }

        public static void DoPatch()
        {
            Harmony harmony = new Harmony("SimplePersonalities.Patch.PrepareCarefully");

            try
            {
                harmony.PatchAll();
                SPM1.Core.Log("Simple personalities: Patched Prepare Carefully!");
            }
            catch (Exception e)
            {
                SPM1.Core.Error("Simple Personalities failed to apply patch Prepare Carefully! Exception:", e);
            }
        }
    }
}
