namespace AoC2024
{
    public abstract class DayProgram
    {
        public static string ReadFromFile_String(string name)
            => File.ReadAllText($"inputs\\{name}");
        public static string[] ReadFromFile_Strings(string name)
            => File.ReadAllLines($"inputs\\{name}");
        public static TYPE[] ParseFromFile<TYPE>(string name, Func<string, TYPE> lineParser)
            => ReadFromFile_Strings(name).Select(lineParser).ToArray();
        public static TYPE[] ParseFromFileCsv<TYPE>(string name, Func<string, TYPE> csvItemParser, string separator = ",", StringSplitOptions stringSplitOptions = StringSplitOptions.RemoveEmptyEntries)
            => ReadFromFile_String(name).Split(separator, stringSplitOptions).Select(item => csvItemParser(item.Trim())).ToArray();
        public static void WriteToFile_String(string name, string contents)
            => File.WriteAllText($"outputs\\{name}", contents);
        public static void WriteToFile_Strings(string name, string[] contents)
            => File.WriteAllLines($"outputs\\{name}", contents);

        private readonly DateTime _programExecutionStart = DateTime.Now;

        protected TimeSpan TimeElapsed
            => DateTime.Now - _programExecutionStart;

        /// <summary>Make sure to pass in non-null item count.</summary>
        protected TimeSpan GetETA(int items, int total)
        {
            if (items == 0)
                return TimeSpan.Zero;
            if (items > total)
                throw new ArgumentException("Processed items cannot exceed total items.");
            double processingRate = items / TimeElapsed.TotalSeconds;
            int remainingItems = total - items;
            double remainingTimeInSeconds = remainingItems / processingRate;
            return TimeSpan.FromSeconds(remainingTimeInSeconds);
        }

        public void RunWithTimer()
        {
            Run();
            HandleTime(TimeElapsed);
        }

        public abstract void Run();

        // overwrite to handle
        public virtual void HandleTime(TimeSpan totalExecutionDuration)
        {
            double ms = totalExecutionDuration.TotalMilliseconds;
            $" > Total program execution took {(ms <= 1000.0 ? $"{ms:N1} ms" : $"{ms / 1000:N3} s")}.".Log();
        }
    }
}