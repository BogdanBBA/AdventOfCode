namespace AoC2016
{
    using AoC2016.MatrixNavigation;

    public record Instruction01(DirectionsLR DirectionLR, int Steps);

    public class Day01 : IDayProgram
    {
        public override void Run()
        {
            Coord here = new(0, 0);
            Cardinal cardinal = Cardinal.North;

            List<Coord> visitedLocations = [here];

            ParseFromFileCsv(@"01", i => new Instruction01(i[0].ParseAsDirectionLR(), i[1..].ParseAsInt())).ToList().ForEach(instruction =>
            {
                cardinal = cardinal.RotateOnce(instruction.DirectionLR);
                Console.Write($" - moving from ({here.X,3}, {here.Y,3}) {cardinal.ToString().ToLowerInvariant(),9}ward for {instruction.Steps.Plural("step"),-7}...");
                here = here.Move(cardinal.ToDirectionsUDLR(), instruction.Steps, newLocations => visitedLocations.AddRange(newLocations));
                $" to ({here.X,3}, {here.Y,3})".Log();
            });
            int distance = Math.Abs(here.X) + Math.Abs(here.Y);
            $" > Easter Bunny HQ is at ({here.X,3}, {here.Y,3}), which is {distance} blocks away from the starting point.".Log();

            Coord? firstRev = visitedLocations.GetFirstDuplicate();
            if (firstRev is not null)
            {
                int correctDistance = Math.Abs(firstRev.X) + Math.Abs(firstRev.Y);
                $" > It turns out that the correct location of Easter Bunny HQ is at the first-revisited location ({firstRev.X,3}, {firstRev.Y,3}), which is {correctDistance} blocks away.".Log();
            }
        }
    }
}