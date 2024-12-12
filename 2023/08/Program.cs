using System.Globalization;
using System.Text.RegularExpressions;

namespace AoC08
{
    internal class Node(string left, string right)
    {
        public string LeftName { get; } = left;
        public string RightName { get; } = right;

        public string this[bool left] => left ? LeftName : RightName;

        public override string ToString() => $"({LeftName}, {RightName})";
    }

    internal class Map(List<bool> navigationSequence, Dictionary<string, Node> nodes)
    {
        public List<bool> NavigationSequence { get; } = navigationSequence;
        public Dictionary<string, Node> Nodes { get; } = nodes;

        public (string EndNodeName, int StepCount) CountSteps(string fromName, Predicate<string> toNamePredicate, int sequenceOffset = 0)
        {
            int steps = 0;
            string current = fromName;
            Queue<bool> sequence = new();

            if (sequenceOffset != 0)
            {
                NavigationSequence.Skip(sequenceOffset % NavigationSequence.Count).ToList()
                    .ForEach(sequence.Enqueue);
            }

            do
            {
                if (sequence.Count == 0)
                {
                    NavigationSequence.ForEach(sequence.Enqueue);
                }

                steps++;
                current = Nodes[current][sequence.Dequeue()];
            }
            while (!toNamePredicate(current));

            return (current, steps);
        }

        public ulong CountStepsAsGhost()
        {
            static ulong CMMaDivC(ulong a, ulong b)
            {
                while (b != 0)
                {
                    ulong temp = b;
                    b = a % b;
                    a = temp;
                }
                return a;
            };

            static ulong CMMiMultC(ulong a, ulong b)
                => a * b / CMMaDivC(a, b);

            List<string> startNodes = Nodes.Keys.Where(name => name.EndsWith('A')).ToList();
            List<ulong> cycleLengths = startNodes.Select(startNode => (ulong)CountSteps(startNode, name => name.EndsWith('Z')).StepCount).ToList();
            ulong cmmmc = cycleLengths.Aggregate(1UL, CMMiMultC);
            Console.WriteLine(string.Join("\n", Enumerable.Range(0, startNodes.Count).Select(i => $"   {startNodes[i]} has a cycle length of {cycleLengths[i]:N0} steps")));
            Console.WriteLine($" > As such, the result (the smallest common multiple of the cycle lengths, meaning the point when all cycled nodes' names will end in a Z) is {cmmmc:N0}.");
            return cmmmc;
        }
    }

    internal class Program
    {
        private static Map ParseMap(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            return new Map(
                lines.First().Select(character => character == 'L').ToList(),
                lines.Skip(2).Select(line =>
                {
                    Match match = Regex.Match(line, @"(\w{3}) = \((\w{3}), (\w{3})\)");
                    (string name, string left, string right) = (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
                    return (name, left, right);
                })
                .ToDictionary(set => set.name, set => new Node(set.left, set.right))
            );
        }

        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Map map = ParseMap("input1.txt");
            Console.WriteLine($" > Map contains {map.Nodes.Count} nodes and a navigation instruction sequence of {map.NavigationSequence.Count} steps, starting with {string.Join(null, map.NavigationSequence.Take(10).Select(l => l ? "L" : "R"))}.");
            if (map.Nodes.ContainsKey("AAA")) Console.WriteLine($" > Navigating from AAA to ZZZ would require {map.CountSteps("AAA", name => name == "ZZZ").StepCount} steps, as per the initial algorithm.");
            Console.WriteLine($" > Navigating from AAA to ZZZ actually requires {map.CountStepsAsGhost()} steps, as a ghost.");
        }
    }
}