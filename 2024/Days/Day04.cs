using AoC2024.ForDay04;
using AoC2024.MatrixNavigation;

namespace AoC2024
{
    namespace ForDay04
    {
        public enum Direction { Horizontal = 'H', Vertical = 'V', DiagonalUp = 'U', DiagonalDown = 'D' }
    }

    public class Day04 : DayProgram
    {
        private static string GetSlice(string[] lines, Coord from, Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    return lines[from.Y];
                case Direction.Vertical:
                    return new(Enumerable.Range(0, lines.Length).Select(iRow => lines[iRow][from.X]).ToArray());
                case Direction.DiagonalDown:
                    List<char> diagonalDownChars = [];
                    for (int iRow = from.Y, iCol = from.X; iRow < lines.Length && iCol < lines[0].Length; iRow++, iCol++)
                        diagonalDownChars.Add(lines[iRow][iCol]);
                    return new([.. diagonalDownChars]);
                case Direction.DiagonalUp:
                    List<char> diagonalUpChars = [];
                    for (int iRow = from.Y, iCol = from.X; iRow >= 0 && iCol < lines[0].Length; iRow--, iCol++)
                        diagonalUpChars.Add(lines[iRow][iCol]);
                    return new([.. diagonalUpChars]);
                default:
                    throw new ArgumentException("Unknown direction!");
            }
        }

        private static int Contains(string slice, string word)
            => slice.Length < word.Length
                ? 0
                : Enumerable.Range(0, slice.Length - word.Length + 1).Select(i => slice.Substring(i, word.Length)).Count(sub => sub == word);

        private static int FindForwardsAndBackwards(string[] lines, string word, Coord from, Direction direction, bool logging)
        {
            const int FMT_LEN = 1;
            string slice = GetSlice(lines, from, direction);
            string reverseSlice = new(slice.Reverse().ToArray());
            (int forwardsFinds, int backwardsFinds) = (Contains(slice, word), Contains(reverseSlice, word));
            if (logging) $" - '{slice}' (len={slice.Length}; {(char)direction} from X={from.X,FMT_LEN},Y={from.Y,FMT_LEN}): {forwardsFinds} + {backwardsFinds}".Log();
            return forwardsFinds + backwardsFinds;
        }

        private static List<Coord> GetDiagonalDownStartingPoints(string[] lines)
            => Enumerable.Range(0, lines[0].Length).Reverse().Select(iCol => new Coord(iCol, 0))
                .Concat(Enumerable.Range(1, lines.Length - 1).Select(iRow => new Coord(0, iRow)))
                .ToList();

        private static List<Coord> GetDiagonalUpStartingPoints(string[] lines)
            => Enumerable.Range(0, lines.Length).Select(iRow => new Coord(0, iRow))
                .Concat(Enumerable.Range(1, lines[0].Length - 1).Select(iCol => new Coord(iCol, lines.Length - 1)))
                .ToList();

        private static void FindWordOccurrences(string title, string[] lines, bool logging)
        {
            const string WORD = "XMAS";
            int occurrences = 0;
            for (int iRow = 0; iRow < lines.Length; iRow++)
                occurrences += FindForwardsAndBackwards(lines, WORD, new(0, iRow), Direction.Horizontal, logging);
            if (logging) string.Empty.Log();
            for (int iCol = 0; iCol < lines[0].Length; iCol++)
                occurrences += FindForwardsAndBackwards(lines, WORD, new(iCol, 0), Direction.Vertical, logging);
            if (logging) string.Empty.Log();
            foreach (Coord coord in GetDiagonalDownStartingPoints(lines))
                occurrences += FindForwardsAndBackwards(lines, WORD, coord, Direction.DiagonalDown, logging);
            if (logging) string.Empty.Log();
            foreach (Coord coord in GetDiagonalUpStartingPoints(lines))
                occurrences += FindForwardsAndBackwards(lines, WORD, coord, Direction.DiagonalUp, logging);
            if (logging) string.Empty.Log();
            $" > In the {title} (a {lines[0].Length} by {lines.Length} matrix), the word '{WORD}' appears a total of {occurrences} times.".Log();
        }

        private static bool IsXDashMas(string[] square)
        {
            if (square[1][1] != 'A')
                return false;
            return (square[0][0] == 'M' && square[2][2] == 'S' || square[0][0] == 'S' && square[2][2] == 'M')
                && (square[0][2] == 'M' && square[2][0] == 'S' || square[0][2] == 'S' && square[2][0] == 'M');
        }

        private static string[] GetSquareOf3(string[] lines, Coord center)
            => Enumerable.Range(-1, 3).Select(iRowDelta => lines[center.Y + iRowDelta].Substring(center.X - 1, 3)).ToArray();

        private static void FindXDashMasOccurrences(string title, string[] lines, bool logging)
        {
            List<string[]> squares = Enumerable.Range(1, lines.Length - 2)
                .SelectMany(iRow => Enumerable.Range(1, lines[0].Length - 2).Select(iCol => new Coord(iCol, iRow)))
                .Select(coord => GetSquareOf3(lines, coord))
                .ToList();
            List<string[]> occurrences = squares.Where(IsXDashMas).ToList();
            if (logging) occurrences.ForEach(square => $"{square[0]}\n{square[1]}\n{square[2]}\n... is an X\n".Log());
            $" > In the {title} (a {lines[0].Length} by {lines.Length} matrix), there are a number of {occurrences.Count} X-MAS'es that appear.".Log();
        }

        public override void Run()
        {
            (string[] testData, string[] fullData) = (ReadFromFile_Strings(@"tests\04"), ReadFromFile_Strings(@"04"));

            FindWordOccurrences("example data", testData, false);
            FindWordOccurrences("full word search data", fullData, false);
            string.Empty.Log();

            FindXDashMasOccurrences("example data", testData, true);
            FindXDashMasOccurrences("full word search data", fullData, false);
            string.Empty.Log();
        }
    }
}