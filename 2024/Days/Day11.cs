using AoC2024.ForDay11;

namespace AoC2024
{
    namespace ForDay11
    {
        public class NumberFrequencyMap
        {
            private readonly Dictionary<ulong, ulong> _map;

            public NumberFrequencyMap(List<ulong> numbers)
            {
                if (numbers.Count != numbers.Distinct().Count())
                    throw new ArgumentException("Unsupported: repeating numbers!");
                _map = numbers.ToDictionary(n => n, _ => 1ul);
            }

            public ulong TotalNumberCount
                => _map.Values.Aggregate(0ul, (x, y) => x + y);

            private void AddOrSetInMap(ulong key, ulong value)
                => _map[key] = _map.TryGetValue(key, out ulong newValueCount) ? newValueCount + value : value;

            private void BlinkOnce()
            {
                KeyValuePair<ulong, ulong>[] data = [.. _map];
                _map.Clear();

                foreach ((ulong number, ulong numberCount) in data)
                {
                    if (number == 0ul)
                    {
                        AddOrSetInMap(1ul, numberCount);
                        continue;
                    }

                    ulong nDigits = number.GetNumberOfDigits();
                    if (nDigits % 2 == 0)
                    {
                        ulong divisor = (ulong)Math.Pow(10, nDigits / 2);
                        AddOrSetInMap(number / divisor, numberCount);
                        AddOrSetInMap(number % divisor, numberCount);
                        continue;
                    }

                    AddOrSetInMap(number * 2024ul, numberCount);
                }
            }

            public void Blink(int count)
            {
                $" > In the beginning, there were {TotalNumberCount} numbers in the list.".Log();
                for (int blinkCounter = 1; blinkCounter <= count; blinkCounter++)
                {
                    $"   - blink {blinkCounter,2}: ".LogNNL();
                    BlinkOnce();
                    $"we now have {_map.Keys.Count,5:N0} distinct numbers representing {TotalNumberCount,19:N0} numbers in total".Log();
                }
                $" > At the end, there are {TotalNumberCount} numbers in the list.".Log();
            }
        }
    }

    public class Day11 : DayProgram
    {
        private const string INPUT = "2 54 992917 5270417 2514 28561 0 990";
        private const int NUMBER_OF_BLINKS = 75;

        public override void Run()
        {
            List<ulong> numbers = [.. INPUT.Split(' ').Select(ulong.Parse)];
            new NumberFrequencyMap(numbers).Blink(NUMBER_OF_BLINKS);
        }
    }
}