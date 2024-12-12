namespace AoC2015.Day20
{
    public class Program : IDayProgram
    {
        private static int CalculatePresentCountForHouse(int houseNumber)
            => Enumerable.Range(1, houseNumber)
                .Where(elfNumber => houseNumber % elfNumber == 0)
                .Select(elfNumber => elfNumber * 10)
                .Sum();

        private static int CalculatePresentCountForHouse_ChatGPT(int houseNumber, ref int maxElfNumber)
        {
            int sum = 0;
            int sqrt = (int)Math.Sqrt(houseNumber);

            for (int elfNumber = 1; elfNumber <= sqrt; elfNumber++)
            {
                if (maxElfNumber < elfNumber) maxElfNumber = elfNumber;
                if (houseNumber % elfNumber == 0)
                {
                    sum += elfNumber * 10;

                    // for any divisor d of a number n, a divisor d' exists such that d*d'=n, so d'=n/d
                    // consider and add complementary divisor, such as 5 in 15 (1 .. 3.87) - which is why we can only go up to sqrt(n)
                    // unless complementary divisor is the same as current divisor (in perfect squares, such as 6 in 36)
                    if (elfNumber != houseNumber / elfNumber)
                    {
                        sum += houseNumber / elfNumber * 10;
                    }
                }
            }

            return sum;
        }

        private static void Solve_Part1()
        {
            DateTime start = DateTime.Now, last = start;
            int maxElfNumber = 0;
            for (int houseNumber = 1; true; houseNumber++)
            {
                int presentCount = CalculatePresentCountForHouse_ChatGPT(houseNumber, ref maxElfNumber);
                if (presentCount >= PUZZLE_INPUT)
                {
                    Console.WriteLine($" > The first house to get at least {PUZZLE_INPUT,10:N0} presents is house number {houseNumber,7:N0}, with exactly {presentCount,10:N0} presents (max elf number {maxElfNumber:N0})");
                    break;
                }
                if (houseNumber % 10000 == 0)
                {
                    DateTime newNow = DateTime.Now;
                    double seconds = newNow.Subtract(last).TotalSeconds;
                    Console.WriteLine($" ... house {houseNumber,7:N0}: {presentCount,10:N0} presents ({seconds,4:N1}s, {100 * seconds,4:N1}s/million), max elf number {maxElfNumber:N0}");
                    last = newNow;
                }
            }
            Console.WriteLine($" > Processing took {DateTime.Now.Subtract(start).TotalSeconds:N1}s");
        }

        private static void Solve_Part2()
        {
            const int MaxElves = 1_000_000;
            DateTime start = DateTime.Now;
            Dictionary<int, int> presentsByHouse = [];
            for (int elfNumber = 1; elfNumber < MaxElves; elfNumber++)
            {
                for (int houseCount = 1, houseNumber = elfNumber; houseCount <= 50; houseCount++, houseNumber += elfNumber)
                {
                    presentsByHouse.TryAdd(houseNumber, 0);
                    presentsByHouse[houseNumber] += elfNumber * 11;
                }
            }

            Console.WriteLine($" > There are {presentsByHouse.Count:N0} houses, with present counts in range of {presentsByHouse.Values.Min():N0} to {presentsByHouse.Values.Max():N0}.");
            KeyValuePair<int, int>[] suchHouses = presentsByHouse.Where(house => house.Value >= PUZZLE_INPUT).ToArray();
            if (suchHouses.Length == 0)
            {
                Console.WriteLine($" > No house found!");
            }
            else
            {
                Console.WriteLine($" > Of them, {suchHouses.Length:N0} have received at least {PUZZLE_INPUT,10:N0} presents:");
                KeyValuePair<int, int> firstSuchHouse = suchHouses.MinBy(house => house.Key);
                for (int houseNumber = firstSuchHouse.Key - 5; houseNumber <= firstSuchHouse.Key + 5; houseNumber++)
                    Console.WriteLine($" ... house {houseNumber,7:N0}: {presentsByHouse[houseNumber],10:N0} presents");
                Console.WriteLine($" > The first house with at least {PUZZLE_INPUT,10:N0} presents is house number {firstSuchHouse.Key}, with exactly {firstSuchHouse.Value,10:N0} presents");
                Console.WriteLine($" > Processing took {DateTime.Now.Subtract(start).TotalSeconds:N1}s");
            }
        }

        private const int PUZZLE_INPUT = 36_000_000;

        public override int GetCurrentDay => 20;
        public override int GetCurrentPart => 2;

        public override void Run()
        {
            if (GetCurrentPart <= 1)
                Solve_Part1();
            else
                Solve_Part2();
        }
    }
}