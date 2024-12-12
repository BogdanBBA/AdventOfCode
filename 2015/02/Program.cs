namespace AoC2015.Day02
{
    public class Box
    {
        public static Box Parse(string line)
        {
            string[] parts = line.Split('x');
            return new()
            {
                Length = int.Parse(parts[0]),
                Width = int.Parse(parts[1]),
                Height = int.Parse(parts[2])
            };
        }

        public int Length { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int GetRequiredPaperArea()
        {
            int area1 = Length * Width, area2 = Width * Height, area3 = Height * Length;
            int result = 2 * area1 + 2 * area2 + 2 * area3 + Math.Min(Math.Min(area1, area2), area3);
            Console.WriteLine($" - {Length} x {Width} x {Height} → {result} (paper)");
            return result;
        }

        public int GetRequiredRibbonLength()
        {
            int halfPerimeter1 = Length + Width, halfPerimeter2 = Width + Height, halfPerimeter3 = Height + Length;
            int result = 2 * Math.Min(Math.Min(halfPerimeter1, halfPerimeter2), halfPerimeter3) + Length * Width * Height;
            Console.WriteLine($" - {Length} x {Width} x {Height} → {result} (ribbon)");
            return result;
        }
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 2;

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart));
            Box[] boxes = lines.Select(Box.Parse).ToArray();
            Console.WriteLine($"\n > Result (part 1): {boxes.Select(box => box.GetRequiredPaperArea()).Sum()} square feet of paper\n");
            Console.WriteLine($"\n > Result (part 2): {boxes.Select(box => box.GetRequiredRibbonLength()).Sum()} feet of ribbon");
        }
    }
}