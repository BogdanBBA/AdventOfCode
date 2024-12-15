using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using AoC2024.ForDay14;
using AoC2024.MatrixNavigation;
using System.Drawing;

namespace AoC2024
{
    namespace ForDay14
    {
        public class Robot(Coord initialPosition, Coord movement)
        {
            public Coord Position { get; private set; } = initialPosition;
            public Coord Movement { get; } = movement;

            public void MoveOnce(Size roomSize)
            {
                (int x, int y) = (Position.X + Movement.X, Position.Y + Movement.Y);
                Position = new(
                    x >= roomSize.Width ? x % roomSize.Width : (x < 0 ? x + roomSize.Width : x),
                    y >= roomSize.Height ? y % roomSize.Height : (y < 0 ? y + roomSize.Height : y)
                );
            }

            public void MoveMultipleTimes(Size roomSize, int moveCount)
            {
                for (int counter = 1; counter <= moveCount; counter++)
                    MoveOnce(roomSize);
            }

            public bool DetermineQuadrant(Coord middle, out int quadrant)
            {
                if (Position.X == middle.X || Position.Y == middle.Y) // middle row or col
                {
                    quadrant = -1;
                    return false;
                }

                if (Position.X < middle.X) // left: 1 or 3
                {
                    quadrant = Position.Y < middle.Y ? 1 : 3;
                    return true;
                }
                else // 2 or 4
                {
                    quadrant = Position.Y < middle.Y ? 2 : 4;
                    return true;
                }
            }
        }

        public class Room(Size roomSize, Robot[] robots)
        {
            public Size RoomSize { get; } = roomSize;
            public Robot[] Robots { get; } = robots;

            private (int, int, int, int) DetermineRobotCountPerQuadrant()
            {
                Coord middle = new(RoomSize.Width / 2, RoomSize.Height / 2);
                Dictionary<int, int> robotCountByQuadrants = new() { [0] = 0, [1] = 0, [2] = 0, [3] = 0 };
                foreach (Robot robot in Robots)
                    if (robot.DetermineQuadrant(middle, out int quadrant))
                        robotCountByQuadrants[quadrant - 1]++;
                return (robotCountByQuadrants[0], robotCountByQuadrants[1], robotCountByQuadrants[2], robotCountByQuadrants[3]);
            }

            private int DetermineSafetyFactor()
            {
                (int a, int b, int c, int d) = DetermineRobotCountPerQuadrant();
                $" - There are {a}, {b}, {c} and {d} robots in the four quadrants.".Log();
                return a * b * c * d;
            }

            public void SimulateAndPrintSafetyFactor(int seconds)
            {
                foreach (Robot robot in Robots)
                    robot.MoveMultipleTimes(RoomSize, seconds);
                $" > The safety factor is {DetermineSafetyFactor()}.".Log();
            }

            private void PrintToFile(string file, Func<List<string>, bool>? writeCondition = null)
            {
                List<string> lines = new(RoomSize.Height);
                for (int y = 0; y < RoomSize.Height; y++)
                {
                    StringBuilder line = new(RoomSize.Width);
                    for (int x = 0; x < RoomSize.Width; x++)
                    {
                        int robots = Robots.Count(r => r.Position.X == x && r.Position.Y == y);
                        line.Append(robots == 0 ? ' ' : (robots == 1 ? 'x' : 'X'));
                    }
                    lines.Add(line.ToString());
                }
                if (writeCondition is null || writeCondition(lines))
                    File.WriteAllText(file, string.Join(Environment.NewLine, lines));
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
            public void LookForChristmasTreeEasterEgg(string outputFolder, int count, bool deleteExisting = true)
            {
                if (deleteExisting && Directory.Exists(outputFolder))
                    Directory.Delete(outputFolder, true);
                Directory.CreateDirectory(outputFolder);

                for (int counter = 1; counter <= count; counter++)
                {
                    foreach (Robot robot in Robots)
                        robot.MoveOnce(RoomSize);

                    if (counter == 1 || counter == 2 || counter == 23 || counter == 6285) // (counter % RoomSize.Height == 2 || counter % RoomSize.Width == 23)
                    {
                        string file = Path.Combine(outputFolder, $"{counter}.bmp");
                        using Bitmap bmp = new(RoomSize.Width, RoomSize.Height);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.Black);
                            Robots.Select(r => r.Position).Distinct().ToList().ForEach(p => bmp.SetPixel(p.X, p.Y, Color.Yellow));
                        }
                        bmp.Save(file);
                    }
                }
            }
        }
    }

    public class Day14 : DayProgram
    {
        private static Room ParseRoom()
            => new(new(101, 103), [..ParseFromFile(@"14", line =>
                {
                    Match match = Regex.Match(line, @"p=(\d+),(\d+) v=(\-?\d+),(\-?\d+)");
                    return new Robot(new(match.ParseGroupAsInt(1), match.ParseGroupAsInt(2)), new(match.ParseGroupAsInt(3), match.ParseGroupAsInt(4)));
                })]);

        public override void Run()
        {
            ParseRoom().SimulateAndPrintSafetyFactor(100);
            ParseRoom().LookForChristmasTreeEasterEgg(@"outputs\14", 6285);
        }
    }
}