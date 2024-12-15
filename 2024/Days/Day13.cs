using System.Text.RegularExpressions;
using AoC2024.MatrixNavigation;

namespace AoC2024
{
    public class Day13 : DayProgram
    {
        private static void DoPart1(string file)
        {
            string fileContents = ReadFromFile_String(file);
            List<Coord> a = Regex.Matches(fileContents, @"Button A: X\+?(\d+), Y\+(\d+)").Select(m => new Coord(m.ParseGroupAsInt(1), m.ParseGroupAsInt(2))).ToList();
            List<Coord> b = Regex.Matches(fileContents, @"Button B: X\+?(\d+), Y\+(\d+)").Select(m => new Coord(m.ParseGroupAsInt(1), m.ParseGroupAsInt(2))).ToList();
            List<Coord> p = Regex.Matches(fileContents, @"Prize: X=(\d+), Y=(\d+)").Select(m => new Coord(m.ParseGroupAsInt(1), m.ParseGroupAsInt(2))).ToList();
            Console.WriteLine($"test");
        }

        public override void Run()
        {
            DoPart1(@"tests\13");
        }
    }
}