namespace AoC2024
{
    public class Day01 : DayProgram
    {
        private static (int, int) ParseItemPair(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return (int.Parse(parts[0]), int.Parse(parts[1]));
        }

        public override void Run()
        {
            (int First, int Second)[] items = ParseFromFile(@"01", ParseItemPair);
            int[] first = [.. items.Select(item => item.First).OrderBy(x => x)];
            int[] second = [.. items.Select(item => item.Second).OrderBy(x => x)];
            int diffSum = Enumerable.Range(0, first.Length).Select(index => Math.Abs(first[index] - second[index])).Sum();

            $" > The sum of the differences between the sorted values of the list is: {diffSum}.".Log();

            int similarityScore = first.Select(a => a * second.Count(b => a == b)).Sum();
            $" > The similarity score of the two lists is {similarityScore}.".Log();
        }
    }
}