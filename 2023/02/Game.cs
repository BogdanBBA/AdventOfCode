using System.Text.RegularExpressions;

namespace AdventOfCode02
{
    internal class Game
    {
        public int ID { get; private set; }
        public List<Hand> Hands { get; private set; }

        public Game(string line)
        {
            string[] parts = line.Split(":");
            ID = int.Parse(Regex.Match(parts[0], @"\d+").Value);
            Hands = parts[1].Split(';').Select(handS => new Hand(handS)).ToList();
        }

        public bool IsPossible(CubeCount limits) => !Hands.Any(hand => hand.ExceedsLimits(limits));

        public CubeCount GetMinimumCubeCounts() => CubeCount.ParseFromDictionary(Cube.AllCubes.Select(cube => (cube, Hands.Max(hand => hand.CubeCount[cube]))).ToDictionary());

        public int GetPowerOfMinimumCubeCounts() => GetMinimumCubeCounts().Values.Aggregate((accumulator, number) => accumulator * number);
    }
}