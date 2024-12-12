using System.Security.Cryptography;

namespace AoC10
{
    public enum CellTypes { Start = 'S', Vertical = '|', Horizontal = '-', NE = 'L', NW = 'J', SW = '7', SE = 'F', Ground = '.' }

    internal enum Directions { Northward, Eastward, Southward, Westward }

    public static class Extensions
    {
        public static char ToReadableGlyph(this CellTypes cellType)
        {
            return cellType switch
            {
                CellTypes.Start => 'S',
                CellTypes.Vertical => '│',
                CellTypes.Horizontal => '─',
                CellTypes.NE => '└',
                CellTypes.NW => '┘',
                CellTypes.SW => '┐',
                CellTypes.SE => '┌',
                CellTypes.Ground => ' ',
                _ => throw new ArgumentException($"{cellType}")
            };
        }
    }

    internal class Cell(int row, int col, CellTypes type)
    {
        public static readonly Dictionary<CellTypes, Directions[]> AllowedDirectionsMap = new()
        {
            { CellTypes.Start, [ Directions.Northward, Directions.Eastward, Directions.Southward, Directions.Westward ]},
            { CellTypes.Vertical, [ Directions.Northward, Directions.Southward ] },
            { CellTypes.Horizontal, [ Directions.Eastward, Directions.Westward ] },
            { CellTypes.NE, [ Directions.Northward, Directions.Eastward ] },
            { CellTypes.NW, [ Directions.Northward, Directions.Westward ] },
            { CellTypes.SW, [ Directions.Southward, Directions.Westward ] },
            { CellTypes.SE, [ Directions.Southward, Directions.Eastward ] },
            { CellTypes.Ground, [ ] }
        };

        public enum Position { NotInitialized = '_', L = 'L', O = '▫', I = '■' }

        public (int Row, int Col) Coordinates { get; } = (row, col);
        public CellTypes Type { get; } = type;
        public Cell? Previous { get; set; }
        public Cell? Next { get; set; }

        public bool IsLoop => Next != null && Previous != null;
        public Directions[] GetAllowedDirections() => AllowedDirectionsMap[Type];

        public override string ToString() => $"r{Coordinates.Row + 1},c{Coordinates.Col + 1}. {$"'{(char)Type}' {Type}",14}";
    }

    internal class Map
    {
        public Cell[][] Cells { get; }
        public Cell StartCell { get; }

        public Map(Cell[][] cells, bool log = true)
        {
            Cells = cells;

            static bool IsOk(Cell sourceCell, Directions direction, Cell? destCell, params CellTypes[] cellTypes)
            {
                if (destCell is null) return false;
                if (!sourceCell.GetAllowedDirections().Contains(direction)) return false;
                return destCell.Type == CellTypes.Start || cellTypes.Contains(destCell.Type);
            }

            (Cell, Directions) GetStartNeighborCell(Cell sourceCell, Directions? directionOfArrival)
            {
                // try north
                Cell? destCell = Directions.Southward == directionOfArrival ? null : GetNorth(sourceCell);
                if (IsOk(sourceCell, Directions.Northward, destCell, CellTypes.Vertical, CellTypes.SW, CellTypes.SE))
                    return (destCell!, Directions.Northward);

                // try south
                destCell = Directions.Northward == directionOfArrival ? null : GetSouth(sourceCell);
                if (IsOk(sourceCell, Directions.Southward, destCell, CellTypes.Vertical, CellTypes.NW, CellTypes.NE))
                    return (destCell!, Directions.Southward);

                // try east
                destCell = Directions.Westward == directionOfArrival ? null : GetEast(sourceCell);
                if (IsOk(sourceCell, Directions.Eastward, destCell, CellTypes.Horizontal, CellTypes.NW, CellTypes.SW))
                    return (destCell!, Directions.Eastward);

                // try west
                destCell = Directions.Eastward == directionOfArrival ? null : GetWest(sourceCell);
                if (IsOk(sourceCell, Directions.Westward, destCell, CellTypes.Horizontal, CellTypes.NE, CellTypes.SE))
                    return (destCell!, Directions.Westward);

                throw new Exception($"Unknown next neighbour of {sourceCell} (coming {directionOfArrival})");
            }

            StartCell = Cells.SelectMany(row => row).First(cell => cell.Type == CellTypes.Start);
            Cell currentCell = StartCell;
            Directions? direction = null;
            do
            {
                (Cell neighborCell, direction) = GetStartNeighborCell(currentCell, direction);
                if (log) Console.WriteLine($" - {currentCell}   ->   {neighborCell}   (going {direction})");
                currentCell.Next = neighborCell;
                neighborCell.Previous = currentCell;
                currentCell = neighborCell;
            } while (currentCell != StartCell);
            if (log) Console.WriteLine();
        }

        private Cell? GetNorth(Cell cell) => cell.Coordinates.Row <= 0 ? null : Cells[cell.Coordinates.Row - 1][cell.Coordinates.Col];
        private Cell? GetSouth(Cell cell) => cell.Coordinates.Row >= Cells.Length - 1 ? null : Cells[cell.Coordinates.Row + 1][cell.Coordinates.Col];
        private Cell? GetWest(Cell cell) => cell.Coordinates.Col <= 0 ? null : Cells[cell.Coordinates.Row][cell.Coordinates.Col - 1];
        private Cell? GetEast(Cell cell) => cell.Coordinates.Col >= Cells[cell.Coordinates.Row].Length - 1 ? null : Cells[cell.Coordinates.Row][cell.Coordinates.Col + 1];

        public int GetLoopLength()
        {
            int length = 0;
            Cell current = StartCell;
            do
            {
                current = current.Next!;
                length++;
            } while (current != StartCell);
            return length;
        }

        public int FurthestStepsFromStartCell()
            => (int)Math.Ceiling(GetLoopLength() / 2.0);

        public int CountTilesWithinLoop(bool log = false)
        {
            int totalTilesInsideLoop = 0;
            if (log) Console.WriteLine();
            foreach (Cell[] row in Cells)
            {
                Cell? previousCell = null;
                bool insideLoop = false;
                foreach (Cell cell in row)
                {
                    if (cell.Type == CellTypes.Start)
                    {
                        insideLoop = false;
                    }
                    else
                    {
                        if (cell.Type == CellTypes.Vertical ||
                            cell.Type == CellTypes.NW && previousCell?.Type == CellTypes.SE ||
                            cell.Type == CellTypes.SW && previousCell?.Type == CellTypes.NE)
                        {
                            insideLoop = !insideLoop;
                        }
                    }
                    if (insideLoop)
                        totalTilesInsideLoop++;
                    previousCell = cell;
                    if (log) Console.Write(cell.IsLoop ? cell.Type.ToReadableGlyph() : (insideLoop ? "■" : "▫"));
                }
                if (log) Console.WriteLine();
            }
            if (log) Console.WriteLine();
            return totalTilesInsideLoop;
        }

        public int CountTilesWithinLoop_ShoelaceFormula(bool log = false) // https://en.wikipedia.org/wiki/Shoelace_formula
        {
            int totalTilesInsideLoop = 0;
            if (log) Console.WriteLine();
            foreach (Cell[] row in Cells)
            {
                foreach (Cell cell in row)
                {
                    if (log) Console.Write(cell.IsLoop ? cell.Type.ToReadableGlyph() : (insideLoop ? "■" : "▫"));
                }
                if (log) Console.WriteLine();
            }
            if (log) Console.WriteLine();
            return totalTilesInsideLoop;
        }

        public void Print(string title)
        {
            string table = string.Join("\n", Cells.Select(row => string.Join(null, row.Select(cell => (char)cell.Type))));
            Console.WriteLine($"{title} ({Cells.Length} x {Cells.First().Length}):\n\n{table}\n");
        }
    }

    internal class Program
    {
        private static void Main(string[] _)
        {
            string[] lines = File.ReadAllLines("input1.txt");
            Cell[][] cells = Enumerable.Range(0, lines.Length).Select(row => Enumerable.Range(0, lines[row].Length).Select(col => new Cell(row, col, (CellTypes)lines[row][col])).ToArray()).ToArray();
            Map map = new(cells, false);

            map.Print("Normal map");
            Console.WriteLine($" > The maximum distance from the start is {map.FurthestStepsFromStartCell()} steps.");
            Console.WriteLine($" > There are {map.CountTilesWithinLoop_ShoelaceFormula(true)} tiles within the loop.");
        }
    }
}