using AoC2024.ForDay07;

namespace AoC2024
{
    namespace ForDay07
    {
        public enum Operator { Add = '+', Multiply = '*', Concatenate = '|' }

        public class Calculation(ulong result, ulong[] operands)
        {
            public static Calculation Parse(string line)
            {
                string[] parts = line.Split(':', StringSplitOptions.TrimEntries);
                string[] ops = parts[1].Split(' ', StringSplitOptions.TrimEntries);
                return new(ulong.Parse(parts[0]), ops.Select(ulong.Parse).ToArray());
            }

            public ulong Result { get; } = result;
            public ulong[] Operands { get; } = operands;
        }
    }

    public class Day07 : DayProgram
    {
        private static ulong Calculate(Calculation calculation, Operator[] operators)
        {
            if (calculation.Operands.Length != operators.Length + 1)
                throw new ArgumentException("Invalid number of operands and operators!");
            ulong result = calculation.Operands[0];
            for (int index = 0; index < operators.Length; index++)
            {
                switch (operators[index])
                {
                    case Operator.Add:
                        result += calculation.Operands[index + 1];
                        break;
                    case Operator.Multiply:
                        result *= calculation.Operands[index + 1];
                        break;
                    case Operator.Concatenate:
                        ulong other = calculation.Operands[index + 1];
                        uint nDigits = other.GetNumberOfDigits();
                        result = (ulong)(result * Math.Pow(10, nDigits) + other); //ulong.Parse($"{result}{calculation.Operands[index + 1]}");
                        break;
                    default:
                        throw new ArgumentException($"Invalid operator {operators[index]} ('{(char)operators[index]}')!");
                }
            }
            return result;
        }

        private static bool EvaluateTrue(Calculation calculation, Operator[] operators, out ulong result, bool logging = false)
        {
            result = Calculate(calculation, operators);
            if (logging) $" - calculated {result} vs. expected {calculation.Result} // {calculation.Operands[0]}{string.Join(string.Empty, Enumerable.Range(0, operators.Length).Select(i => $"{(char)operators[i]}{calculation.Operands[i + 1]}"))}".Log();
            return result == calculation.Result;
        }

        private static ulong SolvePuzzle(Calculation[] calculations)
        {
            ulong sum = 0;
            foreach (Calculation calculation in calculations)
            {
                if (calculation.Operands.Length < 2)
                    continue;
                foreach (Operator[] operatorPossibility in Utils.GenerateCombinationsOfN([Operator.Add, Operator.Multiply, Operator.Concatenate], calculation.Operands.Length - 1))
                {
                    if (EvaluateTrue(calculation, operatorPossibility, out ulong result))
                    {
                        sum += result;
                        break;
                    }
                }
            }
            return sum;
        }

        public override void Run()
        {
            Calculation[] calculations = ParseFromFile(@"07", Calculation.Parse);

            $" > The result is {SolvePuzzle(calculations)}.".Log();
        }
    }
}