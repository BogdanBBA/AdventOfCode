namespace AoC2016.MatrixNavigation
{
    public class NavigableMap
    {
        public record Cell(char Character, Coord Coord, Dictionary<DirectionsUDLR, Coord> MovementOptions);

        public static NavigableMap Parse(string[] lines, Func<Coord, char, bool> cellValidHandler)
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
                        cells.Add(coord, new(chr, coord, []));
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

            return new(cells);
        }

        public Dictionary<Coord, Cell> Cells { get; private set; }

        private NavigableMap(Dictionary<Coord, Cell> cells)
        {
            Cells = cells;
        }

        public Cell GetByCharacter(char character)
            => Cells.Values.First(cell => cell.Character == character);

        public Cell MoveIfPossible(Cell cell, DirectionsUDLR direction) 
            => cell.MovementOptions.TryGetValue(direction, out Coord? coord) ? Cells[coord] : cell;

        public override string ToString()
            => string.Join(" ", Cells.Select(c => $"[{c.Key.X},{c.Key.Y}]='{c.Value.Character}'"));
    }
}