using System.Globalization;

namespace AoC11
{
    public static class Extensions
    {
        public static Map ToMap(this List<string> lines, int emptySkyMultiplier)
        {
            List<List<Cell>> cells = Enumerable.Range(0, lines.Count).Select(row => Enumerable.Range(0, lines[0].Length).Select(col => new Cell(row, col, lines[row][col].ToCellType(), 0, 1, 1)).ToList()).ToList();
            for (int row = 0; row < cells.Count; row++)
                if (cells[row].All(cell => cell.Type == Cell.Types.Empty))
                    cells[row].ForEach(cell => cell.RowMultiplier = emptySkyMultiplier);
            for (int col = 0; col < cells[0].Count; col++)
            {
                List<Cell> colCells = cells.Select(row => row.Skip(col).Take(1).First()).ToList();
                if (colCells.All(cell => cell.Type == Cell.Types.Empty))
                    colCells.ForEach(cell => cell.ColMultiplier = emptySkyMultiplier);
            }
            Map map = new(lines.Count, lines[0].Length, cells);
            int galaxyNumber = 0;
            map.GetGalaxies().ForEach(galaxy => galaxy.Number = ++galaxyNumber);
            return map;
        }

        public static Cell.Types ToCellType(this char character)
            => Enum.GetValues(typeof(Cell.Types)).Cast<Cell.Types>().First(cell => (char)cell == character);

        public static string ToMultiplierString(this Cell cell, Map map)
        {
            string text = (cell.RowMultiplier > 1 ? "R" : "") + (cell.ColMultiplier > 1 ? "C" : "");
            if (text == "") text = ((char)Cell.Types.Empty).ToString();
            return $"{text,3}{(cell.Col == map.ColCount - 1 ? "\n" : "")}";
        }

        public static string ToPrintString(this Cell cell, Map map)
        {
            string text = cell.Type == Cell.Types.Galaxy ? $"{cell.Number}" : $"{(char)cell.Type}";
            return $"{text,2}{(cell.Col == map.ColCount - 1 ? "\n" : "")}";
        }
    }

    public class Cell(int row, int col, Cell.Types type, int number, int rowMultiplier, int colMultipler)
    {
        public enum Types { Empty = '.', Galaxy = '#' }

        public int Row { get; private set; } = row;
        public int Col { get; private set; } = col;
        public Types Type { get; private set; } = type;
        public int Number { get; internal set; } = number;
        public int RowMultiplier { get; internal set; } = rowMultiplier;
        public int ColMultiplier { get; internal set; } = colMultipler;

        public override string ToString()
            => $"({Row},{Col}). {Number} ({Type}, x{RowMultiplier}r, x{ColMultiplier}c)";
    }

    public class Map(int rowCount, int colCount, List<List<Cell>> cells)
    {
        private static (int RangeStart, int RangeEnd, Cell CellX, Cell CellY) GetOrderedCoordinates(Cell a, Cell b, Func<Cell, int> predicate)
        {
            int xA = predicate(a), xB = predicate(b);
            return xA < xB ? (xA, xB, a, b) : (xB, xA, b, a);
        }

        private int GetShortestPath(Cell a, Cell b)
        {
            (int RangeStart, int RangeEnd, Cell cellX, Cell cellY) rows = GetOrderedCoordinates(a, b, cell => cell.Row);
            (int RangeStart, int RangeEnd, Cell cellX, Cell cellY) cols = GetOrderedCoordinates(a, b, cell => cell.Col);
            int rowDistance = 0, colDistance = 0;
            for (int row = rows.RangeStart + 1; row <= rows.RangeEnd; row++)
                rowDistance += Cells[row][rows.cellX.Col].RowMultiplier;
            for (int col = cols.RangeStart + 1; col <= cols.RangeEnd; col++)
                colDistance += Cells[cols.cellX.Row][col].ColMultiplier;
            return rowDistance + colDistance;
        }

        public int RowCount { get; private set; } = rowCount;
        public int ColCount { get; private set; } = colCount;
        public List<List<Cell>> Cells { get; private set; } = cells;

        public List<Cell> GetGalaxies() => Cells.SelectMany(row => row).Where(cell => cell.Type == Cell.Types.Galaxy).ToList();

        public ulong GetSumOfShortestPathsBetweenGalaxies(bool logPairDistances = false)
        {
            List<Cell> galaxies = GetGalaxies();
            ulong totalDistance = 0;

            //int totalCombos = (int)Math.Pow(galaxies.Count, 2) / 2;, iCombo = 0;
            for (int i = 0; i < galaxies.Count; i++)
            {
                for (int j = i + 1; j < galaxies.Count; j++)
                {
                    ulong distance = (ulong)GetShortestPath(galaxies[i], galaxies[j]);
                    totalDistance += distance;

                    if (logPairDistances) Console.WriteLine($" - between {galaxies[i].Number} and {galaxies[j].Number}: {distance}");
                    //else if (++iCombo % 1000 == 0) Console.WriteLine($" - {iCombo}/{totalCombos} ({(double)iCombo / totalCombos:P1})");
                }
            }

            return totalDistance;
        }
    }

    internal class Program
    {
        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string inputFile = "input1.txt";
            Map map = File.ReadAllLines(inputFile).ToList().ToMap(1000000);

            if (true)
            {
                Console.WriteLine($"\n >>> Sky map:\n{string.Join(null, map.Cells.SelectMany(row => row).Select(cell => cell.ToPrintString(map)))}\n");
                Console.WriteLine($"\n >>> Sky map (multipliers):\n{string.Join(null, map.Cells.SelectMany(row => row).Select(cell => cell.ToMultiplierString(map)))}\n");
            }

            ulong totalDistance = map.GetSumOfShortestPathsBetweenGalaxies();
            Console.WriteLine($" >>> There are {map.GetGalaxies().Count} galaxies in the given sky map.");
            Console.WriteLine($" >>> The sum of the shortes path between each pair of galaxies is: {totalDistance:N0} ({totalDistance}).");
        }
    }
}