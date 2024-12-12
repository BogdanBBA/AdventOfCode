namespace AoC2024.MatrixNavigation
{
    /// <summary>Coordinates as with screen pixels (using X and Y, not Row and Col!), positive values going right and down.</summary>
    public record Coord(int X = 0, int Y = 0);
    public record Bounds(int Left, int Top, int Bottom, int Right);

    public enum DirectionsLR { Left = 'L', Right = 'R' }
    public enum DirectionsUDLR { Up = 'U', Down = 'D', Left = 'L', Right = 'R' }
    public enum Cardinal { North = 'N', East = 'E', South = 'S', West = 'W' }

    public static class EnumExtensionMethods
    {
        public static DirectionsLR ParseAsDirectionLR(this char value) => value switch
        {
            'L' => DirectionsLR.Left,
            'R' => DirectionsLR.Right,
            _ => throw new ArgumentException($"Invalid value '{value}'!")
        };

        public static DirectionsUDLR ParseAsDirectionUDLR(this char value) => value switch
        {
            'U' => DirectionsUDLR.Up,
            'D' => DirectionsUDLR.Down,
            'L' => DirectionsUDLR.Left,
            'R' => DirectionsUDLR.Right,
            _ => throw new ArgumentException($"Invalid value '{value}'!")
        };

        public static Cardinal ParseAsCardinal(this char value) => value switch
        {
            'N' => Cardinal.North,
            'E' => Cardinal.East,
            'S' => Cardinal.South,
            'W' => Cardinal.West,
            _ => throw new ArgumentException($"Invalid value '{value}'!")
        };

        public static DirectionsUDLR ToDirectionsUDLR(this Cardinal value) => value switch
        {
            Cardinal.North => DirectionsUDLR.Up,
            Cardinal.East => DirectionsUDLR.Right,
            Cardinal.West => DirectionsUDLR.Left,
            Cardinal.South => DirectionsUDLR.Down,
            _ => throw new ArgumentException($"Invalid value '{value}'!")
        };

        public static Cardinal RotateOnce(this Cardinal cardinal, DirectionsLR direction) => cardinal switch
        {
            Cardinal.North => direction == DirectionsLR.Left ? Cardinal.West : Cardinal.East,
            Cardinal.East => direction == DirectionsLR.Left ? Cardinal.North : Cardinal.South,
            Cardinal.South => direction == DirectionsLR.Left ? Cardinal.East : Cardinal.West,
            Cardinal.West => direction == DirectionsLR.Left ? Cardinal.South : Cardinal.North,
            _ => throw new ArgumentException($"Invalid value '{cardinal}'!")
        };

        public static DirectionsUDLR RotateOnce(this DirectionsUDLR direction, DirectionsLR rotateDirection) => direction switch
        {
            DirectionsUDLR.Up => rotateDirection == DirectionsLR.Left ? DirectionsUDLR.Left : DirectionsUDLR.Right,
            DirectionsUDLR.Right => rotateDirection == DirectionsLR.Left ? DirectionsUDLR.Up : DirectionsUDLR.Down,
            DirectionsUDLR.Down => rotateDirection == DirectionsLR.Left ? DirectionsUDLR.Right : DirectionsUDLR.Left,
            DirectionsUDLR.Left => rotateDirection == DirectionsLR.Left ? DirectionsUDLR.Down : DirectionsUDLR.Up,
            _ => throw new ArgumentException($"Invalid value '{direction}'!")
        };

        public static DirectionsUDLR TurnAround(this DirectionsUDLR direction) => direction switch
        {
            DirectionsUDLR.Up => DirectionsUDLR.Down,
            DirectionsUDLR.Right => DirectionsUDLR.Left,
            DirectionsUDLR.Down => DirectionsUDLR.Up,
            DirectionsUDLR.Left => DirectionsUDLR.Right,
            _ => throw new ArgumentException($"Invalid value '{direction}'!")
        };

        public static bool IsWithinBounds(this Coord coord, Bounds bounds)
            => coord.X >= bounds.Left && coord.X <= bounds.Right && coord.Y >= bounds.Top && coord.Y <= bounds.Bottom;

        public static Coord Move(this Coord coord, Cardinal cardinal)
            => coord.Move(cardinal.ToDirectionsUDLR());

        public static Coord Move(this Coord coord, DirectionsUDLR direction) => direction switch
        {
            DirectionsUDLR.Up => new(coord.X, coord.Y - 1),
            DirectionsUDLR.Right => new(coord.X + 1, coord.Y),
            DirectionsUDLR.Down => new(coord.X, coord.Y + 1),
            DirectionsUDLR.Left => new(coord.X - 1, coord.Y),
            _ => throw new ArgumentException($"Invalid value '{direction}'!")
        };

        public static Coord Move(this Coord coord, DirectionsUDLR direction, int steps, Action<Coord[]>? visitedLocationsCallback = null)
        {
            Coord[] visitedLocations = new Coord[steps];
            for (int step = 1; step <= steps; step++)
            {
                coord = coord.Move(direction);
                visitedLocations[step - 1] = coord;
            }
            visitedLocationsCallback?.Invoke(visitedLocations);
            return coord;
        }

        /// <summary>Assumes a valid starting position, returns last position described by the given movement that is within bounds.</summary>
        public static Coord Move(this Coord coord, DirectionsUDLR direction, int steps, Bounds bounds)
        {
            if (!coord.IsWithinBounds(bounds))
                throw new ArgumentException($"Coord {coord} was supposed to be within bounds {bounds}!");
            Coord valid = coord;
            for (int step = 1; step <= steps; step++)
            {
                coord = coord.Move(direction);
                if (coord.IsWithinBounds(bounds))
                    valid = coord;
            }
            return valid;
        }
    }
}