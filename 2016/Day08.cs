using System.Drawing;
using System.Text.RegularExpressions;
using AoC2016.MatrixNavigation;

namespace AoC2016
{
    public enum Instruction08Type { Rect, RotateRow, RotateColumn }

    public record Instruction08(Instruction08Type Type, int A, int B);

    public class Day08 : IDayProgram
    {
        private static Instruction08 ParseInstruction(string text)
        {
            Match match = Regex.Match(text, @"rect (\d+)x(\d+)");
            if (match.Success)
                return new(Instruction08Type.Rect, int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            match = Regex.Match(text, @"rotate column x=(\d+) by (\d+)");
            if (match.Success)
                return new(Instruction08Type.RotateColumn, int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            match = Regex.Match(text, @"rotate row y=(\d+) by (\d+)");
            if (match.Success)
                return new(Instruction08Type.RotateRow, int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
            throw new Exception($"Unknown operation ({text})!");
        }

        public static void RunInstruction(MonochromeDisplay display, Instruction08 instruction, bool printAfter)
        {
            switch (instruction.Type)
            {
                case Instruction08Type.Rect:
                    display.Fill(new(0, 0, instruction.A, instruction.B), MonochromeDisplay.PixelState.On);
                    break;
                case Instruction08Type.RotateRow:
                    display.ShiftRow(instruction.A, instruction.B, true);
                    break;
                case Instruction08Type.RotateColumn:
                    display.ShiftColumn(instruction.A, instruction.B, true);
                    break;
                default:
                    $"WARNING: instruction {instruction.Type}({instruction.A}, {instruction.B}) not handled!".Log();
                    break;
            }
            if (printAfter)
                display.Print($"after instruction: {instruction.Type} of {instruction.A}, {instruction.B}:");
        }

        public static void RunInstructionsAndPrint(Size displaySize, Instruction08[] instructions)
        {
            MonochromeDisplay display = new(displaySize);
            display.Print("INITIAL STATE:");
            instructions.ToList().ForEach(instruction => RunInstruction(display, instruction, true));
            int litCount = display.CountPixelsWithState(MonochromeDisplay.PixelState.On);
            $" > Out of the {displaySize.Width * displaySize.Height} pixels (size {displaySize.Width} x {displaySize.Height}), a number of {litCount} are lit at the end.\n".Log();
        }

        public override void Run()
        {
            // test screen size is 7x3, normal is 50x6
            RunInstructionsAndPrint(new(50, 6), ParseFromFile(@"08", ParseInstruction));
        }
    }
}