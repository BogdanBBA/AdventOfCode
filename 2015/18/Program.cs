namespace AoC2015.Day18
{
    using Coordinate = (int Row, int Col);

    public class Light(Coordinate coord, bool stateOn)
    {
        public Coordinate Coord { get; private set; } = coord;
        public bool StateOn { get; internal set; } = stateOn;

        public bool DetermineNextState(Light[] neighbours)
        {
            int neighboursThatAreOn = neighbours.Count(neighbour => neighbour.StateOn);
            if (StateOn)
                return neighboursThatAreOn == 2 || neighboursThatAreOn == 3;
            return neighboursThatAreOn == 3;
        }
    }

    public class LightGrid(Light[][] lights)
    {
        public Light[][] Grid { get; private set; } = lights;
        public Light[] List { get; private set; } = lights.SelectMany(row => row).ToArray();

        public int NumberOfLightsThatAreOn
            => List.Count(light => light.StateOn);

        public void ForceCornersToOn()
        {
            Grid.First().First().StateOn = true;
            Grid.First().Last().StateOn = true;
            Grid.Last().First().StateOn = true;
            Grid.Last().Last().StateOn = true;
        }

        public void Print(string title)
        {
            Console.WriteLine($" > {title} (with {NumberOfLightsThatAreOn} lights on)");
            Console.WriteLine($"{string.Join("\n", Grid.Select(row => $"   {string.Join(string.Empty, row.Select(light => light.StateOn ? "#" : "."))}"))}\n");
        }

        private Light[] GetNeighbours(Coordinate coord)
        {
            bool HasAbove() => coord.Row > 0;
            bool HasBelow() => coord.Row < Grid.Length - 1;
            bool HasLeft() => coord.Col > 0;
            bool HasRight() => coord.Col < Grid[coord.Row].Length - 1;
            return new Light[] {
                HasAbove() && HasLeft() ? Grid[coord.Row - 1][coord.Col - 1] : null!,
                HasAbove() ? Grid[coord.Row - 1][coord.Col] : null!,
                HasAbove() && HasRight() ? Grid[coord.Row - 1][coord.Col + 1] : null!,
                HasRight() ? Grid[coord.Row][coord.Col + 1] : null!,
                HasBelow() && HasRight() ? Grid[coord.Row + 1][coord.Col + 1] : null!,
                HasBelow() ? Grid[coord.Row + 1][coord.Col] : null!,
                HasBelow() && HasLeft() ? Grid[coord.Row + 1][coord.Col - 1] : null!,
                HasLeft() ? Grid[coord.Row][coord.Col - 1] : null!
            }.Where(neighbour => neighbour is not null).Cast<Light>().ToArray();
        }

        public LightGrid GenerateNextState()
            => new(Grid.Select(row => row.Select(light => new Light(light.Coord, light.DetermineNextState(GetNeighbours(light.Coord)))).ToArray()).ToArray());

        public static void Animate(ref LightGrid lightGrid, int numberOfSteps, bool cornersAreStuck)
        {
            if (cornersAreStuck) lightGrid.ForceCornersToOn();
            lightGrid.Print("Initial state");
            for (int step = 1; step <= numberOfSteps; step++)
            {
                lightGrid = lightGrid.GenerateNextState();
                if (cornersAreStuck) lightGrid.ForceCornersToOn();
                lightGrid.Print($"After {step} animation {(step == 1 ? "step" : "steps")}");
            }
        }
    }

    public class Program : IDayProgram
    {
        private static LightGrid ParseLightGrid(string[] lines)
            => new(Enumerable.Range(0, lines.Length).Select(iRow => Enumerable.Range(0, lines[iRow].Length).Select(iCol => lines[iRow][iCol] switch
                {
                    '.' => new Light((iRow, iCol), false),
                    '#' => new Light((iRow, iCol), true),
                    _ => throw new ArgumentException($"'{lines[iRow][iCol]}' is an invalid light state!")
                }).ToArray()).ToArray());

        public override int GetCurrentDay => 18;
        public override int GetCurrentPart => 1;
        private int GetNumberOfSteps => GetCurrentPart == 0 ? (CornersAreStuck ? 5 : 4) : 100;
        private bool CornersAreStuck => true;

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            LightGrid lightGrid = ParseLightGrid(lines);
            LightGrid.Animate(ref lightGrid, GetNumberOfSteps, CornersAreStuck);
            Console.WriteLine($" >>> The answer is: there are {lightGrid.NumberOfLightsThatAreOn} lights");
        }
    }
}