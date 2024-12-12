internal partial class Program
{
    private static void Main(string[] _)
    {
        static string GearSelector((Board.Cell cell, int numberA, int numberB) gear) => $"  - at ({gear.cell.Coordinates.Row,3}, {gear.cell.Coordinates.Col,3}), adjacent to numbers {gear.numberA,3} and {gear.numberB,3}";

        Board board = new();

        // (List<(int Number, string SurroundingCellValues)> PartNumbers, List<(int Number, string SurroundingCellValues)> NotPartNumbers) = board.GetAllNumbers();
        // Console.WriteLine($" > The {NotPartNumbers.Count} NON-part numbers are:\n{string.Join("\n", NotPartNumbers.Select(n => $"  - {n.Number,3}, surrounded by '{n.SurroundingCellValues}'"))}");
        // Console.WriteLine($" > The {PartNumbers.Count} PART numbers are:\n{string.Join("\n", PartNumbers.Select(n => $"  - {n.Number,3}, surrounded by '{n.SurroundingCellValues}'"))}");
        // Console.WriteLine($" >>> The sum of all {PartNumbers.Count} PART numbers is: {PartNumbers.Select(pair => pair.Number).Sum()}");

        List<(Board.Cell cell, int numberA, int numberB)> gears = board.GetGears();
        Console.WriteLine($" >>> The following {gears.Count} gears:\n{string.Join("\n", gears.Select(GearSelector))}\n >>> generate the sum of gear ratios of: {gears.Select(gear => gear.numberA * gear.numberB).Sum()}");
    }
}