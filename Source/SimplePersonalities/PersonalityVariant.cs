using System.Collections.Generic;
using System.Xml.Serialization;
using Verse;
using Verse.Grammar;

namespace SPM1
{
    /// <summary>
    /// Corresponds to 
    /// </summary>
    public class PersonalityVariant : Def, IRuleSupplier
    {
        [XmlIgnore]
        public PersonalityRoot Root;

        public List<string> genderReplace = null;

        public IEnumerable<Rule> GetRules(string prefix, Pawn pawn)
        {
            foreach (var item in GrammarUtility.RulesForDef(prefix, this))
                yield return item;

            string lab = label;
            if (genderReplace != null && genderReplace.Count > 0)
            {
                // Male and None use male variant.
                bool isMale = pawn.gender != Gender.Female;
                lab = PersonalityUtils.ReplaceGendered(lab, genderReplace, isMale);
            }

            prefix += '_';
            yield return new Rule_String(prefix + "labelCol", $"<color={Settings.ForcedTraitColor ?? Root.color}>{lab}</color>");
        }

        public override string ToString()
        {
            return $"{LabelCap}";
        }
    }
}
