using LudeonTK;
using SPM1.Comps;
using System;
using System.Collections.Generic;
using Verse;

namespace SPM1
{
    public static class PersonalityUtils
    {
        private static readonly List<object> tempObjList = new List<object>();

        [DebugAction("Simple Personalities", "Re-generate personality", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void RegeneratePersonality(Pawn p)
        {
            var comp = p.TryGetEnneagramComp();
            if (comp != null)
            {
                comp.AssignNewPersonality();
                comp.RegenerateDescription();
                DebugActionsUtility.DustPuffFrom(p);
            }
            DebugActionsUtility.DustPuffFrom(p);
        }

        [DebugAction("Simple Personalities", "Re-generate description", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void RegenerateDescription(Pawn p)
        {
            var comp = p.TryGetEnneagramComp();
            if (comp != null)
            {
                comp.RegenerateDescription();
                DebugActionsUtility.DustPuffFrom(p);
            }
        }

        [DebugAction("Simple Personalities", "Log personality", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void LogPersonality(Pawn p)
        {
            var gram = p.TryGetEnneagramComp();
            if (gram != null)
            {
                Core.Log($"{p.LabelShortCap}'s personality: {gram.Enneagram}");
                Core.Log(gram.GetDescription());
                DebugActionsUtility.DustPuffFrom(p);
            }
        }

        private static string saveString;

        [DebugAction("Simple Personalities", "Extract save string", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ExtractSaveString(Pawn p)
        {
            saveString = p.ExtractPersonality();
            Core.Log($"Extracted save string:");
            Log.Message(saveString);
        }

        [DebugAction("Simple Personalities", "Insert save string", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void IntractSaveString(Pawn p)
        {
            p.IntractPersonality(saveString);
            Core.Log($"Intracted save string!");
        }

        public static bool ShouldHavePersonality(ThingWithComps thing, out Type compType)
        {
            compType = null;

            if (thing == null)
                return false;

            if (thing is not Pawn pawn)
                return false;

            if (pawn.RaceProps == null)
                return false;

            // Animals, mechanical or not, can have animal personalities.
            // This is so that your cyber-cat pet can have personality.
            bool isAnimal = pawn.RaceProps.Animal;
            if (isAnimal)
            {
                compType = typeof(CompEnneagramAnimal);
                return true;
            }

            // Below here is non-animals only...
            compType = typeof(CompEnneagram);

            // Only fleshy things can have personalities.
            // This should include most modded races (assuming they meet the intelligence requirement),
            // but will exclude mechanoids, androids etc.
            if (!pawn.RaceProps.IsFlesh)
                return false;

            // Minimum intelligence is human-level.
            if (pawn.RaceProps.intelligence < Intelligence.Humanlike)
                return false;

            return true;
        }

        public static PersonalityVisibility GetPersonalityVisibility(ThingWithComps thing)
        {
            // Don't show in preparation stage.
            if (Current.ProgramState != ProgramState.Playing)
                return PersonalityVisibility.Invisible;

            if (thing == null)
                return PersonalityVisibility.Invisible;

            // If the thing is not pawn, hide.
            if (thing is not Pawn pawn)
                return PersonalityVisibility.Invisible;

            // If the pawn is of the player faction and is not an animal (animals are handled below)
            if (pawn.Faction != null && pawn.Faction.IsPlayer && !pawn.RaceProps.Animal)
                return PersonalityVisibility.Visible;

            // Get comp (or fail and return invisible).
            var comp = thing.TryGetEnneagramComp();
            if (comp == null)
                return PersonalityVisibility.Invisible;

            // Is the pawn an animal?
            if (pawn.RaceProps?.Animal ?? false)
            {
                var compAnimal = comp as CompEnneagramAnimal;
                return compAnimal.IsPersonalityKnown ? PersonalityVisibility.Visible : PersonalityVisibility.Hidden;
            }

            // Not player colony pawn, not animal.
            if (!pawn.IsPrisonerOfColony)
            {
                // Pawns of other factions are hidden.
                return PersonalityVisibility.Hidden;
            }

            // Code below here is for prisoners only.
            float maxRes = comp.GetMaxResistance();
            if (maxRes < 0f)
                return PersonalityVisibility.Hidden;
            if (maxRes < 4f) // Prisoners with very low resistance automatically reveal personality.
                return PersonalityVisibility.Visible;

            return pawn.guest.Resistance <= 0.5f * maxRes ? PersonalityVisibility.Visible : PersonalityVisibility.Hidden;
        }

        public static string ReplaceGendered(string toInterpolate, List<string> toReplace, bool isMale)
        {
            if (toInterpolate == null)
                return null;
            if (toReplace == null || toReplace.Count == 0)
                return toInterpolate;

            tempObjList.Clear();
            foreach (var raw in toReplace)
            {
                string[] split = raw.Split('/');
                tempObjList.Add(isMale ? split[0] : split[1]);

            }
            return string.Format(toInterpolate, tempObjList.ToArray());
        }
    }

    public enum PersonalityVisibility
    {
        /// <summary>
        /// Is simply not displayed.
        /// </summary>
        Invisible,
        /// <summary>
        /// The user is explicitly shown that the person does
        /// have a personality, but it is hidden.
        /// This happens when the pawn is a prisoner being recruited.
        /// </summary>
        Hidden,
        /// <summary>
        /// The personality is fully visible.
        /// </summary>
        Visible
    }
}
