global using System.Collections.Generic;

using System.Globalization;

namespace AoC2016
{
    public abstract class IDayProgram
    {
        public static string ReadFromFile_String(string name) => File.ReadAllText($"inputs\\{name}");
        public static string[] ReadFromFile_Strings(string name) => File.ReadAllLines($"inputs\\{name}");
        public static TYPE[] ParseFromFile<TYPE>(string name, Func<string, TYPE> lineParser) => ReadFromFile_Strings(name).Select(lineParser).ToArray();
        public static TYPE[] ParseFromFileCsv<TYPE>(string name, Func<string, TYPE> csvItemParser, string separator = ",") => ReadFromFile_String(name).Split(separator).Select(item => csvItemParser(item.Trim())).ToArray();
        public static void WriteToFile_String(string name, string contents) => File.WriteAllText($"outputs\\{name}", contents);
        public static void WriteToFile_Strings(string name, string[] contents) => File.WriteAllLines($"outputs\\{name}", contents);

        public void RunWithTimer()
        {
            DateTime start = DateTime.Now;
            Run();
            DateTime end = DateTime.Now;
            HandleTime(end - start);
        }

        public abstract void Run();

        // overwrite to handle
        public virtual void HandleTime(TimeSpan totalExecutionDuration)
        {
            double ms = totalExecutionDuration.TotalMilliseconds;
            $" > Total program execution took {(ms <= 1000.0 ? $"{ms:N1} ms" : $"{ms / 1000:N3} s")}.".Log();
        }
    }

    public class Program
    {
        private static void Main(string[] _)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            new Day10().RunWithTimer();
        }
    }
}