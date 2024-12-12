namespace AdventOfCode01
{
    internal class Program
    {
        private static void Main(string[] _)
        {
            Calculator2 calculator = new();
            Console.WriteLine($"TOTAL: {File.ReadAllLines(@"in.txt").Select(Calculator2.ProcessLine).Sum()}");
        }
    }
}