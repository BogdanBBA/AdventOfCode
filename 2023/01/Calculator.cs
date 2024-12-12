namespace AdventOfCode01
{
    using System.Text.RegularExpressions;


    internal class Calculator
    {
        public static Dictionary<string, string> Mapping = new()
    {
        { "one", "1" },
        { "two", "2" },
        { "three", "3" },
        { "four", "4" },
        { "five", "5" },
        { "six", "6" },
        { "seven", "7" },
        { "eight", "8" },
        { "nine", "9"}
    };
        public const string Pattern = "(1|2|3|4|5|6|7|8|9|one|two|three|four|five|six|seven|eight|nine)?";

        // private static string ReplaceFancy(string line)
        // {
        //     var sortedByStart = Mapping.Keys.Select(key => (key, line.IndexOf(key))).Where(pair => pair.Item2 >= 0).OrderBy(pair => pair.Item2).ToArray();
        //     if (sortedByStart.Length == 0)
        //         return new string(line);
        //     int replacedLength = sortedByStart[0].key.Length;
        //     string withFirstReplaced = line.Replace(sortedByStart[0].key, Mapping[sortedByStart[0].key]);
        //     if (sortedByStart.Length == 1)
        //         return withFirstReplaced;
        //     var sortedDescByEnd = sortedByStart.OrderByDescending(pair => pair.Item2).ToArray();
        //     return withFirstReplaced.Replace(sortedDescByEnd[0].key, Mapping[sortedDescByEnd[0].key]);
        // }
        // public int ProcessLine(string line)
        // {
        //     var processedLine = ReplaceFancy(line);
        //     var matches = Regex.Matches(processedLine, @"\d");
        //     var result = int.Parse($"{matches.First().Value}{matches.Last().Value}");
        //     Console.WriteLine($"{line}: {result}");
        //     return result;
        // }

        private string ReplaceAllKeys(string line, int keyIndex = 0)
        {
            //Console.WriteLine($"{string.Join("", Enumerable.Range(0, keyIndex).Select(_ => "."))}{line}");
            if (keyIndex >= Mapping.Keys.Count) return line;
            string key = Mapping.Keys.ElementAt(keyIndex);
            return ReplaceAllKeys(line.Replace(key, Mapping[key]), keyIndex + 1);
        }

        public int ProcessLine2(string line)
        {
            MatchCollection matches = Regex.Matches(line, Pattern);
            string digits = $"{ReplaceAllKeys(matches.First().Value)}{ReplaceAllKeys(matches.Last().Value)}";
            int.TryParse(digits, out int result);
            Console.WriteLine($"Line '{line}' -> '{digits}' ({matches.Count} match(es)) -> {result}");
            return result;
        }
    }
}