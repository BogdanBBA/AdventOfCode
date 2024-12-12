namespace AoC2015.Day17
{
    public class Container(int id, int size)
    {
        public int ID { get; private set; } = id;
        public int Size { get; private set; } = size;

        public override string ToString()
            => $"{ID}:{Size}L";
    }

    public class ContainerList(params Container[] containers)
    {
        public Container[] Containers { get; private set; } = containers;

        public override string ToString()
            => string.Join(", ", Containers.Select(container => container.ToString()));
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 17;
        public override int GetCurrentPart => 1;

        private static void Solve_ChatGPT(Container[] containers, int targetVolume)
        {
            static int CountCombinations(Container[] containers, int remainingVolume, int startIndex)
            {
                if (remainingVolume == 0) // Found a valid combination
                {
                    Console.WriteLine($"-Valid: {string.Join(", ", containers.Select(c => c.Size))} (sum {containers.Sum(c => c.Size)})");
                    return 1;
                }

                if (remainingVolume < 0 || startIndex >= containers.Length) return 0; // Invalid or no more containers to consider

                // Include the current container
                int include = CountCombinations(containers, remainingVolume - containers[startIndex].Size, startIndex + 1);

                // Exclude the current container
                int exclude = CountCombinations(containers, remainingVolume, startIndex + 1);

                return include + exclude; // Sum of both possibilities
            }

            int combinations = CountCombinations(containers, targetVolume, 0);
            Console.WriteLine($"Total combinations: {combinations}");
        }

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            Container[] containers = Enumerable.Range(0, lines.Length).Select(index => new Container(index, int.Parse(lines[index]))).ToArray();
            int totalSize = GetCurrentPart == 0 ? 25 : 150;

            Solve_ChatGPT(containers, totalSize);
        }
    }
}