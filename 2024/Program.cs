global using System.Collections.Generic;

using System.Globalization;

namespace AoC2024
{
    public class Program
    {
        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            new Day14().RunWithTimer();
        }
    }
}