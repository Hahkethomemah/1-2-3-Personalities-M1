using Verse;

namespace SPM1
{
    public class Settings : ModSettings
    {
        public static string ForcedTraitColor => UseSingleColor ? "#a2ebe9" : null;

        [TweakValue("Simple Personalities")]
        public static bool UseSingleColor = false;
        
        [TweakValue("Simple Personalities", 0, 120_000)]
        public static int AnimalTicksToReveal = 60_000 * 3; // 3 days.

        [TweakValue("Simple Personalities")]
        public static float AnimalPersonalityOnTameFail = 0.5f;

        [TweakValue("Simple Personalities")]
        public static float AnimalPersonalityOnTameSuccess = 0.5f;
    }
}
