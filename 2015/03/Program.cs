namespace AoC2015.Day03
{
    using Coord = (int Row, int Col);

    public static class Utils
    {
        public static Coord GoingRight(this Coord coord) => (coord.Row, coord.Col + 1);
        public static Coord GoingLeft(this Coord coord) => (coord.Row, coord.Col - 1);
        public static Coord GoingDown(this Coord coord) => (coord.Row + 1, coord.Col);
        public static Coord GoingUp(this Coord coord) => (coord.Row - 1, coord.Col);

        public static Coord GetNextCoord(this Coord coord, char instruction)
            => instruction switch
            {
                '>' => coord.GoingRight(),
                '<' => coord.GoingLeft(),
                'v' => coord.GoingDown(),
                '^' => coord.GoingUp(),
                _ => throw new Exception(),
            };

        public static int CountDistinctHouses_OneSanta(string instructions)
        {
            Coord currentCoord = (0, 0);
            HashSet<Coord> set = [currentCoord];
            foreach (char instruction in instructions)
            {
                currentCoord = currentCoord.GetNextCoord(instruction);
                set.Add(currentCoord);
            }
            return set.Count;
        }

        public static int CountDistinctHouses_MultipleSantas(string instructions, int santasCount)
        {
            if (instructions.Length % santasCount != 0) throw new Exception();
            Coord[] santaCoords = Enumerable.Range(0, santasCount).Select(_ => (0, 0)).ToArray();
            HashSet<Coord> set = [.. santaCoords];
            for (int iInstruction = 0; iInstruction < instructions.Length; iInstruction += santasCount)
            {
                for (int iSanta = 0; iSanta < santasCount; iSanta++)
                {
                    santaCoords[iSanta] = santaCoords[iSanta].GetNextCoord(instructions[iInstruction + iSanta]);
                    set.Add(santaCoords[iSanta]);
                }
            }
            return set.Count;
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 3;
        public override int GetCurrentPart => 10;

        public override void Run()
        {
            string input = File.ReadAllText(GetInputFilePath(GetCurrentPart));
            Console.WriteLine($" > Result (input file {GetCurrentPart}): {Utils.CountDistinctHouses_OneSanta(input)} distinct houses");
            Console.WriteLine($" > Result (input file {GetCurrentPart}): {Utils.CountDistinctHouses_MultipleSantas(input, 2)} distinct houses");
        }
    }
}