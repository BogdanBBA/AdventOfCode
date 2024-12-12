namespace AdventOfCode01
{
    internal class Calculator2
    {
        public static Dictionary<string, int> Mapping = new()
        {
            { "1", 1 },
            { "one", 1 },
            { "2", 2 },
            { "two", 2 },
            { "3", 3 },
            { "three", 3 },
            { "4", 4 },
            { "four", 4 },
            { "5", 5 },
            { "five", 5 },
            { "6", 6 },
            { "six", 6 },
            { "7", 7 },
            { "seven", 7 },
            { "8", 8 },
            { "eight", 8 },
            { "9", 9},
            { "nine", 9}
        };

        private static string GetKey(string line, int startIndex, Func<int, bool> condition, Func<int, int> iterate, string errorKeyDescription)
        {
            string[] keysList = Mapping.Keys.ToArray();
            for (int iC = startIndex; condition(iC); iC = iterate(iC))
            {
                for (int iK = 0; iK < keysList.Length; iK++)
                {
                    if (line[iC..].StartsWith(keysList[iK]))
                        return keysList[iK];
                }
            }
            throw new Exception($"No {errorKeyDescription} key found for line '{line}'!");
        }

        public static int ProcessLine(string line)
        {
            string firstKey = GetKey(line, 0, index => index < line.Length, index => index + 1, "first");
            string lastKey = GetKey(line, line.Length - 1, index => index >= 0, index => index - 1, "last");
            int result = int.Parse($"{Mapping[firstKey]}{Mapping[lastKey]}");
            Console.WriteLine($"Line '{line}': '{firstKey}' and '{lastKey}' => {result};");
            return result;
        }
    }
}