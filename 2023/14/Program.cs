using System.Globalization;

using Rock = (int Row, int Col);

namespace AoC14
{
    public class Map
    {
        public const char CHAR_SPACE = '░';
        public const char CHAR_CUBE = '█';
        public const char CHAR_ROUND = '●';

        public (int Rows, int Columns) Size { get; private set; }
        public HashSet<Rock> CubeRocks { get; private set; }
        public HashSet<Rock> RoundRocks { get; private set; }

        public Map(Map other)
        {
            Size = other.Size;
            CubeRocks = new(other.CubeRocks);
            RoundRocks = new(other.RoundRocks);
        }

        public Map(string inputFile)
        {
            string[] lines = File.ReadAllLines(inputFile);
            Size = (lines.Length, lines[0].Length);
            CubeRocks = [];
            RoundRocks = [];
            for (int iRow = 0; iRow < lines.Length; iRow++)
            {
                for (int iCol = 0; iCol < lines[iRow].Length; iCol++)
                {
                    if (lines[iRow][iCol] == '#') CubeRocks.Add(new Rock(iRow, iCol));
                    if (lines[iRow][iCol] == 'O') RoundRocks.Add(new Rock(iRow, iCol));
                }
            }
        }

        public bool SameRoundRocksAs(Map other)
            => RoundRocks.SetEquals(other.RoundRocks);

        private enum Side { Above, Below, ToLeftOf, ToRightOf }

        private int PlaceRocksAndReturn0(Side side, Rock anchor, int count)
        {
            if (count <= 0)
                return 0;
            switch (side)
            {
                case Side.Above:
                    for (int row = anchor.Row - 1; row >= anchor.Row - count; row--)
                        RoundRocks.Add((row, anchor.Col));
                    return 0;
                case Side.Below:
                    for (int row = anchor.Row + 1; row <= anchor.Row + count; row++)
                        RoundRocks.Add((row, anchor.Col));
                    return 0;
                case Side.ToLeftOf:
                    for (int col = anchor.Col - 1; col >= anchor.Col - count; col--)
                        RoundRocks.Add((anchor.Row, col));
                    return 0;
                case Side.ToRightOf:
                    for (int col = anchor.Col + 1; col <= anchor.Col + count; col++)
                        RoundRocks.Add((anchor.Row, col));
                    return 0;
                default:
                    throw new ArgumentException($"Side '{side}' is invalid!");
            }
        }

        public void TiltNorth()
        {
            for (int iCol = 0; iCol < Size.Columns; iCol++)
            {
                int rocksPickedUp = 0;
                for (int iRow = Size.Rows - 1; iRow >= 0; iRow--)
                {
                    Rock rock = (iRow, iCol);
                    if (RoundRocks.Remove(rock))
                        rocksPickedUp++;
                    else if (CubeRocks.Contains(rock))
                        rocksPickedUp = PlaceRocksAndReturn0(Side.Below, rock, rocksPickedUp);
                }
                PlaceRocksAndReturn0(Side.Below, (-1, iCol), rocksPickedUp);
            }
        }

        public void TiltSouth()
        {
            for (int iCol = 0; iCol < Size.Columns; iCol++)
            {
                int rocksPickedUp = 0;
                for (int iRow = 0; iRow < Size.Rows; iRow++)
                {
                    Rock rock = (iRow, iCol);
                    if (RoundRocks.Remove(rock))
                        rocksPickedUp++;
                    else if (CubeRocks.Contains(rock))
                        rocksPickedUp = PlaceRocksAndReturn0(Side.Above, rock, rocksPickedUp);
                }
                PlaceRocksAndReturn0(Side.Above, (Size.Rows, iCol), rocksPickedUp);
            }
        }

        public void TiltWest()
        {
            for (int iRow = 0; iRow < Size.Rows; iRow++)
            {
                int rocksPickedUp = 0;
                for (int iCol = Size.Columns - 1; iCol >= 0; iCol--)
                {
                    Rock rock = (iRow, iCol);
                    if (RoundRocks.Remove(rock))
                        rocksPickedUp++;
                    else if (CubeRocks.Contains(rock))
                        rocksPickedUp = PlaceRocksAndReturn0(Side.ToRightOf, rock, rocksPickedUp);
                }
                PlaceRocksAndReturn0(Side.ToRightOf, (iRow, -1), rocksPickedUp);
            }
        }

        public void TiltEast()
        {
            for (int iRow = 0; iRow < Size.Rows; iRow++)
            {
                int rocksPickedUp = 0;
                for (int iCol = 0; iCol < Size.Columns; iCol++)
                {
                    Rock rock = (iRow, iCol);
                    if (RoundRocks.Remove(rock))
                        rocksPickedUp++;
                    else if (CubeRocks.Contains(rock))
                        rocksPickedUp = PlaceRocksAndReturn0(Side.ToLeftOf, rock, rocksPickedUp);
                }
                PlaceRocksAndReturn0(Side.ToLeftOf, (iRow, Size.Columns), rocksPickedUp);
            }
        }

        public double DoCycle()
        {
            DateTime start = DateTime.Now;
            TiltNorth();
            TiltWest();
            TiltSouth();
            TiltEast();
            return DateTime.Now.Subtract(start).TotalSeconds;
        }

        public double DoCycles(int cycleCount)
        {
            Console.WriteLine($" > Running {cycleCount:N0} tilt cycles...");
            double seconds = 0.0;
            for (int iCycle = 0; iCycle < cycleCount; iCycle++)
            {
                seconds += DoCycle();
                if (iCycle % 5000 == 0)
                {
                    double secondsRemaining = seconds / iCycle * (cycleCount - iCycle);
                    Console.WriteLine($" - {iCycle,13:N0} / {cycleCount,13:N0} ({(double)iCycle / cycleCount,9:P3} in {seconds,8:N1}s, {secondsRemaining,6:N0}s / {secondsRemaining / 3600,3:N1}h remaining)");
                }
            }
            Console.WriteLine($" - {cycleCount,13:N0} / {cycleCount,13:N0} ({(double)cycleCount / cycleCount,9:P3} in {seconds,8:N1}s, done)");
            return seconds;
        }

        public string RoundRocksAsString
            => string.Join("; ", RoundRocks.OrderBy(x => x.Row).ThenBy(x => x.Col).Select(x => $"{x.Row},{x.Col}"));

        public (int FirstCount, int SecondCount) DetermineFirstRepeatingCycle()
        {
            Dictionary<int, Map> states = [];
            states.Add(-1, new(this));
            for (int cycle = 1; true; cycle++)
            {
                DoCycle();
                KeyValuePair<int, Map> iveSeenThisStateBefore = states.FirstOrDefault(previousState => previousState.Value.SameRoundRocksAs(this));
                if (iveSeenThisStateBefore.Value is not null)
                    return (iveSeenThisStateBefore.Key, cycle);
                states.Add(cycle, new(this));
            }
        }

        public int GetTotalLoad()
            => RoundRocks.Select(rock => Size.Rows - rock.Row).Sum();

        public void Print(string title)
        {
            Console.WriteLine($" > Map ({Size.Rows} rows x {Size.Columns} columns; {CubeRocks.Count} cube rocks and {RoundRocks.Count} round rocks): {title}");
            Console.WriteLine($"    {string.Join(string.Empty, Enumerable.Range(0, Size.Columns).Select(col => col % 10))}");

            char[][] mapLines = Enumerable.Range(0, Size.Rows).Select(_ => new string(CHAR_SPACE, Size.Columns).ToCharArray()).ToArray();
            CubeRocks.ToList().ForEach(rock => mapLines[rock.Row][rock.Col] = CHAR_CUBE);
            RoundRocks.ToList().ForEach(rock => mapLines[rock.Row][rock.Col] = CHAR_ROUND);

            for (int iRow = 0, loadNumber = Size.Rows; iRow < Size.Rows; iRow++, loadNumber--)
                Console.WriteLine($"{iRow,3} {new string(mapLines[iRow])} {loadNumber,3}");
            Console.WriteLine($" > Total load: {GetTotalLoad()} ({title})\n");
        }
    }

    internal class Program
    {
        private const int OneBillion = 1_000_000_000;
        private const bool AlsoDoPart2 = true;

        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Map map0original = new("input0.txt"), map1original = new("input1.txt"), map;

            map = new(map0original);
            map.Print("original state");
            map.TiltNorth();
            map.Print("after tilting north only");
            map.TiltWest();
            map.Print("after tilting north and then west");
            map.TiltSouth();
            map.Print("after tilting north, west and then south");
            map.TiltEast();
            map.Print("after tilting north, west, south and then east (one cycle)");
            Console.WriteLine($" > State is same as original: {map.SameRoundRocksAs(map0original).ToString().ToUpperInvariant()}\n");

            if (AlsoDoPart2)
            {
                Console.WriteLine($" > Processing the full input...");
                map = new(map1original);
                (int FirstCount, int SecondCount) = map.DetermineFirstRepeatingCycle();
                map = new(map1original);
                int cycleCycleLength = SecondCount - FirstCount;
                int skipCycleCount = (OneBillion - FirstCount) / cycleCycleLength;
                int remainingCount = (OneBillion - FirstCount) % cycleCycleLength;
                Console.WriteLine($" > Trick: map state repeats between cycle counts {FirstCount:N0} and {SecondCount:N0} (a cycle cycle length of {cycleCycleLength})!");
                Console.WriteLine($" > Running first {FirstCount:N0} cycles, skipping {skipCycleCount:N0} cycle cycles (that is {skipCycleCount * cycleCycleLength:N0} cycles) and then running remaining {remainingCount:N0} cycles ...");
                double seconds = map.DoCycles(FirstCount) + map.DoCycles(remainingCount);
                map.Print($"after doing all the cycles, having taken {seconds:N3}s");
            }
        }
    }
}