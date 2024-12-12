using System.Drawing;

namespace AoC2016.MatrixNavigation
{
    public class MonochromeDisplay
    {
        public enum PixelState { Off = '□', On = '■' }

        public Size Size { get; private set; }
        public PixelState[][] Pixels { get; private set; }

        public MonochromeDisplay(int width, int height)
            : this(new(width, height)) { }

        public MonochromeDisplay(Size size, PixelState initialState = PixelState.Off)
        {
            Size = size;
            Pixels = Enumerable.Range(0, size.Height).Select(iRow => new PixelState[Size.Width]).ToArray();
            Fill(new(0, 0, size.Width, size.Height), initialState);
        }

        public void Fill(Rectangle rectangle, PixelState state)
        {
            for (int iRow = rectangle.Top; iRow < rectangle.Bottom; iRow++)
                for (int iCol = rectangle.Left; iCol < rectangle.Right; iCol++)
                    Pixels[iRow][iCol] = state;
        }

        public PixelState[] GetRow(int iRow)
            => Pixels[iRow];

        public PixelState[] GetColumn(int iCol)
            => Pixels.Select(row => row[iCol]).ToArray();

        public void SetRow(int iRow, PixelState[] states)
        {
            if (states.Length != Size.Width)
                throw new ArgumentException($"Array length should be {Size.Width}, not {states.Length}!");
            for (int iCol = 0; iCol < states.Length; iCol++)
                Pixels[iRow][iCol] = states[iCol];
        }

        public void SetColumn(int iCol, PixelState[] states)
        {
            if (states.Length != Size.Height)
                throw new ArgumentException($"Array length should be {Size.Height}, not {states.Length}!");
            for (int iRow = 0; iRow < states.Length; iRow++)
                Pixels[iRow][iCol] = states[iRow];
        }

        public void ShiftRow(int iRow, bool rotatePixels = true)
        {
            List<PixelState> row = [.. GetRow(iRow)];
            row.Insert(0, rotatePixels ? row[^1] : PixelState.Off);
            row.RemoveAt(row.Count - 1);
            SetRow(iRow, [.. row]);
        }

        public void ShiftRow(int iRow, int count, bool rotatePixels = true)
        {
            for (int i = 0; i < count; i++)
                ShiftRow(iRow, rotatePixels);
        }

        public void ShiftColumn(int iCol, bool rotatePixels = true)
        {
            List<PixelState> col = [.. GetColumn(iCol)];
            col.Insert(0, rotatePixels ? col[^1] : PixelState.Off);
            col.RemoveAt(col.Count - 1);
            SetColumn(iCol, [.. col]);
        }

        public void ShiftColumn(int iCol, int count, bool rotatePixels = true)
        {
            for (int i = 0; i < count; i++)
                ShiftColumn(iCol, rotatePixels);
        }

        public int CountPixelsWithState(PixelState state)
            => Pixels.Sum(row => row.Count(pixelState => pixelState == state));

        public string ToPrint()
            => string.Join("\n", Pixels.Select(row => new string(row.Select(pixel => (char)pixel).ToArray())));

        public void Print(string? title = null)
            => $"{(string.IsNullOrEmpty(title) ? string.Empty : $"{title}\n")}{ToPrint()}\n".Log();
    }
}