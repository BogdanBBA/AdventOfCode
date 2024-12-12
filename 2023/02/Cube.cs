namespace AdventOfCode02
{
    internal enum CubeEnum { Red, Green, Blue }

    internal class Cube
    {
        public static readonly Cube Red = new(CubeEnum.Red, "red");
        public static readonly Cube Green = new(CubeEnum.Green, "green");
        public static readonly Cube Blue = new(CubeEnum.Blue, "blue");
        public static readonly List<Cube> AllCubes = [Red, Green, Blue];

        public static Cube ParseCube(string color) => AllCubes.First(cube => cube.Color == color);

        public CubeEnum CubeEnum { get; private set; }
        public string Color { get; private set; }

        private Cube(CubeEnum cubeEnum, string color)
        {
            CubeEnum = cubeEnum;
            Color = color;
        }
    }
}