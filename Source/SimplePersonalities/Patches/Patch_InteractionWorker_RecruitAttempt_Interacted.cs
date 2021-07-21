using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace SPM1.Patches
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "Interacted")]
    static class Patch_InteractionWorker_RecruitAttempt_Interacted
    {
        private static void Prefix(Pawn recipient, out PersonalityVisibility __state)
        {
            Pre(recipient, out __state);
        }

        private static void Postfix(Pawn initiator, Pawn recipient, PersonalityVisibility __state)
        {
            Post(initiator, recipient, __state, "talk");
        }

        public static void Pre(Pawn recipient, out PersonalityVisibility __state)
        {
            if (!PersonalityUtils.ShouldHavePersonality(recipient, out _))
            {
                __state = PersonalityVisibility.Hidden;
                return;
            }

            __state = PersonalityUtils.GetPersonalityVisibility(recipient);
        }

        public static void Post(Pawn initiator, Pawn recipient, PersonalityVisibility __state, string type)
        {
            if (!PersonalityUtils.ShouldHavePersonality(recipient, out _))
                return;
            
            var newState = PersonalityUtils.GetPersonalityVisibility(recipient);
            if (newState != __state && newState == PersonalityVisibility.Visible)
            {
                var request = new GrammarRequest();
                request.Rules.AddRange(GrammarUtility.RulesForPawn("PRISONER", recipient, request.Constants, true, true));
                request.Rules.AddRange(GrammarUtility.RulesForPawn("RECRUITER", initiator, request.Constants, true, true));
                request.Includes.Add(SPDefOf.SP_PersonalityRevealed);
                string msg = GrammarResolver.Resolve(type, request);
                Messages.Message(msg, new LookTargets(initiator, recipient), MessageTypeDefOf.PositiveEvent, true);
            }
        }
    }
}
