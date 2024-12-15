using System.Text.RegularExpressions;
using AoC2024.ForDay03;

namespace AoC2024
{
    namespace ForDay03
    {
        public enum InstructionType { Do, Dont, Multiply };

        public record Instruction(InstructionType Type);

        public record MultiplyInstruction(InstructionType Type, int X, int Y) : Instruction(Type)
        {
            public int GetResult => X * Y;
        }
    }

    public class Day03 : DayProgram
    {
        private static MatchCollection ExtractInstructionsMatches(string input)
            => Regex.Matches(input, @"mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)", RegexOptions.IgnorePatternWhitespace);

        private static List<Instruction> ParseInstructions(MatchCollection matches)
            => matches.Select(m =>
            {
                if (m.Value.Contains("do()"))
                    return new Instruction(InstructionType.Do);
                if (m.Value.Contains("don't()"))
                    return new Instruction(InstructionType.Dont);
                return new MultiplyInstruction(InstructionType.Multiply, int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
            }).ToList();

        public static int CalculateTotalResult(List<Instruction> instructions, bool processDosAndDonts)
        {
            if (!processDosAndDonts)
            {
                return instructions.Where(i => i.Type == InstructionType.Multiply).Cast<MultiplyInstruction>().Sum(i => i.GetResult);
            }

            int sum = 0;
            bool active = true;
            foreach (Instruction instruction in instructions)
            {
                if (instruction.Type == InstructionType.Do)
                {
                    active = true;
                }
                else if (instruction.Type == InstructionType.Dont)
                {
                    active = false;
                }
                else if (instruction.Type == InstructionType.Multiply)
                {
                    if (active)
                    {
                        sum += (instruction as MultiplyInstruction)!.GetResult;
                    }
                }
                else
                    throw new NotImplementedException();
            }
            return sum;
        }

        public override void Run()
        {
            string input = ReadFromFile_String(@"03");
            MatchCollection matches = ExtractInstructionsMatches(input);
            List<Instruction> instructions = ParseInstructions(matches);
            Dictionary<InstructionType, int> countDict = instructions.GroupBy(i => i.Type).ToDictionary(g => g.Key, g => g.Count());

            $" > The sum of all the {countDict[InstructionType.Multiply]} instruction multiplications is: {CalculateTotalResult(instructions, false)}".Log();
            $" > The sum of the multiplications, while considering the {countDict[InstructionType.Do]} 'do' and {countDict[InstructionType.Dont]} 'don't' instructions, is: {CalculateTotalResult(instructions, true)}".Log();
        }
    }
}