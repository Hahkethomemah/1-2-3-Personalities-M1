using System;
using System.Reflection;
using CharacterEditor;
using HarmonyLib;
using SPM1.UI;
using UnityEngine;
using Verse;

namespace SPM1.CharacterEditor
{
    [HarmonyPatch(typeof(BlockBio), "Draw")]
    static class Patch
    {
        [TweakValue("__TEMP", 0, 600)]
        private static int Offset = 420;
        [TweakValue("__TEMP", 0, 600)]
        private static int Offset2 = 380;

        static PropertyInfo CurrentPawnProperty;
        static FieldInfo EditorField;

        static void Postfix(int x, int y, int w, int h)
        {
            if (CurrentPawnProperty == null)
            {
                Type editor = AccessTools.TypeByName("CharacterEditor.Editor");
                if (editor == null)
                    SPM1.Core.Error("Editor not found!");
                CurrentPawnProperty = AccessTools.Property(editor, "Pawn");
                if (editor == null)
                    SPM1.Core.Error("Pawn not found!");
                EditorField = AccessTools.Field(typeof(PRMod), "API");
                if (editor == null)
                    SPM1.Core.Error("API not found!");
            }

            Pawn pawn = CurrentPawnProperty.GetValue(EditorField.GetValue(null)) as Pawn;

            if (pawn == null)
                return;

            var comp = pawn.TryGetEnneagramComp();
            var gram = comp?.Enneagram;
            if (gram == null)
                return;

            GUI.color = Color.white;
            Rect area = new Rect(x + 320, y + 100 + Offset, w - 320, h - (100 + Offset));
            Rect area2 = new Rect(x + 320, y + 100 + Offset2, w - 320, 30);

            string desc = gram.GenerateDescriptionFor(pawn, comp.Seed);

            Widgets.Label(area, desc);

            if (Widgets.ButtonText(area2, "SP.EditPersonality".Translate()))
            {
                Find.WindowStack.Add(new Dialog_PersonalityEditor() {Pawn = pawn, ApplyOnClose = true});
            }
        }
    }
}
