namespace AoC2015.Day06
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using Coord = (int Row, int Col);

    public static class Utils
    {
        public static Instruction.Types ParseAsInstructionType(this string text)
            => text switch
            {
                "turn on" => Instruction.Types.TurnOn,
                "turn off" => Instruction.Types.TurnOff,
                "toggle" => Instruction.Types.Toggle,
                _ => throw new ArgumentException(text)
            };
    }

    public class Instruction
    {
        public enum Types { TurnOn, TurnOff, Toggle }

        public static Instruction Parse(string line)
        {
            const string RegExPattern = @"(turn on|toggle|turn off) (\d+),(\d+) through (\d+),(\d+)";
            Match match = Regex.Match(line, RegExPattern);
            return new()
            {
                From = (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)),
                To = (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value)),
                Type = match.Groups[1].Value.ParseAsInstructionType()
            };
        }

        public Coord From { get; private set; }
        public Coord To { get; private set; }
        public Types Type { get; private set; }

        public override string ToString()
            => $"{From.Row,3},{From.Col,3} to {To.Row,3},{To.Col,3}: {Type}";
    }

    public class Light
    {
        public bool Old_IsOn { get; private set; } = false;
        public int BrightnessValue { get; private set; } = 0;

        public void Set(Instruction.Types instruction)
        {
            switch (instruction)
            {
                case Instruction.Types.TurnOn:
                    Old_IsOn = true;
                    BrightnessValue++;
                    break;
                case Instruction.Types.Toggle:
                    Old_IsOn = !Old_IsOn;
                    BrightnessValue += 2;
                    break;
                case Instruction.Types.TurnOff:
                    Old_IsOn = false;
                    if (BrightnessValue > 0) BrightnessValue--;
                    break;
            }
        }
    }

    public class Decoration
    {
        public Light[][] Lights { get; private set; }

        public Decoration(int width, int height, List<Instruction> instructions)
        {
            Lights = new Light[width][];
            for (int iRow = 0; iRow < width; iRow++)
                Lights[iRow] = Enumerable.Range(0, height).Select(_ => new Light()).ToArray();
            FollowInstructions(instructions);
        }

        private void FollowInstruction(Instruction instruction)
        {
            if (instruction.From.Row > instruction.To.Row || instruction.From.Col > instruction.To.Col)
                throw new ArgumentException(instruction.ToString());
            for (int iRow = instruction.From.Row; iRow <= instruction.To.Row; iRow++)
            {
                for (int iCol = instruction.From.Col; iCol <= instruction.To.Col; iCol++)
                {
                    Lights[iRow][iCol].Set(instruction.Type);
                }
            }
        }

        private void FollowInstructions(List<Instruction> instructions)
            => instructions.ForEach(FollowInstruction);

        public int LitLightsCount
            => Lights.SelectMany(row => row).Count(light => light.Old_IsOn);

        public List<int> GetBrightnesses()
            => Lights.SelectMany(row => row).Select(light => light.BrightnessValue).ToList();

        public void Print(string filePath, char lightOff, char lightOn)
        {
            string[] lines = Lights.Select(row => new string(row.Select(light => light.Old_IsOn ? lightOn : lightOff).ToArray())).ToArray();
            File.WriteAllLines(filePath, lines);
            Console.WriteLine($" // decoration data written to file '{filePath}'");
        }

        public void GenerateBitmap(string filePath, int alreadyDeterminedMax)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new Exception();
#pragma warning disable CA1416 // Validate platform compatibility
            using (Bitmap bmp = new(Lights.Length, Lights[0].Length))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    for (int iRow = 0; iRow < Lights.Length; iRow++)
                    {
                        for (int iCol = 0; iCol < Lights[iRow].Length; iCol++)
                        {
                            int scaledGray = (int)(255.0 * Lights[iRow][iCol].BrightnessValue / alreadyDeterminedMax);
                            Color color = Color.FromArgb(scaledGray, scaledGray, scaledGray);
                            bmp.SetPixel(iRow, iCol, color);
                        }
                    }
                }
                bmp.Save(filePath);
            }
#pragma warning restore CA1416 // Validate platform compatibility
            Console.WriteLine($" // decoration data drawn to file '{filePath}'");
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 6;

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            List<Instruction> instructions = lines.Select(Instruction.Parse).ToList();
            Decoration decoration = new(1000, 1000, instructions);

            List<int> brightnesses = decoration.GetBrightnesses();
            int maxBrightness = brightnesses.Max();
            Console.WriteLine($" // brightness levels: min {brightnesses.Min():N0}, avg {brightnesses.Average():N3}, max {maxBrightness:N0}");
            decoration.Print(GetOutputFilePath(GetCurrentPart), ' ', '*');
            decoration.GenerateBitmap(GetOutputFilePath(GetCurrentPart).Replace(".txt", ".bmp"), maxBrightness);

            Console.WriteLine($" > Result (part 1): {decoration.LitLightsCount:N0} lights are 'on'");
            Console.WriteLine($" > Result (part 2): {brightnesses.Sum():N0} total brightness");
        }
    }
}