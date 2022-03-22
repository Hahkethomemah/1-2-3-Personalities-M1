using System;
using HarmonyLib;
using Verse;

namespace SPM1.CharacterEditor
{
    public class Core : Mod
    {
        public Core(ModContentPack content) : base(content)
        {
            LongEventHandler.QueueLongEvent(DoPatch, "SP.CharacterEditorPatchLabel", false, null);
        }

        public static void DoPatch()
        {
            Harmony harmony = new Harmony("SimplePersonalities.Patch.CharacterEditor");

            try
            {
                harmony.PatchAll();
                SPM1.Core.Log("Simple personalities: Patched Character Editor!");
            }
            catch (Exception e)
            {
                SPM1.Core.Error("Simple Personalities failed to apply patch Character Editor! Exception:", e);
            }
        }
    }
}
