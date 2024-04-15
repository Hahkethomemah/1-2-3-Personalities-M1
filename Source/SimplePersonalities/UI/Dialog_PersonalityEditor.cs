using System;
using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using UnityEngine;
using Verse;

namespace SPM1.UI
{
    public class Dialog_PersonalityEditor : Window
    {
        [DebugAction("Simple Personalities", "Edit personality (pawn)", actionType = DebugActionType.ToolMapForPawns)]
        public static void OpenDialogFor(Pawn pawn)
        {
            Find.WindowStack.Add(new Dialog_PersonalityEditor() {Pawn = pawn});
        }

        [DebugAction("Simple Personalities", "Edit personality (generic, human)", actionType = DebugActionType.Action)]
        public static void OpenDialogGenericHuman()
        {
            Find.WindowStack.Add(new Dialog_PersonalityEditor() { Gram = new Enneagram() });
        }

        [DebugAction("Simple Personalities", "Edit personality (generic, animal)", actionType = DebugActionType.Action)]
        public static void OpenDialogGenericAnimal()
        {
            Find.WindowStack.Add(new Dialog_PersonalityEditor() { Gram = new EnneagramAnimal() });
        }

        public override Vector2 InitialSize => new Vector2(500, 600);
        
        public Pawn Pawn;
        public Enneagram Gram
        {
            get => _gram ?? Pawn?.TryGetEnneagram();
            set => _gram = value;
        }
        public bool ApplyOnClose;

        private Enneagram _gram;
        private Listing_Standard std = new Listing_Standard();
        private List<PersonalityTrait> tempTraits = new List<PersonalityTrait>();
        private HashSet<PersonalityTrait> tempHashTraits = new HashSet<PersonalityTrait>();

        public Dialog_PersonalityEditor()
        {
            closeOnClickedOutside = false;
            doCloseX = false;
            doCloseButton = true;
            resizeable = true;
            preventCameraMotion = false;
            drawShadow = true;
            draggable = true;
            layer = WindowLayer.Super;
        }

        public override void DoWindowContents(Rect inRect)
        {
            if(Pawn != null && Pawn.Destroyed)
            {
                Core.Warn($"Personality editor for '{Pawn.Name}' is closing because that pawn was destroyed.");
                Close();
                return;
            }

            var gram = Gram;
            if (gram == null)
            {
                Core.Error("Opened personality editor window with null enneagram.");
                Close();
                return;
            }

            bool isAnimal = gram is EnneagramAnimal;

            Rect labelRect = inRect;
            if (Pawn != null)
            {
                labelRect.x += 40;
                Widgets.ThingIcon(new Rect(inRect.x, inRect.y, 32, 32), Pawn);
            }
            Widgets.Label(labelRect, $"<b><size=28>{"SP.EditingFor".Translate(Pawn?.LabelShortCap ?? $"<generic, {(isAnimal ? "animal" : "human")}>")}</size></b>");

            inRect.y += 50;
            inRect.height -= 80f;
            std.Begin(inRect);

            // ROOT
            string root = gram.Root?.LabelColor ?? "SP.MissingPart".Translate();
            if (std.ButtonText("SP.RootLabel".Translate(root)))
            {
                var roots = isAnimal ? EnneagramAnimal.AllRootsFor(Pawn) : Enneagram.AllRootsFor(Pawn);
                MakeDropdown(roots, t => t.LabelColor, t =>
                {
                    gram.Root = t;
                    if(gram.Variant != null && !t.variants.Contains(gram.Variant))
                    {
                        Core.Warn("Resetting enneagram since new root was picked and variant is not from that root.");
                        gram.Variant = null;
                        gram.MainTrait = null;
                        gram.SecondaryTrait = null;
                        gram.OptionalTrait = null;
                    }
                });
            }

            // VARIANT
            bool isMale = Pawn == null || Pawn.gender != Gender.Female;
            string variant = gram.Variant?.GetLabelColor(isMale) ?? "SP.MissingPart".Translate();
            GUI.enabled = gram.Root != null;
            if (std.ButtonText("SP.VariantLabel".Translate(variant)))
            {
                var variants = gram.Root.variants;
                MakeDropdown(variants, t => t.GetLabelColor(isMale), t =>
                {
                    gram.Variant = t;
                    if (gram.MainTrait != null && !gram.MainTrait.IsCompatibleWith(t))
                    {
                        Core.Warn($"Removing main trait '{gram.MainTrait}' because it is incompatible with new variant '{gram.Variant}'");
                        gram.MainTrait = null;
                    }
                    if (gram.SecondaryTrait != null && !gram.SecondaryTrait.IsCompatibleWith(t))
                    {
                        Core.Warn($"Removing secondary trait '{gram.SecondaryTrait}' because it is incompatible with new variant '{gram.Variant}'");
                        gram.SecondaryTrait = null;
                    }

                    if (gram.OptionalTrait != null && !gram.OptionalTrait.IsCompatibleWith(t))
                    {
                        Core.Warn($"Removing optional trait '{gram.OptionalTrait}' because it is incompatible with new variant '{gram.Variant}'");
                        gram.OptionalTrait = null;
                    }
                });
            }
            GUI.enabled = true;

            if (!isAnimal)
            {
                // MAIN TRAIT
                GUI.enabled = gram.Variant != null && gram.Root != null;
                string main = gram.MainTrait?.LabelColor ?? "SP.MissingPart".Translate();
                if (std.ButtonText("SP.MainTraitLabel".Translate(main)))
                {
                    tempHashTraits.Clear();
                    if (gram.SecondaryTrait != null)
                        tempHashTraits.Add(gram.SecondaryTrait);
                    if (gram.OptionalTrait != null)
                        tempHashTraits.Add(gram.OptionalTrait);

                    var traits = gram.Root.GetAllCompatibleTraits(gram.Variant, tempHashTraits);

                    MakeDropdown(traits, t => t.LabelColor, t =>
                    {
                        gram.MainTrait = t;
                    });
                }
                GUI.enabled = true;

                // SECONDARY TRAIT
                GUI.enabled = gram.Variant != null && gram.Root != null && gram.MainTrait != null;
                string secondary = gram.SecondaryTrait?.LabelColor ?? "SP.MissingPart".Translate();
                if (std.ButtonText("SP.SecondaryTraitLabel".Translate(secondary)))
                {
                    tempHashTraits.Clear();
                    tempHashTraits.Add(gram.MainTrait);
                    if (gram.OptionalTrait != null)
                        tempHashTraits.Add(gram.OptionalTrait);

                    tempTraits.Clear();
                    var roots = isAnimal ? EnneagramAnimal.AllRootsFor(Pawn) : Enneagram.AllRootsFor(Pawn);

                    foreach (var r in roots)
                    {
                        var traits = r.GetAllCompatibleTraits(gram.Variant, tempHashTraits);
                        tempTraits.AddRange(traits);
                    }

                    MakeDropdown(tempTraits, t => t.LabelColor, t =>
                    {
                        gram.SecondaryTrait = t;
                    });
                }
                GUI.enabled = true;

                // OPTIONAL TRAIT
                GUI.enabled = gram.Variant != null && gram.Root != null && gram.MainTrait != null && gram.SecondaryTrait != null;
                string optional = gram.OptionalTrait?.LabelColor ?? "SP.MissingPart".Translate();
                if (std.ButtonText("SP.OptionalTraitLabel".Translate(optional)))
                {
                    tempHashTraits.Clear();
                    tempHashTraits.Add(gram.MainTrait);
                    tempHashTraits.Add(gram.SecondaryTrait);

                    tempTraits.Clear();
                    tempTraits.Add(null);
                    var roots = isAnimal ? EnneagramAnimal.AllRootsFor(Pawn) : Enneagram.AllRootsFor(Pawn);

                    foreach (var r in roots)
                    {
                        // Don't include active root, since optional trait must come from outside own root.
                        if (r == gram.Root)
                            continue;

                        var traits = r.GetAllCompatibleTraits(gram.Variant, tempHashTraits);
                        tempTraits.AddRange(traits);
                    }

                    MakeDropdown(tempTraits, t => t?.LabelColor ?? "SP.MissingPart".Translate(), t =>
                    {
                        gram.OptionalTrait = t;
                    });
                }
                GUI.enabled = true;
            }

            // Draw actual description if possible.
            if (Pawn != null)
            {
                std.GapLine();
                var comp = Pawn.TryGetEnneagramComp();
                if (!isAnimal && comp != null && std.ButtonText("SP.RollNewDescription".Translate()))
                {
                    comp.GenerateNewSeed();
                }
                if (!isAnimal && comp?.Seed != null && std.ButtonText("SP.TweakDescription".Translate()))
                {
                    Find.WindowStack.Add(new Dialog_SeedEditor(comp.Seed));
                }

                if (!ApplyOnClose)
                {
                    GUI.color = Color.Lerp(Color.green, Color.white, 0.5f);
                    if (comp != null && std.ButtonText("SP.ApplyToCharacter".Translate()))
                    {
                        comp.RegenerateDescription();
                    }
                    GUI.color = Color.white;
                }

                var seed = comp?.Seed;
                string desc = gram.GenerateDescriptionFor(Pawn, seed);
                std.Label(desc);
            }

            // Show invalid warning if applicable.
            if (!gram.IsValid)
            {
                std.GapLine();
                std.Label($"<color=red>{"SP.InvalidWarning".Translate()}</color>");
            }

            std.End();
        }

        public override void PostClose()
        {
            base.PostClose();

            if (!ApplyOnClose)
                return;

            var comp = Pawn?.TryGetEnneagramComp();
            comp?.RegenerateDescription();
        }

        private void MakeDropdown<T>(IEnumerable<T> options, Func<T, string> makeLabel, Action<T> onClick)
        {
            FloatMenuUtility.MakeMenu(options, makeLabel, t => () =>
            {
                onClick(t);
            });
        }
    }
}
