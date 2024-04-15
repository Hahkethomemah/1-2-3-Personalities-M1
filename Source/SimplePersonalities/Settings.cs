using LudeonTK;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace SPM1
{
    public class Settings : ModSettings
    {
        public static string ForcedTraitColor => UseSingleColor ? "#a2ebe9" : null;

        [SettingsCategory("SPS.VisualCat")]
        [TweakValue("Simple Personalities")]
        public static bool UseSingleColor = false;

        [TweakValue("Simple Personalities", 20, 200)]
        public static int ExtraBioHeight = 120;
        [TweakValue("Simple Personalities", 0, 1)]
        public static bool UseWorldviewTab = false;

        [TweakValue("Simple Personalities")]
        [SettingsAdvanced]
        public static bool FreezeDescriptions = false;

        [SettingsCategory("SPS.AnimalCat")]
        [TweakValue("Simple Personalities")]
        public static bool DoAnimalHediffs = true;
        [TweakValue("Simple Personalities")]
        public static bool ShowPersonalityRevealMessage = true;

        [TweakValue("Simple Personalities", 0f, 1f)]
        [SettingsPercentage]
        public static float AnimalPersonalityOnTrainChance = 0.5f;

        [TweakValue("Simple Personalities", 0, 180_000)]
        [SettingsAdvanced]
        public static int AnimalTicksToReveal = 60_000 * 3; // 3 days.

        private static bool showAdvanced;
        private static Vector2 scroll;
        private static Rect viewRect;

        public static List<SettingsEntry> GetAllEntries()
        {
            if (SettingsEntry.all != null)
                return SettingsEntry.all;

            var list = new List<SettingsEntry>();
            foreach (var field in typeof(Settings).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var cat = field.GetCustomAttribute<SettingsCategoryAttribute>();
                if (cat != null)
                {
                    list.Add(new SettingsCategory(cat.GetLabel()));
                }
                var tv = field.GetCustomAttribute<TweakValue>();
                if (tv == null)
                    continue;

                var entry = new SettingsEntry(field);
                entry.TweakValue = tv;
                entry.DefaultValue = field.GetValue(null);
                entry.IsPercentage = field.GetCustomAttribute<SettingsPercentageAttribute>() != null;
                entry.IsAdvanced = field.GetCustomAttribute<SettingsAdvancedAttribute>() != null;

                entry.PreTranslation = $"SPS.{field.Name}";
                list.Add(entry);
            }

            SettingsEntry.all = list;
            return list;
        }

        public override void ExposeData()
        {
            foreach (var item in GetAllEntries())
            {
                if (item.Field == null)
                    continue;

                try
                {
                    item.ExposeData();
                }
                catch (Exception e)
                {
                    Core.Error($"Exception exposing settings data for '{item.Field.Name}':", e);
                }
            }
        }

        public static void DrawUI(Rect area)
        {
            var listingArea = area;
            listingArea.width = Mathf.Min(450, area.width);

            Listing_Standard listing = new Listing_Standard();

            Widgets.BeginScrollView(listingArea, ref scroll, viewRect);
            listing.Begin(new Rect(0, 0, listingArea.width - 24, 99999));

            listing.Gap();
            listing.Label("SPS.Header".Translate());
            listing.CheckboxLabeled("SPS.ShowAdvanced".Translate(), ref showAdvanced, "SPS.ShowAdvancedDesc".Translate());
            GUI.color = Color.Lerp(Color.red, Color.white, 0.4f);
            bool reset = listing.ButtonText("SPS.ResetAll".Translate());
            GUI.color = Color.white;

            foreach (var item in GetAllEntries())
            {
                try
                {
                    if (reset)
                        item.Reset();
                    if (item.IsAdvanced && !showAdvanced)
                        continue;

                    item.Draw(listing);
                }
                catch (Exception e)
                {
                    Core.Error($"Exception drawing settings item '{item.Label}':", e);
                }
            }

            float h = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
            viewRect = new Rect(0, 0, listingArea.width - 25, h);
        }
    }

    public class SettingsEntry
    {
        internal static List<SettingsEntry> all;

        public readonly FieldInfo Field;
        public TweakValue TweakValue;
        public string PreTranslation;
        public string Label;
        public string Description;
        public object DefaultValue;
        public bool IsAdvanced;
        public bool IsPercentage;

        public SettingsEntry(FieldInfo fi)
        {
            this.Field = fi;
        }

        public T GetValue<T>()
        {
            return (T)Field.GetValue(null);
        }

        public T GetDefaultValue<T>()
        {
            return (T)DefaultValue;
        }

        public void SetValue(object obj)
        {
            Field.SetValue(null, obj);
        }

        public virtual void ExposeData()
        {
            if (Field == null)
                return;

            var type = Field.FieldType;
            string key = Field.Name;

            if (type == typeof(int))
            {
                var value = GetValue<int>();
                Scribe_Values.Look(ref value, key, (int)DefaultValue);
                SetValue(value);
                return;
            }
            if (type == typeof(float))
            {
                var value = GetValue<float>();
                Scribe_Values.Look(ref value, key, (float)DefaultValue);
                SetValue(value);
                return;
            }
            if (type == typeof(bool))
            {
                bool value = GetValue<bool>();
                Scribe_Values.Look(ref value, key, (bool)DefaultValue);
                SetValue(value);
                return;
            }

            Core.Error($"Unhandled setting type: {type.FullName}");
        }

        public virtual void Reset()
        {
            if (Field == null)
                return;

            SetValue(DefaultValue);
        }

        public virtual void Draw(Listing_Standard listing)
        {
            if (Field == null)
                return;

            if (Label == null)
            {
                string txt = PreTranslation.Translate();
                if (txt.IndexOf(':') < 0)
                {
                    Core.Error($"No translation for 'SPS.{Field.Name}'!");
                    Label = Field.Name;
                    Description = "No description (missing translation)";
                }
                else
                {
                    string[] split = txt.Split(':');
                    Label = $"<b>{split[0]}</b>";
                    Description = split[1].Trim() + $"\n{"SPS.Default".Translate(IsPercentage ? ((float)DefaultValue * 100f).ToString("F0") + '%' : DefaultValue.ToString())}";
                }
            }

            var type = Field.FieldType;
            bool isChanged = !GetValue<object>().Equals(DefaultValue);
            Rect labelRect = default;

            if (type == typeof(int))
            {
                var value = GetValue<int>();
                var old = value;
                string label = isChanged ? Label + $": <color=yellow>{value}</color>" : Label + $": {value}";
                labelRect = listing.Label(label, tooltip: Description);
                float changed = listing.Slider(value, TweakValue.min, TweakValue.max);
                value = Mathf.RoundToInt(changed);
                if (old != value)
                    SetValue(value);
            }
            else if (type == typeof(float))
            {
                var value = GetValue<float>();
                var old = value;
                string label = isChanged ? Label + $": <color=yellow>{(IsPercentage ? (value * 100f).ToString("F0") + '%' : value.ToString("F1"))}</color>" : Label + $": {(IsPercentage ? (value * 100f).ToString("F0") + '%' : value.ToString("F1"))}";
                labelRect = listing.Label(label, tooltip: Description);
                value = listing.Slider(value, TweakValue.min, TweakValue.max);
                if (old != value)
                    SetValue(value);
            }
            else if (type == typeof(bool))
            {
                bool value = GetValue<bool>();
                var old = value;
                string label = isChanged ? Label + $": <color=yellow>{(value ? "SPS.Yes" : "SPS.No").Translate()}</color>" : Label + $": {(value ? "SPS.Yes" : "SPS.No").Translate()}";
                listing.CheckboxLabeled(label, ref value, Description);
                if (old != value)
                    SetValue(value);
            }

            if (labelRect != default && Mouse.IsOver(labelRect))
            {
                Widgets.DrawHighlight(labelRect);
                if (Input.GetMouseButtonDown(1))
                    Reset();
            }
        }
    }

    public class SettingsCategory : SettingsEntry
    {
        public SettingsCategory(string label)
            : base(null)
        {
            base.Label = label;
        }

        public override void Draw(Listing_Standard listing)
        {
            listing.GapLine();
            listing.Label($"<color=cyan>{Label.Translate()}</color>");
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingsCategoryAttribute : Attribute
    {
        private readonly string label;

        public SettingsCategoryAttribute(string label)
        {
            this.label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public string GetLabel() => label;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingsPercentageAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingsAdvancedAttribute : Attribute
    {
    }
}
