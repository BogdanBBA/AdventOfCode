using System.Text.RegularExpressions;

namespace AoC2024
{
    public static class Utils
    {
        public static void Swap<TYPE>(ref TYPE[] array, int i, int j)
            => (array[j], array[i]) = (array[i], array[j]);

        public static void Swap<TYPE>(ref List<TYPE> list, int i, int j)
            => (list[j], list[i]) = (list[i], list[j]);

        public static int FirstIndexOf<TYPE>(this TYPE[] array, Predicate<TYPE> predicate)
        {
            for (int index = 0; index < array.Length; index++)
                if (predicate(array[index]))
                    return index;
            return -1;
        }

        public static int LastIndexOf<TYPE>(this TYPE[] array, Predicate<TYPE> predicate)
        {
            for (int index = array.Length - 1; index >= 0; index--)
                if (predicate(array[index]))
                    return index;
            return -1;
        }

        public static IEnumerable<TYPE[]> GenerateCombinationsOf2<TYPE>(TYPE a, TYPE b, int n)
        {
            int totalCombinations = 1 << n; // 2^n
            for (int i = 0; i < totalCombinations; i++)
            {
                var combination = new TYPE[n];
                for (int bit = 0; bit < n; bit++)
                {
                    combination[bit] = (i & (1 << bit)) == 0 ? a : b;
                }
                yield return combination;
            }
        }

        public static IEnumerable<TYPE[]> GenerateCombinationsOfN<TYPE>(TYPE[] values, int n)
        {
            int totalCombinations = (int)Math.Pow(values.Length, n); // N^n
            for (int i = 0; i < totalCombinations; i++)
            {
                var combination = new TYPE[n];
                int current = i;
                for (int position = 0; position < n; position++)
                {
                    combination[position] = values[current % 3];
                    current /= values.Length;
                }
                yield return combination;
            }
        }

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

        public static List<List<int>> GeneratePartitions(int count, int total)
        {
            List<List<int>> result = [];
            List<int> current = [];
            PopulatePartitions(count, total, ref result, ref current/*, 0*/);
            if (result.Any(item => item.Count != count || item.Sum() != total))
                throw new Exception("Incorrect partition algorithm!");
            return result;
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

        public static TYPE? GetFirstDuplicate<TYPE>(this IEnumerable<TYPE> list)
        {
            HashSet<TYPE> unique = [];
            foreach (TYPE item in list)
            {
                if (unique.Contains(item))
                    return item;
                unique.Add(item);
            }
            return default;
        }

        private static int[] BuildPrefixArray<TYPE>(TYPE[] sequence)
            where TYPE : class
        {
            int[] prefixArray = new int[sequence.Length];
            int j = 0;

            for (int i = 1; i < sequence.Length; i++)
            {
                while (j > 0 && !sequence[i].Equals(sequence[j]))
                {
                    j = prefixArray[j - 1];
                }

                if (sequence[i].Equals(sequence[j]))
                {
                    j++;
                }

                prefixArray[i] = j;
            }

            return prefixArray;
        }

        public static bool HasRepeatingPattern<TYPE>(IEnumerable<TYPE> coords)
            where TYPE : class
        {
            int itemCount = coords.Count();
            if (itemCount < 2)
                return false;

            // Create an extended list to simulate circular behavior
            List<TYPE> extendedList = [.. coords, .. coords];

            // Use prefix-suffix matching to detect the pattern
            int[] prefixArray = BuildPrefixArray(coords.ToArray());

            for (int i = 1; i < itemCount; i++)
            {
                // Check if the prefix-suffix length matches
                if (prefixArray[i] > 0 && itemCount % i == 0)
                {
                    return true; // Repeating pattern found
                }
            }

            return false; // No repeating pattern
        }

        public static int ParseGroupAsInt(this Match match, int groupNumber)
            => int.Parse(match.Groups[groupNumber].Value);

        public static uint GetNumberOfDigits(this uint number)
            => (uint)(Math.Floor(Math.Log10(number)) + 1);

        public static uint GetNumberOfDigits(this long number)
            => (uint)(Math.Floor(Math.Log10(number)) + 1);

        public static uint GetNumberOfDigits(this ulong number)
            => (uint)(Math.Floor(Math.Log10(number)) + 1);
    }
}