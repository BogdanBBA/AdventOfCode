using System.Text;
using System.Xml.XPath;

namespace AoC12
{
    public static class Extensions
    {
        public static RecordItem ToRecordItem(this char character) => character switch
        {
            'A' => RecordItem.Operational,
            'B' => RecordItem.Damaged,
            '?' => RecordItem.Unknown,
            _ => throw new ArgumentException(character.ToString())
        };

        public static char ToFormatted(this RecordItem recordItem) => recordItem switch
        {
            RecordItem.Operational => 'A',
            RecordItem.Damaged => 'B',
            RecordItem.Unknown => '?',
            _ => throw new ArgumentException(recordItem.ToString())
        };

        public static string GetRecordsListing(this List<Record> records, bool print, string prefix = "")
        {
            if (!print) return " (not listed)";
            return prefix + string.Join("\n", records.Select(rec => $" - {rec}"));
        }

        public static ulong GetEstimateOfPossibleArrangements(this List<Record> records)
        {
            return records
                .Select(record => (ulong)Math.Pow(2, record.RecordItems.Count(item => item == RecordItem.Unknown)))
                .Aggregate((total, next) => total + next);
        }

        public static List<int> ReduceRecordItemsToCounts(this Record record)
        {
            List<int> result = [];
            bool previousWasDamaged = false;
            int count = 0;
            foreach (RecordItem item in record.RecordItems)
            {
                switch (item)
                {
                    case RecordItem.Damaged:
                        if (previousWasDamaged)
                            count++;
                        else
                            count = 1;
                        previousWasDamaged = true;
                        break;
                    case RecordItem.Operational:
                        if (previousWasDamaged)
                        {
                            result.Add(count);
                        }
                        previousWasDamaged = false;
                        break;
                    case RecordItem.Unknown:
                        return result;
                }
            }
            if (previousWasDamaged)
                result.Add(count);
            return result;
        }

        public static Validity DetermineValidity(this Record record)
        {
            if (record.RecordItems.Count == 0 || record.DamagedUnitCounts.Length == 0)
                throw new ArgumentException($"Invalid record '{record}'!");

            List<int> reducedCounts = record.ReduceRecordItemsToCounts();

            if (reducedCounts.Count > record.DamagedUnitCounts.Length) // assume correct decode
                return Validity.No_UnitCountMismatch;

            for (int index = 0; index < reducedCounts.Count; index++)
                if (reducedCounts[index] != record.DamagedUnitCounts[index])
                    return Validity.No_UnitCountMismatch;

            if (reducedCounts.Count < record.DamagedUnitCounts.Length) // > addressed above, == is ok
            {
                if (record.CountUnknownItems > 0)
                    return Validity.Maybe_UnknownItems;
                return Validity.No_UnitCountMismatch;
            }

            return Validity.Yes;
        }

        public static (Record PathA, Record PathB) ExpandFirstUnknownOnly(this Record record)
        {
            int index = record.RecordItems.IndexOf(RecordItem.Unknown);
            if (index == -1) throw new ArgumentException($"Record {record} has no unknown items, can't expand!");
            return (new(record, index, RecordItem.Operational), new(record, index, RecordItem.Damaged));
        }

        public static ulong CountPossibleArrangements(this Record record, ref ulong computed)
        {
            if (record.CountUnknownItems == 0)
            {
                computed += 1;
                return record.DetermineValidity() == Validity.Yes ? 1UL : 0UL;
            }

            (Record PathA, Record PathB) = record.ExpandFirstUnknownOnly();
            ulong countA, countB;

            if (PathA.DetermineValidity() == Validity.No_UnitCountMismatch)
            {
                computed += PathA.EstimatePossibleArrangements;
                countA = 0UL;
            }
            else
            {
                countA = PathA.CountPossibleArrangements(ref computed);
            }

            if (PathB.DetermineValidity() == Validity.No_UnitCountMismatch)
            {
                computed += PathB.EstimatePossibleArrangements;
                countB = 0UL;
            }
            else
            {
                countB = PathB.CountPossibleArrangements(ref computed);
            }

            return countA + countB;
        }

        //

        public static string[] ToDucStrings(this Record record)
        {
            string[] result = new string[record.DamagedUnitCounts.Length];
            for (int iDUC = 0; iDUC < record.DamagedUnitCounts.Length; iDUC++)
            {
                result[iDUC] = new string(RecordItem.Damaged.ToFormatted(), record.DamagedUnitCounts[iDUC]);
            }
            return result;
        }

        private static List<(int Start, int Middle, int End)> GetAllStartMiddleEndPermutations(int freeItems)
        {

            List<(int, int, int)> result = [];
            for (int endOperationalItems = 0; endOperationalItems < freeItems; endOperationalItems++)
            {
                for (int startOperationalItems = 0; startOperationalItems < freeItems - endOperationalItems; startOperationalItems++)
                {
                    int freeMiddleOperationalItems = freeItems - startOperationalItems - endOperationalItems;
                    result.Add((startOperationalItems, freeMiddleOperationalItems, endOperationalItems));
                }
            }
            return result;
        }

        private static ulong Factorial(ulong number)
            => number <= 1UL ? number : number * Factorial(number - 1UL);

        private static ulong Combinari(ulong n, ulong k)
        {
            if (n < k) throw new ArgumentException($"n={n} < k={k}");
            return Factorial(n) / Factorial(k) * Factorial(n - k);
        }

        public static ulong CountPossibleArrangements_Reverse(this Record record)
        {
            int turdGroups = record.DamagedUnitCounts.Length;
            int middleBaskets = turdGroups - 1;

            int applesAndTurds = record.RecordItems.Count;

            int turds = record.DamagedUnitCounts.Sum();
            int apples = applesAndTurds - turds;
            int applesToGive = apples - middleBaskets;

            List<(int Start, int Middle, int End)> appleCountsPermutations = GetAllStartMiddleEndPermutations(applesToGive);
            List<ulong> middleCombinations = appleCountsPermutations
                .Select(permutation => Combinari((ulong)(permutation.Middle + middleBaskets), (ulong)middleBaskets))
                .ToList();

            Console.Write($"total={applesAndTurds}, turds={turds}, apples={apples}, appledToGive={applesToGive}; SME permutations: {string.Join(", ", appleCountsPermutations.Select(x => $"({x.Start}, {x.Middle}, {x.End})"))}; middle combinations: {string.Join(", ", middleCombinations)}\n   ");

            ulong result = Enumerable.Range(0, appleCountsPermutations.Count)
                .Select(index =>
                {
                    ulong result = (ulong)appleCountsPermutations[index].Middle * middleCombinations[index];
                    Console.Write($"{appleCountsPermutations[index].Middle}x{middleCombinations[index]}={result}; ");
                    return result;
                })
                .Aggregate((total, next) => total + next);

            return result;
        }
    }
}