using HarmonyLib;
using System;
using UnityEngine;
using Verse;

namespace SPM1
{
    public class Core : Mod
    {
        public static void Log(string msg)
        {
            Verse.Log.Message($"<color=#34eb92>[Simple Personalities]</color> {msg}");
        }

        public static void Warn(string msg)
        {
            Verse.Log.Warning($"<color=#34eb92>[Simple Personalities]</color> {msg}");
        }

        public static void Error(string msg, Exception e = null)
        {
            Verse.Log.Error($"<color=#34eb92>[Simple Personalities]</color> {msg}");
            if(e != null)
                Verse.Log.Error(e.ToString());
        }

        public static void Trace(string msg)
        {
            Verse.Log.Message($"<color=#34eb92>[Simple Personalities] [TRACE] </color> {msg}");
        }

        public Core(ModContentPack pack) : base(pack)
        {
            Log("Hello, world!");

            try
            {
                var harmony = new Harmony("hahkethomemah.simplepersonalities");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Error("Failed to patch 1 or more methods! The mod will probably not work!", e);
            }

            LongEventHandler.QueueLongEvent(Load, "SP.LoadingLabel", false, null);
        }

        public void Load()
        {
            // Do any work here.
            GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DrawUI(inRect);
        }
    }
}
