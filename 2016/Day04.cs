#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
#pragma warning disable CA1854 // Prefer the 'IDictionary.TryGetValue(TKey, out TValue)' method
#pragma warning disable CA1864 // Prefer the 'IDictionary.TryAdd(TKey, TValue)' method
using System.Text.RegularExpressions;

namespace AoC2016
{
    public class Room
    {
        private const string ROOM_ENCODING_PATTERN = @"([\w\-]+)-(\d+)\[(\w+)\]";

        private static readonly Dictionary<(char, int), char> CypherMap = [];

        public string Encoded { get; private set; }
        public string Name { get; private set; }
        public int SectorID { get; private set; }
        public string Checksum { get; private set; }

        public Room(string encoded)
        {
            Encoded = encoded;
            Match match = Regex.Match(Encoded, ROOM_ENCODING_PATTERN);
            Name = match.Groups[1].Value;
            SectorID = int.Parse(match.Groups[2].Value);
            Checksum = match.Groups[3].Value;
        }

        public bool IsRealRoom(bool print)
        {
            string calculatedChecksum = CalculateChecksum(Name);
            bool result = calculatedChecksum == Checksum;
            if (print) $" - {result.ToString().ToUpper(),5} room: {Encoded} (\"{Name}\", {SectorID}, {Checksum}), vs. {calculatedChecksum}".Log();
            return result;
        }

        private static string CalculateChecksum(string name)
        {
            Dictionary<char, int> freq = [];
            foreach (char ch in name)
            {
                if (ch == '-')
                    continue;
                if (freq.ContainsKey(ch))
                    freq[ch]++;
                else
                    freq.Add(ch, 1);
            }

            KeyValuePair<char, int>[] sortedEntries = [.. freq.OrderByDescending(pair => pair.Value).ThenBy(pair => pair.Key)];
            return new string(sortedEntries.Take(5).Select(pair => pair.Key).ToArray());
        }

        private static char ShiftOnce(char ch)
            => ch switch
            {
                >= 'a' and <= 'y' => (char)(ch + 1),
                'z' => 'a',
                _ => ' '
            };

        private static char Shift(char ch, int shifts)
        {
            for (int count = 1; count <= shifts; count++)
                ch = ShiftOnce(ch);
            return ch;
        }

        private static char Decrypt(char ch, int shifts)
        {
            return CypherMap.TryGetValue((ch, shifts), out char output)
                ? output
                : Shift(ch, shifts);

        }

        public static string Decrypt(Room room)
            => new(room.Encoded.Select(ch => Decrypt(ch, room.SectorID)).ToArray());
    }

    public class Day04 : IDayProgram
    {
        public override void Run()
        {
            Room[] rooms = ParseFromFile(@"04", line => new Room(line));
            Room[] realRooms = rooms.Where(room => room.IsRealRoom(true)).ToArray();
            int sectorIdSum = realRooms.Sum(room => room.SectorID);
            $"\n > From the list of {rooms.Length} rooms, only {realRooms.Length} are real and not decoys, and the sum of their sector IDs is {sectorIdSum}.\n".Log();

            $" > The decrypted room names are:".Log();
            int longestName = realRooms.Max(room => room.Name.Length);
            List<string> decryptedRoomNames = realRooms.Select(Room.Decrypt).ToList();
            for (int index = 0; index < realRooms.Length; index++)
            {
                Room room = realRooms[index];
                $" - {index + 1,3}. {room.Name.PadRight(longestName)} -> {decryptedRoomNames[index]}".Log();
            }
            Room targetRoom = realRooms[decryptedRoomNames.IndexOf(decryptedRoomNames.First(name => name.Contains("north")))];
            $"\n > The room you're interested in (where North Pole objects are stored) is:\n - {$"{targetRoom.Name}, {targetRoom.SectorID}".PadRight(longestName + 5)} -> {Room.Decrypt(targetRoom)}\n".Log();
        }
    }
}