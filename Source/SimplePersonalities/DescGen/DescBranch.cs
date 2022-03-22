using System.Collections.Generic;
using System.Text;
using Verse;

namespace SPM1
{
    public class DescBranch
    {
        protected static StringBuilder str = new StringBuilder();

        public List<DescLeaf> leaves = new List<DescLeaf>();

        public virtual bool Matches(Pawn pawn)
        {
            return true;
        }

        public virtual string MakeString(Pawn pawn, DescriptionSeed seed)
        {
            str.Clear();
            foreach (var leaf in leaves)
            {
                string s = leaf.MakeString(pawn, seed);
                if (s != null)
                    str.Append(s);
            }
            return str.ToString();
        }
    }
}
