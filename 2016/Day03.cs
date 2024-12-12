namespace AoC2016
{
    public class Day03 : IDayProgram
    {
        public record Numbers(int A, int B, int C);

        public static bool CanBeTriangle(Numbers numbers)
        {
            int[] ordered = [.. new int[] { numbers.A, numbers.B, numbers.C }.OrderBy(x => x)];
            return ordered[0] + ordered[1] > ordered[2];
        }

        public static int CanBeTriangles(Numbers[] numbers)
        {
            static int CanBeTriangle_01(Numbers n) => CanBeTriangle(n) ? 1 : 0;

            int count = 0;
            for (int index = 0; index < numbers.Length; index += 3)
            {
                Numbers[] set = numbers.Skip(index).Take(3).ToArray();
                count += CanBeTriangle_01(new Numbers(set[0].A, set[1].A, set[2].A));
                count += CanBeTriangle_01(new Numbers(set[0].B, set[1].B, set[2].B));
                count += CanBeTriangle_01(new Numbers(set[0].C, set[1].C, set[2].C));
            }
            return count;
        }

        public override void Run()
        {
            Numbers[] numbers = ParseFromFile(@"03", line =>
            {
                int[] nums = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
                return new Numbers(nums[0], nums[1], nums[2]);
            });
            $" > Out of the {numbers.Length} tuples of numbers, {numbers.Count(CanBeTriangle)} could be triangles.".Log();
            $" > Well, actually... {CanBeTriangles(numbers)} numbers in the design document you've marked out can be triangles.".Log();
        }
    }
}