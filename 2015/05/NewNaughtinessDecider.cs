namespace AoC2015.Day05
{
    using NewReasoning = (string String, bool IsNice, NewNaughtinessDecider.NaughtyReason Reason);

    public static class NewNaughtinessDecider
    {
        public enum NaughtyReason { None, NoRepeatingLetterSpacedExactly1Apart, NoRepeatingLetterPairWithoutOverlap }

        public static NewReasoning DetermineNaughtiness(string s)
        {
            bool repeatingLetter = false;
            for (int i = 0; i < s.Length - 2; i++)
            {
                if (s[i] == s[i + 2])
                {
                    repeatingLetter = true;
                    break;
                }
            }
            if (!repeatingLetter)
                return (s, false, NaughtyReason.NoRepeatingLetterSpacedExactly1Apart);

            bool repeatingLetterPair = false;
            for (int i = 0; i < s.Length - 1; i++)
            {
                string sub = s.Substring(i, 2);
                for (int j = i + 2; j < s.Length - 1; j++)
                {
                    string jSub = s.Substring(j, 2);
                    if (sub == jSub)
                    {
                        repeatingLetterPair = true;
                        break;
                    }
                }
            }
            if (!repeatingLetterPair)
                return (s, false, NaughtyReason.NoRepeatingLetterPairWithoutOverlap);
            return (s, true, NaughtyReason.None);
        }
    }
}