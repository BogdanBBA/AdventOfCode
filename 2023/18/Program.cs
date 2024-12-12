using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AoC18
{
    using Position = (int row, int col);

    public static class Utils
    {
        public static char ToChar(this Direction direction)
            => (char)direction;

        public static Rule ParseAsRule(this string line)
        {
            Match match = Regex.Match(line, @"(\w) (\d+) \((.+)\)");
            return new((Direction)match.Groups[1].Value[0], int.Parse(match.Groups[2].Value), ColorTranslator.FromHtml(match.Groups[3].Value));
        }

        public static Position GetNewPosition(this Position position, Direction direction, int meters = 1)
        {
            return direction switch
            {
                Direction.U => (position.row - meters, position.col),
                Direction.D => (position.row + meters, position.col),
                Direction.L => (position.row, position.col - meters),
                Direction.R => (position.row, position.col + meters),
                _ => throw new ArgumentException($"Unknown direction {direction}!")
            };
        }

        public static bool PositionIsValid<TYPE>(this TYPE[][] matrix, Position position)
            => position.row >= 0 && position.col >= 0 && position.row < matrix.Length && position.col < matrix[position.row].Length;

        public static bool PositionShouldBeFilled(this bool[][] matrix, Position position)
            => matrix.PositionIsValid(position) && !matrix[position.row][position.col];

        public static void EnqueueNeighbourIfItShouldBeFilled(this Queue<Position> queue, Position position, Direction direction, bool[][] matrix)
        {
            Position neighbour = GetNewPosition(position, direction);
            if (matrix.PositionShouldBeFilled(neighbour) && !queue.Contains(neighbour))
                queue.Enqueue(neighbour);
        }

        public static void Fill(this bool[][] matrix, Position startPosition)
        {
            if (!matrix.PositionShouldBeFilled(startPosition))
                return;

            Queue<Position> queue = [];
            queue.Enqueue(startPosition);
            while (queue.Count > 0)
            {
                Position position = queue.Dequeue();
                if (!matrix.PositionShouldBeFilled(position))
                    continue;
                matrix[position.row][position.col] = true;
                queue.EnqueueNeighbourIfItShouldBeFilled(position, Direction.U, matrix);
                queue.EnqueueNeighbourIfItShouldBeFilled(position, Direction.D, matrix);
                queue.EnqueueNeighbourIfItShouldBeFilled(position, Direction.L, matrix);
                queue.EnqueueNeighbourIfItShouldBeFilled(position, Direction.R, matrix);
            }
        }
    }

    public enum Direction { U = 'U', D = 'D', L = 'L', R = 'R' }

    public class Rule(Direction direction, int meters, Color color)
    {
        public Direction Direction { get; private set; } = direction;
        public int Meters { get; private set; } = meters;
        public Color Color { get; private set; } = color;

        public override string ToString()
            => $"{Direction.ToChar()} {Meters} ({Color})";
    }

    public class Program
    {
        public static List<Position> GetNewPositions(Position position, Rule rule)
            => Enumerable.Range(1, rule.Meters).Select(meters => position.GetNewPosition(rule.Direction, meters)).ToList();

        private static int GetPoolCapacity(List<Rule> rules, Position manualFillStartPosition, bool print = true)
        {
            List<Position> positions = [(0, 0)];
            for (int iRule = 0; iRule < rules.Count; iRule++)
                positions.AddRange(GetNewPositions(positions.Last(), rules[iRule]));
            int rowUpmost = positions.Min(pos => pos.row), rowDownmost = positions.Max(pos => pos.row), height = rowDownmost - rowUpmost + 1;
            int colLeftmost = positions.Min(pos => pos.col), colRightmost = positions.Max(pos => pos.col), width = colRightmost - colLeftmost + 1;
            bool[][] matrix = new bool[height][];
            for (int iRow = 0; iRow < matrix.Length; iRow++)
                matrix[iRow] = new bool[width];
            positions.ForEach(position => matrix[position.row - rowUpmost][position.col - colLeftmost] = true);
            if (print) File.WriteAllText(@"out-lines.txt", $"{string.Join("\n", matrix.Select(row => string.Join(null, row.Select(cell => cell ? '#' : '.'))))}\n");
            matrix.Fill(manualFillStartPosition);
            if (print) File.WriteAllText(@"out-dashs.txt", $"{string.Join("\n", matrix.Select(row => string.Join(null, row.Select(cell => cell ? '#' : '.'))))}\n");
            return matrix.SelectMany(row => row).Count(pos => pos);
        }

        private static void Main(string[] _)
        {
            bool testInput = false;
            DateTime startMoment = DateTime.Now;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            List<Rule> rules = File.ReadAllLines(testInput ? "input0.txt" : "input1.txt").Select(line => line.ParseAsRule()).ToList();
            Console.WriteLine($" > The capacity of the lava pool is: {GetPoolCapacity(rules, testInput ? (1, 1) : (105, 1))} cubic meters");
            Console.WriteLine($" > Processing took {DateTime.Now.Subtract(startMoment).TotalSeconds:N2} seconds");
        }
    }
}