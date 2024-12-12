namespace AoC2015.Day05
{
    using OldReasoning = (string String, bool IsNice, OldNaughtinessDecider.NaughtyReason Reason);

    public static class OldNaughtinessDecider
    {
        public enum NaughtyReason { None, NoDoubleLetter, DisallowedLetterPairs, LessThan3Vowels }

        private static readonly char[] VOWELS = ['a', 'e', 'i', 'o', 'u'];
        private static readonly string[] DISALLOWED = ["ab", "cd", "pq", "xy"];

        private static bool IsVowel(char character)
            => VOWELS.Any(vowel => vowel == character);

        public static OldReasoning DetermineNaughtiness(string s)
        {
            bool doubleLetterAppeared = false;
            int vowelCount = 0;

            if (IsVowel(s[0])) vowelCount++;
            for (int i = 0; i < s.Length - 1; i++)
            {
                char c = s[i], n = s[i + 1];
                if (IsVowel(n)) // second!
                    vowelCount++;
                if (c == n)
                    doubleLetterAppeared = true;
                string pair = $"{c}{n}";
                if (DISALLOWED.Any(d => d == pair))
                    return (s, false, NaughtyReason.DisallowedLetterPairs);
            }
            if (!doubleLetterAppeared)
                return (s, false, NaughtyReason.NoDoubleLetter);
            if (vowelCount < 3)
                return (s, false, NaughtyReason.LessThan3Vowels);
            return (s, true, NaughtyReason.None);
        }
    }
}