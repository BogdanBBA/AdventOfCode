internal partial class Program
{
    internal class Board
    {
        internal class Cell
        {
            public (int Row, int Col) Coordinates { get; private set; }
            public char Value { get; private set; }
            public bool Visited { get; set; }

            public Cell(int row, int col, char value)
            {
                Coordinates = (row, col);
                Value = value;
                Visited = false;
            }

            public bool IsDigit => char.IsDigit(Value);

            public bool IsGear => Value == GearSymbol;
        }

        public const char GearSymbol = '*';
        public static readonly char[] Symbols = ['-', '/', '+', '@', '#', '&', '$', '=', '%', GearSymbol];

        public List<Cell> Matrix { get; private set; }
        private (int Width, int Height) MatrixSize { get; set; }

        public Board()
        {
            char[][] charMatrix = File.ReadAllLines(@"input.txt").Select(line => line.ToCharArray()).ToArray();
            Matrix = [];
            MatrixSize = (charMatrix[0].Length, charMatrix.Length);
            for (int row = 0; row < charMatrix.Length; row++)
                for (int col = 0; col < charMatrix[row].Length; col++)
                    Matrix.Add(new Cell(row, col, charMatrix[row][col]));
        }

        private Cell GetCell(int row, int col)
            => Matrix[row * MatrixSize.Width + col];

        private List<Cell> GetCellsByCoordinateRange((int row, int leftCol, int rightCol) coord)
            => Enumerable.Range(coord.leftCol, coord.rightCol - coord.leftCol + 1).Select(iCol => GetCell(coord.row, iCol)).ToList();

        private static int GetNumber(List<Cell> numberCells)
            => int.Parse(string.Join(null, numberCells.Select(iCell => iCell.Value)));

        private (int row, int leftCol, int rightCol) GetNumberCoordinateRange(int row, int col)
        {
            int resultLeft = col, resultRight = col;
            for (int iLeft = col - 1; iLeft >= 0 && GetCell(row, iLeft).IsDigit; iLeft--) resultLeft = iLeft;
            for (int iRight = col + 1; iRight < MatrixSize.Width && GetCell(row, iRight).IsDigit; iRight++) resultRight = iRight;
            return (row, resultLeft, resultRight);
        }

        private List<Cell> GetSurroundingCells(int row, int leftCol, int rightCol)
        {
            List<(int iRow, int iCol)> coords = Enumerable.Range(row - 1, 3).SelectMany(iRow => Enumerable.Range(leftCol - 1, rightCol - leftCol + 3).Select(iCol => (iRow, iCol))).ToList();
            coords.RemoveAll(coord => coord.iRow == row && coord.iCol >= leftCol && coord.iCol <= rightCol);
            coords.RemoveAll(coord => coord.iRow < 0 || coord.iRow >= MatrixSize.Height);
            coords.RemoveAll(coord => coord.iCol < 0 || coord.iCol >= MatrixSize.Width);
            return coords.Select(coord => GetCell(coord.iRow, coord.iCol)).ToList();
        }

        public (List<(int Number, string SurroundingCellValues)> PartNumbers, List<(int Number, string SurroundingCellValues)> NotPartNumbers) GetAllNumbers()
        {
            List<(int Number, string SurroundingCellValues)> resultPartNumbers = [], resultNotPartNumbers = [];

            Matrix.ForEach(cell => cell.Visited = false);

            for (int iGo = 1; Matrix.Any(cell => !cell.Visited) && iGo <= Matrix.Count; iGo++)
            {
                Cell cell = Matrix.First(cell => !cell.Visited);

                if (cell.Value == '.' || Symbols.Contains(cell.Value))
                {
                    cell.Visited = true;
                    continue;
                }
                if (cell.IsDigit)
                {
                    (int row, int leftCol, int rightCol) = GetNumberCoordinateRange(cell.Coordinates.Row, cell.Coordinates.Col);
                    List<Cell> numberCells = GetCellsByCoordinateRange((row, leftCol, rightCol));
                    List<Cell> surroundingCells = GetSurroundingCells(row, leftCol, rightCol);
                    int number = GetNumber(numberCells);
                    string surroundingCellValues = string.Join(null, surroundingCells.Select(iCell => iCell.Value));
                    if (surroundingCells.Any(iCell => Symbols.Contains(iCell.Value)))
                        resultPartNumbers.Add((number, surroundingCellValues));
                    else
                        resultNotPartNumbers.Add((number, surroundingCellValues));
                    numberCells.ForEach(iCell => iCell.Visited = true);
                }
                else
                    throw new Exception($"What is '{cell.Value}'?");
            }

            return (resultPartNumbers, resultNotPartNumbers);
        }

        public List<(Cell cell, int numberA, int numberB)> GetGears()
        {
            List<(Cell cell, int numberA, int numberB)> result = [];
            List<Cell> gears = Matrix.Where(cell => cell.IsGear).ToList();
            foreach (Cell gear in gears)
            {
                List<Cell> surroundingCells = GetSurroundingCells(gear.Coordinates.Row, gear.Coordinates.Col, gear.Coordinates.Col);
                List<Cell> digitCells = surroundingCells.Where(cell => cell.IsDigit).ToList();
                if (digitCells.Count < 2) continue;
                List<(int row, int leftCol, int rightCol)> distinctNumberCoordRanges = digitCells
                    .Select(digitCell => GetNumberCoordinateRange(digitCell.Coordinates.Row, digitCell.Coordinates.Col))
                    .Distinct().ToList();
                if (distinctNumberCoordRanges.Count == 2)
                    result.Add((gear, GetNumber(GetCellsByCoordinateRange(distinctNumberCoordRanges[0])), GetNumber(GetCellsByCoordinateRange(distinctNumberCoordRanges[1]))));
            }
            return result;
        }
    }
}