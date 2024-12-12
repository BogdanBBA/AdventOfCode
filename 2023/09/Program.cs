using System.Text.RegularExpressions;

internal class Program
{
    private static long TheRecursiveOne(List<long> inSequence, bool intoTheFuture)
    {
        List<long> diff = [];
        for (int index = 0; index < inSequence.Count - 1; index++)
            diff.Add(inSequence[index + 1] - inSequence[index]);
        if (diff.All(val => val == 0L))
            return intoTheFuture ? inSequence.Last() : inSequence.First();
        return intoTheFuture ? inSequence.Last() : inSequence.First() + (intoTheFuture ? 1 : -1) * TheRecursiveOne(diff, intoTheFuture);
    }

    private static void Main(string[] _)
    {
        List<List<long>> sequences = File.ReadAllLines("input1.txt")
            .Where(line => line.Length > 0 && !line.StartsWith('/'))
            .Select(line => line.Split(' ').Select(long.Parse).ToList())
            .ToList();
        Console.WriteLine($" > Read {sequences.Count} sequences.");

        List<long> futureValues = sequences.Select(sequence => TheRecursiveOne(sequence, true)).ToList();
        Console.WriteLine($" > Sum of the {futureValues.Count} future values: {string.Join(" + ", futureValues)} = {futureValues.Sum()}");

        List<long> pastValues = sequences.Select(sequence => TheRecursiveOne(sequence, false)).ToList();
        Console.WriteLine($" > Sum of the {pastValues.Count} past values: {string.Join(" + ", pastValues)} = {pastValues.Sum()}");
    }
}