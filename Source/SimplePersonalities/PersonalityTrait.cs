using System.Collections.Generic;
using System.Xml.Serialization;
using Verse;
using Verse.Grammar;

namespace SPM1
{
    /// <summary>
    /// Corresponds to a trait from the traits pool.
    /// Each <see cref="PersonalityRoot"/> has a list of these traits.
    /// </summary>
    public class PersonalityTrait : Def, IRuleSupplier
    {
        public string LabelColor => $"<color={Settings.ForcedTraitColor ?? Root.color}>{label}</color>";

        public List<PersonalityVariant> incompatibleVariants;

        [XmlIgnore]
        public PersonalityRoot Root;

        public virtual bool IsCompatibleWith(PersonalityVariant variant)
        {
            if (variant == null)
                return true;

            if (incompatibleVariants == null || incompatibleVariants.Count == 0)
                return true;

            foreach (var item in incompatibleVariants)
            {
                if (item == variant)
                    return false;
            }
            return true;
        }

        public IEnumerable<Rule> GetRules(string prefix, Pawn pawn)
        {
            foreach (var item in GrammarUtility.RulesForDef(prefix, this))
                yield return item;

            prefix += '_';
            yield return new Rule_String(prefix + "labelCol", LabelColor);
        }

        public override string ToString()
        {
            return $"{LabelCap}";
        }
    }
}
