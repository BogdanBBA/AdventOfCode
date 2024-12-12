using System.Globalization;

namespace AoC12
{
    internal class Program
    {
        public static (ulong Count, TimeSpan ProcessingDuration) GetPossibleArrangements_Reverse(List<Record> records)
        {
            ulong count = 0;
            TimeSpan duration = TimeSpan.Zero;
            for (int index = 0; index < records.Count; index++)
            {
                Record record = records[index];
                Console.Write($" - {record}\n   processing ... ");
                DateTime recordStart = DateTime.Now;
                ulong recordCount = record.CountPossibleArrangements_Reverse();
                TimeSpan recordDuration = DateTime.Now.Subtract(recordStart);
                duration.Add(recordDuration);
                count += recordCount;
                Console.WriteLine($"got {recordCount:N0} arrangements, in {recordDuration.TotalSeconds:N1} seconds\n");
            }
            return (count, duration);
        }

        private static void Main(string[] _)
        {
            bool quintuplicate = false;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string[] lines = File.ReadAllLines("input0.txt");
            List<Record> records = Enumerable.Range(0, lines.Length).Select(index => new Record(index + 1, lines[index], quintuplicate)).ToList();

            (ulong Count, TimeSpan ProcessingDuration) = GetPossibleArrangements_Reverse(records);
            Console.WriteLine($" > All possible arrangements count (result): {Count:N0} (processed in a total of {ProcessingDuration.TotalSeconds:N1}s) // {Count}\n");
        }
    }
}