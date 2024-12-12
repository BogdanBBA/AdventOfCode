namespace AoC2015.Day24
{
    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 24;
        public override int GetCurrentPart => 0;

        private static List<int[]> GetWaysToPackPackages(int[] packageSet, int weight)
        {
            HashSet<string> temp = [];
            int[][] permutations = UtilsOther.GeneratePermutations(packageSet);
            foreach (int[] permutation in permutations)
            {
                for (int index = 0, sum = 0; index < permutation.Length; index++)
                {
                    sum += permutation[index];
                    if (sum == weight)
                    {
                        temp.Add(string.Join(",", permutation.Take(index).Select(x => x.ToString())));
                    }
                }
            }
            int minPackages = result.Min(variant => variant.Length);
            return result.Where(variant => variant.Length == minPackages).ToList();
        }

        public override void Run()
        {
            int[] weights = File.ReadAllLines(GetInputFilePath(GetCurrentPart)).Select(int.Parse).ToArray();
            int third = weights.Sum() / 3;
            Console.WriteLine($" > There are {weights.Length} packages weighing {weights.Sum()} in total, meaning the weight of one compartment should be {third}.");

            var passengerCompartment = GetWaysToPackPackages(weights, third);
        }
    }
}