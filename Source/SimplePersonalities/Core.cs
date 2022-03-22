using HarmonyLib;
using SPM1.UI;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace SPM1
{
    [HotSwappable]
    public class Core : Mod
    {
        class HotSwappable : Attribute { }

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

            GetSettings<Settings>();

            if (Settings.UseWorldviewTab)
            {
                try
                {
                    var patch = CreatePawnPatchOperation();
                    var list = pack.Patches as List<PatchOperation>;
                    list.Add(patch);
                    Log("Patched tab!");
                }
                catch (Exception e)
                {
                    Error("Failed to create dynamic patch!", e);
                }
            }
        }

        private PatchOperation CreatePawnPatchOperation()
        {
            var op = new PatchOperationInsert();
            void Set(string name, object value)
            {
                AccessTools.Field(typeof(PatchOperationInsert), name).SetValue(op, value);
            }

            Set("xpath", "Defs/ThingDef[@Name='BasePawn']/inspectorTabs/li[text()='ITab_Pawn_Character']");

            var doc = new XmlDocument();
            var value = doc.CreateElement("value");
            var element = doc.CreateElement("li");
            element.InnerText = typeof(ITab_Pawn_Worldview).FullName;
            value.AppendChild(element);

            var container = new XmlContainer() { node = value };
            Set("value", container);

            return op;
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
