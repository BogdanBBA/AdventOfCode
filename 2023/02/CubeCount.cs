namespace AdventOfCode02
{
    internal class CubeCount : Dictionary<Cube, int>
    {
        public static CubeCount ParseFromDictionary(Dictionary<Cube, int> dict)
        {
            CubeCount result = [];
            foreach (Cube key in dict.Keys) result.Add(key, dict[key]);
            return result;
        }
    }
}