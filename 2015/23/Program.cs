namespace AoC2015.Day23
{
    public static class C
    {
        public static readonly bool Print = true;
    }

    public class Register(string name, uint initialValue)
    {
        public string Name { get; } = name;
        public uint Value { get; private set; } = initialValue;

        public void ApplyAndMoveForward(Func<uint, uint> TransformerFunction, ref int index)
        {
            Value = TransformerFunction(Value);
            index++;
            if (C.Print) Console.WriteLine($"registry {Name}'s value is now {Value}, index is now {index}");
        }

        public override string ToString() => $"{Name}: {Value}";
    }

    public class Instruction(Instruction.Types type, Register? register = null, int? offset = null)
    {
        public enum Types { Half, Triple, Increment, Jump, JumpIfEven, JumpIfOne }

        public Types Type { get; } = type;
        public Register? Register { get; } = register;
        public int? Offset { get; } = offset;

        public override string ToString() => $"{Type}: reg {Register?.Name ?? "<null>"}, offset {(Offset.HasValue ? $"{Offset.Value}" : "<null>")}";
    }

    public class Calculator(uint initialValueA, uint initialValueB, string[] instructionLines)
    {
        public Register A { get; } = new("A", initialValueA);
        public Register B { get; } = new("B", initialValueB);
        public string[] InstructionLines { get; } = instructionLines;

        private Instruction Parse(string line)
        {
            string[] parts = line.Replace(",", string.Empty).Split(' ');
            Register? register = parts[1] == "a" ? A : (parts[1] == "b" ? B : null);
            int? offset = int.TryParse(parts[1], out int temp) ? temp : (parts.Length > 2 ? (int.TryParse(parts[2], out temp) ? temp : null) : null);
            return parts[0] switch
            {
                "hlf" => new Instruction(Instruction.Types.Half, register),
                "tpl" => new Instruction(Instruction.Types.Triple, register),
                "inc" => new Instruction(Instruction.Types.Increment, register),
                "jmp" => new Instruction(Instruction.Types.Jump, null, offset),
                "jie" => new Instruction(Instruction.Types.JumpIfEven, register, offset),
                "jio" => new Instruction(Instruction.Types.JumpIfOne, register, offset),
                _ => throw new Exception($"Not a valid instruction: '{line}'!")
            };
        }

        private static void Jump(ref int index, Instruction instruction, Func<uint, bool>? condition = null)
        {
            if (condition is null || condition(instruction.Register!.Value))
            {
                index += instruction.Offset!.Value;
                if (C.Print) Console.WriteLine($"jumped, index is now {index}");
            }
            else
            {
                index++;
                if (C.Print) Console.WriteLine($"no jump, continuing to next, index is now {index}");
            }
        }

        public void RunProgram()
        {
            int executions = 0;

            Console.WriteLine($" > Running program consisting of {InstructionLines.Length} instructions (A is {A.Value} and B is {B.Value}).");
            for (int iIndex = 0; true;)
            {
                if (iIndex < 0 || iIndex >= InstructionLines.Length)
                {
                    if (C.Print) Console.WriteLine($" > No instruction defined at index {iIndex}! Exiting program...");
                    break;
                }

                string instructionLine = InstructionLines[iIndex];
                Instruction instruction = Parse(instructionLine);
                if (C.Print) Console.Write($" - applying instruction ({instructionLine,-10}) ... ");
                Register register = instruction.Register!;
                switch (instruction.Type)
                {
                    case Instruction.Types.Half:
                        register.ApplyAndMoveForward(value => value / 2, ref iIndex);
                        break;
                    case Instruction.Types.Triple:
                        register.ApplyAndMoveForward(value => value * 3, ref iIndex);
                        break;
                    case Instruction.Types.Increment:
                        register.ApplyAndMoveForward(value => value + 1, ref iIndex);
                        break;
                    case Instruction.Types.Jump:
                        Jump(ref iIndex, instruction);
                        break;
                    case Instruction.Types.JumpIfEven:
                        Jump(ref iIndex, instruction, value => value % 2 == 0);
                        break;
                    case Instruction.Types.JumpIfOne:
                        Jump(ref iIndex, instruction, value => value == 1);
                        break;
                }
                executions++;
            }

            Console.WriteLine($"\n > After program execution ({executions} executions), A is {A.Value} and B is {B.Value}.");
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 23;
        public override int GetCurrentPart => 2;

        public override void Run()
        {
            uint initialValueA = GetCurrentPart == 2 ? 1u : 0u;
            new Calculator(initialValueA, 0u, File.ReadAllLines(GetInputFilePath(Math.Min(1, GetCurrentPart)))).RunProgram();
        }
    }
}