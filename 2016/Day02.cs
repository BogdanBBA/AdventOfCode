using AoC2016.MatrixNavigation;

namespace AoC2016
{
    public class Day02 : IDayProgram
    {
        public record InstructionLine(DirectionsUDLR[] Instructions);

        public override void Run()
        {
            string[] KEYPAD = ["123", "456", "789"];
            char GetKey(Coord coord) => KEYPAD[coord.Y][coord.X];
            string FmtKey(Coord coord) => $"'{GetKey(coord)}' ({coord.X},{coord.Y})";

            InstructionLine[] instructionLines = ParseFromFile(@"02", line => new InstructionLine(line.Select(character => character.ParseAsDirectionUDLR()).ToArray()));
            Coord keypadPos = new(1, 1);
            string code = string.Empty;

            foreach (InstructionLine instructionLine in instructionLines)
            {
                Coord oldPos = keypadPos;
                foreach (DirectionsUDLR direction in instructionLine.Instructions)
                {
                    keypadPos = keypadPos.Move(direction, 1, new Bounds(0, 0, 2, 2));
                }
                char key = GetKey(keypadPos);
                code += key;
                $" - for {string.Join(string.Empty, instructionLine.Instructions.Select(i => (char)i))} ...".Log();
                $"   moving from {FmtKey(oldPos)} to {FmtKey(keypadPos)}: key is {key}".Log();
            }
            $" > The code to the bathroom is {code}.".Log();

            NavigableMap FANCY_KEYPAD = NavigableMap.Parse(["  1  ", " 234 ", "56789", " ABC ", "  D  "], (coord, chr) => chr != ' ');
            NavigableMap.Cell currentKey = FANCY_KEYPAD.GetByCharacter('5');
            code = string.Empty;
            foreach (InstructionLine instructionLine in instructionLines)
            {
                foreach (DirectionsUDLR direction in instructionLine.Instructions)
                {
                    currentKey = FANCY_KEYPAD.MoveIfPossible(currentKey, direction);
                }
                code += currentKey.Character;
            }
            $" > Well, actually... the code on the actual fancy keypad, according to the same instructions, is {code}.".Log();
        }
    }
}