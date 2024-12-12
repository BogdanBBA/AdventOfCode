namespace AoC16
{
    using Position = (int Row, int Col);
    using Direction = (int dRow, int dCol);

    public static class Utils
    {
        public static Cell.Kinds ToCellKind(this char character)
            => (Cell.Kinds)character;

        public static char ToCharacter(this Cell.Kinds kind)
            => (char)kind;

        public static char ToCharacter(this Cell cell)
            => cell.Kind.ToCharacter();

        public static char ToEnergizedOrNot(this Cell cell)
            => cell.Energized ? '#' : '.';

        public static bool IsInsideGrid(this Position position, int gridSize)
            => position.Row >= 0 && position.Col >= 0 && position.Row < gridSize && position.Col < gridSize;

        public static bool IsHorizontal(this Direction direction)
            => direction == Beam.Rightward || direction == Beam.Leftward;

        public static bool IsVertical(this Direction direction)
            => direction == Beam.Downward || direction == Beam.Upward;

        public static Direction GetNewDirectionByMirror(Cell.Kinds kind, Direction direction)
        {
            if (kind != Cell.Kinds.MirrorFwd && kind != Cell.Kinds.MirrorBwd)
                throw new ArgumentException($"Cell kind '{kind}' not allowed here!");

            if (direction == Beam.Rightward)
                return kind == Cell.Kinds.MirrorFwd ? Beam.Downward : Beam.Upward;
            if (direction == Beam.Leftward)
                return kind == Cell.Kinds.MirrorFwd ? Beam.Upward : Beam.Downward;
            if (direction == Beam.Downward)
                return kind == Cell.Kinds.MirrorFwd ? Beam.Rightward : Beam.Leftward;
            if (direction == Beam.Upward)
                return kind == Cell.Kinds.MirrorFwd ? Beam.Leftward : Beam.Rightward;

            throw new ArgumentException($"What direction even is '{direction}'?!");
        }
    }

    public class Cell(Position position, Cell.Kinds kind)
    {
        public enum Kinds { Space = '.', MirrorFwd = '\\', MirrorBwd = '/', SplitterV = '|', SplitterH = '-' }

        public Position Position { get; private set; } = position;
        public Kinds Kind { get; private set; } = kind;
        public bool Energized { get; set; } = false;

        public override string ToString()
            => $"{Position}:{Kind} ({Energized})";
    }

    public class Beam(int number, Position position, Direction direction)
    {
        public static readonly Direction Rightward = (0, +1);
        public static readonly Direction Leftward = (0, -1);
        public static readonly Direction Downward = (+1, 0);
        public static readonly Direction Upward = (-1, 0);

        public int Number { get; private set; } = number;
        public bool Active { get; set; } = true;
        public Position Position { get; set; } = position;
        public Direction Direction { get; set; } = direction;

        public Position NextPosition
            => (Position.Row + Direction.dRow, Position.Col + Direction.dCol);

        public void Update(Position position)
        {
            Position = position;
        }

        public void Update(Position position, Direction direction)
        {
            Position = position;
            Direction = direction;
        }

        public override string ToString()
            => $"{Number}. {Position}/{Direction}: {Active}";
    }

    public class Grid
    {
        public Cell[][] Cells { get; private set; }
        public List<Beam> Beams { get; private set; }
        private int beamIdCounter;

        public Grid(string inputFile, params Beam[] beams)
        {
            string[] lines = File.ReadAllLines(inputFile);
            Cells = Enumerable.Range(0, lines.Length)
                .Select(row => Enumerable.Range(0, lines.Length).Select(col => new Cell((row, col), lines[row][col].ToCellKind())).ToArray())
                .ToArray();
            Beams = [.. beams];
            beamIdCounter = beams.Length;
        }

        public int GetNewBeamNumber
            => ++beamIdCounter;

        private void MoveBeamOnce(Beam beam, bool logSteps)
        {
            if (logSteps) Console.Write($"{beam} ... ");
            Position position = beam.NextPosition;

            if (!position.IsInsideGrid(Cells.Length))
            {
                beam.Active = false;
                if (logSteps) Console.WriteLine($"{beam}");
                return;
            }

            Cell cell = Cells[position.Row][position.Col];

            bool wasAlreadyEnergized = cell.Energized;
            cell.Energized = true;

            switch (cell.Kind)
            {
                case Cell.Kinds.Space:
                    beam.Update(position, beam.Direction);
                    break;
                case Cell.Kinds.MirrorFwd:
                case Cell.Kinds.MirrorBwd:
                    beam.Update(position, Utils.GetNewDirectionByMirror(cell.Kind, beam.Direction));
                    break;
                case Cell.Kinds.SplitterV:
                    if (beam.Direction.IsHorizontal())
                    {
                        beam.Update(position);
                        beam.Active = false;
                        if (!wasAlreadyEnergized)
                            Beams.AddRange([new Beam(GetNewBeamNumber, position, Beam.Upward), new Beam(GetNewBeamNumber, position, Beam.Downward)]);
                    }
                    else
                    {
                        beam.Update(position, beam.Direction);
                    }
                    break;
                case Cell.Kinds.SplitterH:
                    if (beam.Direction.IsVertical())
                    {
                        beam.Update(position);
                        beam.Active = false;
                        if (!wasAlreadyEnergized)
                            Beams.AddRange([new Beam(GetNewBeamNumber, position, Beam.Leftward), new Beam(GetNewBeamNumber, position, Beam.Rightward)]);
                    }
                    else
                    {
                        beam.Update(position, beam.Direction);
                    }
                    break;
            }
            if (logSteps) Console.WriteLine($"{beam}");
        }

        public Grid PewPew(bool logSteps)
        {
            do
            {
                for (int iBeam = 0; iBeam < Beams.Count; iBeam++)
                {
                    Beam beam = Beams[iBeam];
                    if (beam.Active)
                    {
                        MoveBeamOnce(beam, logSteps);
                    }
                }
            } while (Beams.Any(beam => beam.Active));
            return this;
        }

        public string GetDisplayGrid(bool showEnergized)
        {
            string topRow = $"    {string.Join(null, Enumerable.Range(0, Cells.Length).Select(iCol => iCol % 10))}\n";
            return topRow + string.Join("\n", Enumerable.Range(0, Cells.Length).Select(iRow => $"{iRow,3} {string.Join(null, Cells[iRow].Select(cell => showEnergized ? cell.ToEnergizedOrNot() : cell.ToCharacter()))}"));
        }

        public int GetNumberOfEnergizedCells()
            => Cells.SelectMany(row => row).Count(cell => cell.Energized);

        public void Print(string title, bool showEnergized)
        {
            Console.WriteLine($"\n > {title}:\n{GetDisplayGrid(showEnergized)}");
            if (showEnergized) Console.WriteLine($" > Number of cells that are energized: {GetNumberOfEnergizedCells()}");
        }
    }

    internal class Program
    {
        private static List<Beam> GetStartupBeams(int gridSize)
        {
            List<Beam> result = [];
            result.AddRange(Enumerable.Range(0, gridSize).SelectMany(iCol => new List<Beam>() { new(1, (-1, iCol), Beam.Downward), new(1, (gridSize, iCol), Beam.Upward) }));
            result.AddRange(Enumerable.Range(0, gridSize).SelectMany(iRow => new List<Beam>() { new(1, (iRow, -1), Beam.Rightward), new(1, (iRow, gridSize), Beam.Leftward) }));
            return result;
        }

        private static void Main(string[] _)
        {
            string inputFile = false ? "input0.txt" : "input1.txt";

            Grid grid = new Grid(inputFile, new Beam(1, (0, -1), Beam.Rightward)).PewPew(false);
            grid.Print("Original grid", false);
            grid.Print("After light shining", true);

            List<Grid> grids = [.. GetStartupBeams(grid.Cells.Length)
                .Select(startupBeam => new Grid(inputFile, startupBeam).PewPew(false))
                .OrderByDescending(iGrid => iGrid.GetNumberOfEnergizedCells())];
            Console.WriteLine($"\n > The most energized configuration has {grids.First().GetNumberOfEnergizedCells()} energized cells");
        }
    }
}