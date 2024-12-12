using System.Drawing;

namespace AoC2024.MatrixNavigation
{
    public class NavigableMap(Size mapSize, Dictionary<Coord, NavigableMap.Cell> cells)
    {
        public record Cell(char Character, Coord Coord, Dictionary<DirectionsUDLR, Coord> MovementOptions, object? Tag = null);

        public static NavigableMap Parse(string[] lines, Func<Coord, char, bool> cellValidHandler, Func<Coord, char, object?>? cellTagGetter = null)
        {
            if (lines.Length == 0 || lines[0].Length == 0)
                throw new ArgumentException("Map must not be empty!");
            if (!lines.Skip(1).All(line => line.Length == lines[0].Length))
                throw new ArgumentException("Map must be rectangular!");

            int matrixHeight = lines.Length, matrixWidth = lines[0].Length;
            Dictionary<Coord, Cell> cells = [];

            for (int y = 0; y < matrixHeight; y++)
            {
                for (int x = 0; x < matrixWidth; x++)
                {
                    Coord coord = new(x, y);
                    char chr = lines[y][x];
                    if (cellValidHandler.Invoke(coord, chr))
                    {
                        Cell value = new(chr, coord, [], cellTagGetter?.Invoke(coord, chr));
                        cells.Add(coord, value);
                    }
                }
            }

            void CheckIfOkAndAdd(Coord coord, DirectionsUDLR direction, bool coordinateOk)
            {
                Coord newCoord = coord.Move(direction);
                if (coordinateOk && cells.TryGetValue(coord, out Cell? coordCell) && cells.ContainsKey(newCoord))
                    coordCell.MovementOptions.Add(direction, newCoord);
            }

            for (int y = 0; y < matrixHeight; y++)
            {
                for (int x = 0; x < matrixWidth; x++)
                {
                    Coord coord = new(x, y);
                    CheckIfOkAndAdd(coord, DirectionsUDLR.Up, y > 0);
                    CheckIfOkAndAdd(coord, DirectionsUDLR.Down, y < matrixHeight - 1);
                    CheckIfOkAndAdd(coord, DirectionsUDLR.Left, x > 0);
                    CheckIfOkAndAdd(coord, DirectionsUDLR.Right, x < matrixWidth - 1);
                }
            }

            return new(new(matrixWidth, matrixHeight), cells);
        }

        public Size MapSize { get; private set; } = mapSize;
        public Dictionary<Coord, Cell> Cells { get; private set; } = cells;

        public NavigableMap(NavigableMap map) : this(map.MapSize, new(map.Cells)) { }

        public Cell GetByCharacter(char character)
            => Cells.Values.First(cell => cell.Character == character);

        public bool TryGetByCoord(Coord coord, out Cell? cell)
            => Cells.TryGetValue(coord, out cell);

        public Cell MoveIfPossible(Cell cell, DirectionsUDLR direction)
            => cell.MovementOptions.TryGetValue(direction, out Coord? coord) ? Cells[coord] : cell;

        public string DrawMapStr()
            => string.Join(Environment.NewLine, Enumerable.Range(0, MapSize.Height).Select(r => new string(Enumerable.Range(0, MapSize.Width).Select(c => Cells[new(c, r)].Character).ToArray())));

        public string ToTagMapString()
            => string.Join(Environment.NewLine, Enumerable.Range(0, MapSize.Height).Select(iRow => string.Join(string.Empty, Enumerable.Range(0, MapSize.Width).Select(iCol => TryGetByCoord(new(iCol, iRow), out Cell? cell) ? (cell!.Tag != null ? cell.Tag.ToString() : ".") : "."))));

        public override string ToString()
            => string.Join(" ", Cells.Select(c => $"[{c.Key.X},{c.Key.Y}]='{c.Value.Character}'"));
    }
}