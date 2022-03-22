using System.Collections.Generic;
using System.Xml;
using Verse;

namespace SPM1
{
    public class DescLeaf
    {
        protected static List<(object obj, string key)> tempData = new List<(object obj, string key)>();

        public DescLeafType type = DescLeafType.Grammar;
        public RulePackDef rulePack;
        public string root;
        public List<PartObject> uses = new List<PartObject>();
        public string text;
        public bool onlyIfHasParts = true;

        public virtual string MakeString(Pawn pawn, DescriptionSeed seed)
        {
            switch (type)
            {
                case DescLeafType.FixedString:
                    return text;

                case DescLeafType.Space:
                    return " ";

                case DescLeafType.NewLine:
                    return "\n";

                case DescLeafType.Grammar:
                    var gram = pawn.TryGetEnneagram();
                    if (gram == null)
                        return "ERROR_NO_PERSONALITY";

                    var uses = ResolveUses(pawn, gram);
                    // If uses is null it means that the grammar should not be generated - such as when a
                    // personality trait is missing.
                    // This is not necessarily an error.
                    if (uses == null)
                        return null;

                    if (seed != null)
                    {
                        int rand = seed.NextSeed();
                        Rand.PushState(rand);
                    }

                    string resolved = DescriptionGenerator.MakePart(root, pawn, uses);

                    if(seed != null)
                        Rand.PopState();

                    return resolved;
                default:
                    return null;
            }
        }

        protected virtual List<(object obj, string key)> ResolveUses(Pawn pawn, Enneagram gram)
        {
            tempData.Clear();
            tempData.Add((rulePack, null));
            tempData.Add((pawn, "PAWN"));

            foreach (var item in uses)
            {
                var pair = GetPart(gram, item.key, item.part);
                if (onlyIfHasParts && pair.obj == null)
                    return null;

                tempData.Add(pair);
            }

            return tempData;
        }

        protected (object obj, string key) GetPart(Enneagram gram, string key, Part part)
        {
            return part switch
            {
                Part.Root => (gram.Root, key),
                Part.Drive => (gram.Root.drive, key),
                Part.Variant => (gram.Variant, key),
                Part.MainTrait => (gram.MainTrait, key),
                Part.SecondaryTrait => (gram.SecondaryTrait, key),
                Part.OptionalTrait => (gram.OptionalTrait, key),
                _ => (null, null)
            };
        }

        public enum DescLeafType
        {
            Grammar,
            Space,
            NewLine,
            FixedString
        }

        public enum Part
        {
            Root,
            Drive,
            Variant,
            MainTrait,
            SecondaryTrait,
            OptionalTrait
        }

        public class PartObject
        {
            public Part part;
            public string key;

            public void LoadDataFromXmlCustom(XmlNode node)
            {
                part = ParseHelper.FromString<Part>(node.Name);
                key = node.InnerText;
            }
        }
    }
}
