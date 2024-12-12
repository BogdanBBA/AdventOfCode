using System.Text.RegularExpressions;

namespace AoC2015.Day09
{
    public class Map(List<(string, string, int)> distances)
    {
        private readonly Dictionary<(string From, string To), int> _distances = distances.ToDictionary(tuple => (tuple.Item1, tuple.Item2), tuple => tuple.Item3);

        public readonly string[] Cities = [.. distances.SelectMany(tuple => new[] { tuple.Item1, tuple.Item2 }).Distinct().OrderBy(city => city)];

        public int this[string fromCity, string toCity]
        {
            get
            {
                if (_distances.ContainsKey((fromCity, toCity)))
                    return _distances[(fromCity, toCity)];
                return _distances[(toCity, fromCity)];
            }
        }

        public void PrintDistances()
            => Console.WriteLine($" > Distances ({_distances.Count}):\n{string.Join("\n", _distances.Select(t => $" - {t.Key.From}→{t.Key.To}: {t.Value}"))}\n");

        public int CalculateRouteLength(string[] route, bool log = false)
        {
            int[] distances = Enumerable.Range(0, route.Length - 1)
                .Select(index => (route[index], route[index + 1]))
                .Select(pair => this[pair.Item1, pair.Item2])
                .ToArray();
            int distanceSum = distances.Sum();
            if (log) Console.WriteLine($" - {string.Join("→", route)}: {string.Join("+", distances)}={distanceSum}");
            return distanceSum;
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 9;
        public override int GetCurrentPart => 1;

        private static (string, string, int) ParseLine(string line)
        {
            Match match = Regex.Match(line, @"(\w+) to (\w+) = (\d+)");
            return (match.Groups[1].Value, match.Groups[2].Value, int.Parse(match.Groups[3].Value));
        }

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            Map map = new(lines.Select(ParseLine).ToList());
            map.PrintDistances();

            string[][] cityPermutations = UtilsOther.GeneratePermutations(map.Cities);
            Console.WriteLine($" > Processing {cityPermutations.Length} city permutations...");
            int[] distances = cityPermutations.Select(route => map.CalculateRouteLength(route)).ToArray();
            Console.WriteLine($" > Result (part 1): the minimum route is {distances.Min()} in length");
            Console.WriteLine($" > Result (part 2): the minimum route is {distances.Max()} in length");
        }
    }
}