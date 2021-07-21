using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace SPM1
{
    public interface IRuleSupplier
    {
        public IEnumerable<Rule> GetRules(string prefix, Pawn pawn);
    }
}
