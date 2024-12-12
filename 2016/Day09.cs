using System.Text;
using System.Text.RegularExpressions;

namespace AoC2016
{
    public class Day09 : IDayProgram
    {
        private static (int CharacterCount, int RepeatCount) ParseMarker(string data)
        {
            Match match = Regex.Match(data, @"(\d+)x(\d+)");
            return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        }

        private static string Format(string data)
            => data.Length <= 50 ? data : $"{data[..24]}...{data[^24..]}";

        private static void Decompress(string data)
        {
            StringBuilder sb = new();
            for (int index = 0; index < data.Length; index++)
            {
                char ch = data[index];
                if (ch >= 'A' && ch <= 'Z')
                {
                    sb.Append(ch);
                }
                else if (ch == '(')
                {
                    int postMarkerIndex = data.IndexOf(')', index) + 1;
                    string marker = data[index..postMarkerIndex];
                    (int characterCount, int repeatCount) = ParseMarker(marker);
                    string section = data[postMarkerIndex..(postMarkerIndex + characterCount)];
                    for (int time = 1; time <= repeatCount; time++)
                        sb.Append(section);
                    index = postMarkerIndex + characterCount - 1;
                }
            }
            string result = sb.ToString();
            $" - {Format(data)} -> {Format(result)} ({result.Length})".Log();
        }

        private static ulong DecompressV2(string data, int level = 0)
        {
            ulong size = 0;
            for (int index = 0; index < data.Length; index++)
            {
                char ch = data[index];
                if (ch >= 'A' && ch <= 'Z')
                {
                    size++;
                }
                else if (ch == '(')
                {
                    int postMarkerIndex = data.IndexOf(')', index) + 1;
                    string marker = data[index..postMarkerIndex];
                    (int characterCount, int repeatCount) = ParseMarker(marker);
                    string section = data[postMarkerIndex..(postMarkerIndex + characterCount)];
                    ulong sectionLength = DecompressV2(section, level + 1);
                    size += sectionLength * (ulong)repeatCount;
                    index = postMarkerIndex + characterCount - 1;
                }
            }
            if (level == 0)
                $" - {Format(data)} -> [...] ({size})".Log();
            return size;
        }

        private static void Decompress(string[] lines, bool useVersion2)
        {
            foreach (string line in lines)
                if (!useVersion2)
                    Decompress(line);
                else
                    DecompressV2(line);
            string.Empty.Log();
        }

        public override void Run()
        {
            string[] lines = ParseFromFile(@"09", line => line);
            Decompress(lines, false);
            Decompress(lines, true);
        }
    }
}