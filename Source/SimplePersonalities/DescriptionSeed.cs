using System.Collections.Generic;
using System.Text;
using Verse;

namespace SPM1
{
    public class DescriptionSeed : IExposable
    {
        public static readonly DescriptionSeed Default = new DescriptionSeed();
        private static StringBuilder str = new StringBuilder();

        public bool IsUniform
        {
            get
            {
                if(Seeds == null || Seeds.Count == 0)
                    return false;

                int last = Seeds[0];
                foreach(var seed in Seeds)
                {
                    if (seed != last)
                        return false;
                    last = seed;
                }
                return true;
            }
        }
        public List<int> Seeds = new List<int>();

        private int currentIndex;

        public DescriptionSeed() : this(4, null)
        {

        }

        public DescriptionSeed(int toGen, int? forceTo)
        {
            for (int i = 0; i < toGen; i++)
            {
                Seeds.Add(forceTo ?? Rand.Range(0, 10_000));
            }
        }

        /// <summary>
        /// Note: Not used internally, only for external mod support such as Character editor or altered carbon.
        /// </summary>
        public DescriptionSeed(string saveString)
        {
            if (string.IsNullOrWhiteSpace(saveString))
                throw new System.ArgumentException(nameof(saveString), "Save string must not be null or blank.");

            string[] split = saveString.Split(',');
            int count = int.Parse(split[0]);
            for (int i = 0; i < count; i++)
            {
                Seeds.Add(int.Parse(split[i + 1]));
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref Seeds, "seeds", LookMode.Value);
            Seeds ??= new List<int>();
        }

        public void ResetReader()
        {
            currentIndex = 0;
        }

        public int NextSeed()
        {
            if (Seeds == null || Seeds.Count <= currentIndex)
                return 0;

            return Seeds[currentIndex++];
        }

        /// <summary>
        /// Note: Not used internally, only for external mod support such as Character editor or altered carbon.
        /// </summary>
        public string ToSaveString()
        {
            str.Clear();
            str.Append(Seeds.Count).Append(',');
            for (int i = 0; i < Seeds.Count; i++)
            {
                str.Append(Seeds[i]);
                if (i != Seeds.Count - 1)
                    str.Append(',');
            }
            return str.ToString();
        }
    }
}
