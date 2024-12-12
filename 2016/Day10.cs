using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace AoC2016
{
    public abstract class Numbered(int number)
    {
        public int Number { get; } = number;

        public virtual void ReceiveChip(Chip chip)
            => throw new NotImplementedException();
    }

    public class Chip(int number) : Numbered(number)
    {
        public override string ToString()
            => $"{Number}";
    }

    public abstract class NumberedWithChips(int number) : Numbered(number)
    {
        protected List<Chip> _chips = [];

        public abstract override void ReceiveChip(Chip chip);

        public override string ToString()
            => $"{Number} {{{string.Join(",", _chips.Where(chip => chip is not null))}}}";
    }

    public class Bin(int number) : NumberedWithChips(number)
    {
        public override void ReceiveChip(Chip chip)
            => _chips.Add(chip);

        public override string ToString()
            => $"Bin-{base.ToString()}";
    }

    public class Bot(int number) : NumberedWithChips(number)
    {
        public override void ReceiveChip(Chip chip)
        {
            if (_chips.Count == 2)
                throw new InvalidOperationException($"Bot {Number} full!");
            _chips = [.. _chips.Append(chip).OrderBy(x => x.Number)];
        }

        public (Chip low, Chip high) GiveAwayChips()
        {
            if (_chips.Count != 2)
                throw new InvalidOperationException($"Bot {Number} not full!");
            Chip low = _chips[0], high = _chips[1];
            _chips.Clear();
            return (low, high);
        }

        public override string ToString()
            => $"Bot-{base.ToString()}";
    }

    public class NumberedDictionary<TYPE>(Func<int, TYPE> newInstanceFunc)
    {
        private readonly Func<int, TYPE> _newInstanceFunc = newInstanceFunc;
        private readonly Dictionary<int, TYPE> _dict = [];

        public int Count => _dict.Count;

        public List<KeyValuePair<int, TYPE>> KeyValuePairs => [.. _dict.OrderBy(x => x.Key)];

        public List<TYPE> Values => [.. _dict.OrderBy(x => x.Key).Select(x => x.Value)];

        public TYPE GetByNumber(int number)
        {
            if (!_dict.TryGetValue(number, out TYPE? item))
            {
                item = _newInstanceFunc.Invoke(number);
                _dict.Add(number, item);
            }
            return item;
        }
    }

    public class BotFactory(int watchChipValueA, int watchChipValueB)
    {
        private readonly int _watchChipValueLow = Math.Min(watchChipValueA, watchChipValueB);
        private readonly int _watchChipValueHigh = Math.Max(watchChipValueA, watchChipValueB);

        private static bool ExecuteIfMatches(string instruction, string pattern, Action<Match> executor)
        {
            Match match = Regex.Match(instruction, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
                executor.Invoke(match);
            return match.Success;
        }

        private NumberedDictionary<Bin> Bins { get; } = new(number => new(number));
        private NumberedDictionary<Bot> Bots { get; } = new(number => new(number));

        private void RunInstruction_ValueGoesToBot(Match match)
        {
            Chip chip = new(int.Parse(match.Groups[1].Value));
            Bot bot = Bots.GetByNumber(int.Parse(match.Groups[2].Value));
            bot.ReceiveChip(chip);
        }

        public void RunInstruction_BotGivesLowHighChips(Match match)
        {
            Bot bot = Bots.GetByNumber(int.Parse(match.Groups[1].Value));
            (string chipA, string destTypeA, int destNumberA) = (match.Groups[2].Value, match.Groups[3].Value, int.Parse(match.Groups[4].Value));
            (string chipB, string destTypeB, int destNumberB) = (match.Groups[5].Value, match.Groups[6].Value, int.Parse(match.Groups[7].Value));

            if (!((chipA == "low" || chipB == "low") && (chipA == "high" || chipB == "high")))
                throw new InvalidOperationException($"Invalid instruction (chips: {chipA}, {chipB})!");
            if (!((destTypeA == "bot" || destTypeA == "output") && (destTypeB == "bot" || destTypeB == "output")))
                throw new InvalidOperationException($"Invalid instruction (destinations: {destTypeA}, {destTypeB})!");

            (Chip low, Chip high) = bot.GiveAwayChips();
            if (low.Number == _watchChipValueLow && high.Number == _watchChipValueHigh)
                $" - bot {bot.Number} is giving away chips {low.Number} and {high.Number}".Log();
            Numbered destA = destTypeA == "bot" ? Bots.GetByNumber(destNumberA) : Bins.GetByNumber(destNumberA);
            Numbered destB = destTypeB == "bot" ? Bots.GetByNumber(destNumberB) : Bins.GetByNumber(destNumberB);

            destA.ReceiveChip(chipA == "low" ? low : high);
            destB.ReceiveChip(chipB == "low" ? low : high);
        }

        public void RunInstructions(string[] instructions)
        {
            Dictionary<string, bool> executionDict = instructions.ToDictionary(x => x, _ => false);
            foreach (string instruction in instructions)
                if (ExecuteIfMatches(instruction, @"value (\d+) goes to bot (\d+)", RunInstruction_ValueGoesToBot))
                    executionDict[instruction] = true;
            foreach (string instruction in instructions)
                if (ExecuteIfMatches(instruction, @"bot (\d+) gives (\w+) to (\w+) (\d+) and (\w+) to (\w+) (\d+)", RunInstruction_BotGivesLowHighChips))
                    executionDict[instruction] = true;
            string[] unexecutedInstructions = [.. executionDict.Where(pair => !pair.Value).Select(pair => pair.Key)];
            if (unexecutedInstructions.Length > 0)
                throw new Exception($"There are unexecuted instructions ({string.Join(", ", unexecutedInstructions.Select(i => $"'{i}'"))})!");
        }

        public override string ToString()
            => $" > Bins ({Bins.Count}): {string.Join(", ", Bins.Values)}\n > Bots ({Bots.Count}): {string.Join(", ", Bots.Values)}";
    }

    public class Day10 : IDayProgram
    {
        public override void Run()
        {
            string[] instructions = ParseFromFile(@"10", line => line);
            BotFactory factory = new(61, 17); // 2,5
            factory.RunInstructions(instructions);
            $"{factory}".Log();
        }
    }
}