using HarmonyLib;
using RimWorld;
using SPM1.Comps;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(Pawn_TrainingTracker), "Train")]
    static class Patch_Pawn_TrainingTracker_Train
    {
        static void Prefix(Pawn ___pawn, Pawn trainer)
        {
            if (___pawn?.TryGetEnneagramComp() is not CompEnneagramAnimal comp)
                return;

            if (trainer == null || !(trainer.Faction?.IsPlayer ?? false))
                return;

            comp.OnTrain(trainer);
        }
    }
}
