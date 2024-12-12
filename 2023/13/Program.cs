using System.Globalization;
using System.Text;
using System.Xml;

namespace AoC13
{
    public static class Extensions
    {
        public const int FmtPts = 4;
        public const int FmtCoord = 2;

        private static readonly string DashesTuple = new('-', 8);

        public static char Opposite(this char character)
            => character == '#' ? '.' : '#';

        public static string ToFormatted(this (int, int) tuple)
            => $"({tuple.Item1,FmtCoord}, {tuple.Item2,FmtCoord})";

        public static string ToFormatted(this (int, int)? tuple)
            => tuple is null ? DashesTuple : $"({tuple.Value.Item1,FmtCoord}, {tuple.Value.Item2,FmtCoord})";

        public static Pattern FindAndFixSmudge(this Pattern pattern, ref (int Row, int Col) replacementLocation)
        {
            (int RowA, int RowB)? oVSymm = pattern.DetermineVerticalSymmetry();
            (int ColA, int ColB)? oHSymm = pattern.DetermineHorizontalSymmetry();
            int rowCount = pattern.Rows.Length, colCount = pattern.Columns.Length;
            StringBuilder[] rows = pattern.Rows.Select(row => new StringBuilder(row)).ToArray();

            for (int index = 0, count = rowCount * colCount; index < count; index++)
            {
                int iRow = index / colCount, iCol = index % colCount;
                rows[iRow][iCol] = rows[iRow][iCol].Opposite();
                Pattern newPattern = new(string.Join(Environment.NewLine, rows.Select(row => row.ToString())));
                (int RowA, int RowB)? vSymm = newPattern.DetermineVerticalSymmetry();
                (int ColA, int ColB)? hSymm = newPattern.DetermineHorizontalSymmetry();
                if (vSymm != oVSymm || hSymm != oHSymm)
                {
                    replacementLocation = (iRow, iCol);
                    return newPattern;
                }
                rows[iRow][iCol] = rows[iRow][iCol].Opposite();
            }

            throw new Exception($"Pattern doesn't seem to have any symmetry no matter which character you replace!?");
        }
    }

    public class Pattern
    {
        public string[] Rows { get; private set; }
        public string[] Columns { get; private set; }

        public Pattern(string text)
        {
            Rows = text.Split(Environment.NewLine);
            if (Rows.Select(line => line.Length).Distinct().Count() != 1)
                throw new Exception();
            Columns = new string[Rows[0].Length];
            for (int iCol = 0; iCol < Columns.Length; iCol++)
                Columns[iCol] = new string(Rows.Select(line => line[iCol]).ToArray());
        }

        private bool ConfirmVerticalSymmetry(int rowA, int rowB)
        {
            for (int a = rowA, b = rowB; a >= 0 && b < Rows.Length; a--, b++)
                if (Rows[a] != Rows[b])
                    return false;
            return true;
        }

        private bool ConfirmHorizontalSymmetry(int colA, int colB)
        {
            for (int a = colA, b = colB; a >= 0 && b < Columns.Length; a--, b++)
                if (Columns[a] != Columns[b])
                    return false;
            return true;
        }

        public (int RowA, int RowB)? DetermineVerticalSymmetry()
        {
            for (int iRow = 0; iRow < Rows.Length - 1; iRow++)
            {
                if (Rows[iRow] == Rows[iRow + 1] && ConfirmVerticalSymmetry(iRow, iRow + 1))
                    return (iRow, iRow + 1);
            }
            // for (int iRow = 1; iRow < Rows.Length - 1; iRow++)
            // {
            //     if (Rows[iRow - 1] == Rows[iRow + 1] && ConfirmVerticalSymmetry(iRow - 1, iRow + 1))
            //         return (iRow - 1, iRow + 1);
            // }
            return null;
        }

        public (int ColA, int ColB)? DetermineHorizontalSymmetry()
        {
            for (int iCol = 0; iCol < Columns.Length - 1; iCol++)
            {
                if (Columns[iCol] == Columns[iCol + 1] && ConfirmHorizontalSymmetry(iCol, iCol + 1))
                    return (iCol, iCol + 1);
            }
            // for (int iCol = 1; iCol < Columns.Length - 1; iCol++)
            // {
            //     if (Columns[iCol - 1] == Columns[iCol + 1] && ConfirmHorizontalSymmetry(iCol - 1, iCol + 1))
            //         return (iCol - 1, iCol + 1);
            // }
            return null;
        }

        public static int GetVerticalSymmetryPoints((int RowA, int RowB)? vSymm)
        {
            if (vSymm is null)
                return 0;
            if ((vSymm.Value.RowB - vSymm.Value.RowA) % 2 == 1)
                return vSymm.Value.RowB * 100;
            return (vSymm.Value.RowA + vSymm.Value.RowB) / 2 * 100;
        }

        public static int GetHorizontalSymmetryPoints((int ColA, int ColB)? hSymm)
        {
            if (hSymm is null)
                return 0;
            if ((hSymm.Value.ColB - hSymm.Value.ColA) % 2 == 1)
                return hSymm.Value.ColB;
            return (hSymm.Value.ColA + hSymm.Value.ColB) / 2;
        }

        public override string ToString() => string.Join(Environment.NewLine, Rows);

        public string ToString(bool includeNumbers) => includeNumbers
            ? $"{" ",Extensions.FmtCoord} {string.Join(null, Enumerable.Range(0, Columns.Length).Select(iCol => iCol % 10))}{Environment.NewLine}{string.Join(Environment.NewLine, Enumerable.Range(0, Rows.Length).Select(index => $"{index,Extensions.FmtCoord} {Rows[index]}"))}"
            : ToString();
    }

    internal class Program
    {
        private const string InputFile = "input0.txt";
        private const string OutputFolder = "outputFiles";
        private const bool PrintNotes = true;
        private const bool FixSmudges = true;

        private static readonly string NL = Environment.NewLine;

        private static List<Pattern> LoadInput(string filePath)
            => File.ReadAllText(filePath).Split(Environment.NewLine + Environment.NewLine).Select(text => new Pattern(text)).ToList();

        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Directory.CreateDirectory(OutputFolder);

            List<Pattern> patterns = LoadInput(InputFile);

            TimeSpan totalDuration = TimeSpan.Zero;
            int total = 0;
            for (int iPattern = 0; iPattern < patterns.Count; iPattern++)
            {
                Pattern pattern = patterns[iPattern];
                DateTime start = DateTime.Now;
                string prefix = PrintNotes ? $"{NL}{pattern.ToString(true)}{NL}" : string.Empty;
                Console.Write($"{prefix} - note #{iPattern + 1,3} ... ");

                if (FixSmudges)
                {
                    Console.Write("fixing smudge ... ");
                    (int Row, int Col) replacementLocation = (-1, -1);
                    Pattern newPattern = pattern.FindAndFixSmudge(ref replacementLocation);
                    Console.Write($"changed @ {replacementLocation.ToFormatted()} ... ");
                    //File.WriteAllText($"{OutputFolder}/{iPattern + 1}.txt", $"{pattern}{NL}{NL}Changed at {replacementLocation}{NL}{NL}{newPattern}");
                    pattern = newPattern;
                }

                (int RowA, int RowB)? vSymm = pattern.DetermineVerticalSymmetry();
                int verticalSymmetryPoints = Pattern.GetVerticalSymmetryPoints(vSymm);
                (int ColA, int ColB)? hSymm = pattern.DetermineHorizontalSymmetry();
                int horizontalSymmetryPoints = Pattern.GetHorizontalSymmetryPoints(hSymm);

                TimeSpan duration = DateTime.Now.Subtract(start);
                totalDuration = totalDuration.Add(duration);
                total += verticalSymmetryPoints + horizontalSymmetryPoints;

                Console.Write($"V: {vSymm.ToFormatted()} -> {verticalSymmetryPoints,Extensions.FmtPts}; ");
                Console.Write($"H: {hSymm.ToFormatted()} -> {horizontalSymmetryPoints,Extensions.FmtPts}");
                Console.WriteLine($"; done in {duration.TotalSeconds:N2}s");
            }
            Console.WriteLine($" > Total: {total} (done in {totalDuration.TotalSeconds:N2}s in total)");
        }
    }
}