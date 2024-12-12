namespace AoC2015
{
    public static class UtilsOther
    {
        private static void Swap<TYPE>(ref TYPE[] array, int i, int j)
            => (array[j], array[i]) = (array[i], array[j]);

        public static TYPE[][] GeneratePermutations<TYPE>(TYPE[] array)
            => GeneratePermutations(array, array.Length);

        public static TYPE[][] GeneratePermutations<TYPE>(TYPE[] array, int howManyToSelect)
        {
            List<TYPE[]> result = [];

            // c is an encoding of the stack state. c[k] encodes the for-loop counter for when generate(k - 1, A) is called
            int[] c = new int[howManyToSelect];

            result.Add([.. array]);

            // i acts similarly to a stack pointer
            for (int i = 1; i < howManyToSelect;)
            {
                if (c[i] < i)
                {
                    if (i % 2 == 0)
                        Swap(ref array, 0, i);
                    else
                        Swap(ref array, c[i], i);
                    result.Add([.. array]);
                    // Swap has occurred ending the for-loop. Simulate the increment of the for-loop counter
                    c[i]++;
                    // Simulate recursive call reaching the base case by bringing the pointer to the base case analog in the array
                    i = 1;
                }
                else
                {
                    // Calling generate(i+1, A) has ended as the for-loop terminated. Reset the state and simulate popping the stack by incrementing the pointer.
                    c[i] = 0;
                    i++;
                }
            }

            // return [.. result.OrderBy(x => x[0]).ThenBy(x => x[1]).ThenBy(x => x[2]).ThenBy(x => x[3]).ThenBy(x => x[4])];
            return [.. result];
        }

        private static void GenerateDigitPermutations(int[] digits, string current, int length, ref HashSet<string> result)
        {
            if (current.Length == length)
            {
                result.Add(current);
                return;
            }

            foreach (int digit in digits)
            {
                GenerateDigitPermutations(digits, current + digit, length, ref result);
            }
        }

        public static List<int[]> GenerateDigitPermutations(int[] digits, int length)
        {
            HashSet<string> result = [];
            GenerateDigitPermutations(digits, string.Empty, length, ref result);
            return result.OrderBy(s => s).Select(result => result.Select(@char => int.Parse(@char.ToString())).ToArray()).ToList();
        }

        private static void PopulatePartitions(int count, int total, ref List<List<int>> result, ref List<int> current/*, int start*/)
        {
            if (current.Count == count)
            {
                if (current.Sum() == total)
                {
                    result.Add(new List<int>(current));
                }
                return;
            }

            for (int i = 0/*start*/; i <= total; i++)
            {
                current.Add(i);
                PopulatePartitions(count, total, ref result, ref current/*, i*/);
                current.RemoveAt(current.Count - 1);
            }
        }

        public static List<List<int>> GeneratePartitions(int count, int total)
        {
            List<List<int>> result = [];
            List<int> current = [];
            PopulatePartitions(count, total, ref result, ref current/*, 0*/);
            if (result.Any(item => item.Count != count || item.Sum() != total))
                throw new Exception("Incorrect partition algorithm!");
            return result;
        }

        public static bool StartsWith(this int[] list, int[] sublist)
        {
            if (sublist.Length > list.Length)
                return false;
            for (int index = 0; index < sublist.Length; index++)
            {
                if (list[index] != sublist[index])
                    return false;
            }
            return true;
        }
    }
}