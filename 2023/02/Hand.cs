using System.Text.RegularExpressions;

namespace AdventOfCode02
{
    internal class Hand
    {
        private static (Cube cube, int count) ParseCubeDescription(string text)
        {
            Match match = Regex.Match(text, @"(\d+)\s+(\w+)");
            return (Cube.ParseCube(match.Groups[2].Value), int.Parse(match.Groups[1].Value));
        }

        public CubeCount CubeCount { get; private set; }

        public Hand(string text)
        {
            CubeCount = CubeCount.ParseFromDictionary(Cube.AllCubes.ToDictionary(cube => cube, _ => 0));

            CubeCount parsedCubeCounts = CubeCount.ParseFromDictionary(text.Split(',').Select(ParseCubeDescription).ToDictionary());
            parsedCubeCounts.Keys.ToList().ForEach(key => CubeCount[key] = parsedCubeCounts[key]);
        }

        public bool ExceedsLimits(CubeCount limits) => limits.Keys.Any(cube => CubeCount[cube] > limits[cube]);
    }
}