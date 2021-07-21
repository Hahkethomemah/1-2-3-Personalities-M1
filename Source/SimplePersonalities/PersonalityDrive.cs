using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace SPM1
{
    public class PersonalityDrive : Def, IRuleSupplier
    {
        public string color;

        public IEnumerable<Rule> GetRules(string prefix, Pawn pawn)
        {
            foreach (var rule in GrammarUtility.RulesForDef(prefix, this))
                yield return rule;

            prefix += '_';
            yield return new Rule_String(prefix + "labelCol", $"<color={Settings.ForcedTraitColor ?? color}>{label}</color>");
        }
    }
}
