using HarmonyLib;
using RimWorld;
using Verse;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(CompAbilityEffect_OffsetPrisonerResistance), "Apply")]
    static class Patch_CompAbilityEffect_OffsetPrisonerResistance_Apply
    {
        private static void Prefix(LocalTargetInfo target, out PersonalityVisibility __state)
        {
            Pawn prisoner = target.Pawn;

            Patch_InteractionWorker_RecruitAttempt_Interacted.Pre(prisoner, out __state);
        }

        private static void Postfix(CompAbilityEffect_OffsetPrisonerResistance __instance, LocalTargetInfo target, PersonalityVisibility __state)
        {
            Pawn recruiter = __instance.parent.pawn;
            Pawn prisoner = target.Pawn;
            Patch_InteractionWorker_RecruitAttempt_Interacted.Post(recruiter, prisoner, __state, "psycast");
        }
    }
}
