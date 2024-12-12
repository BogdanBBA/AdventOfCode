using System.Globalization;

namespace AoC2015
{
    public abstract class IDayProgram
    {
        public virtual int GetCurrentPart => 1;

        public abstract int GetCurrentDay { get; }

        public string GetCurrentDayString => $"{GetCurrentDay:D2}";

        public string GetInputFilePath(int number) => $"inputs\\{GetCurrentDayString}-{number}.txt";

        public string GetOutputFilePath(int number) => $"outputs\\{GetCurrentDayString}-{number}.txt";

        public abstract void Run();
    }

    public class Program
    {
        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            new Day24.Program().Run();
        }
    }
}