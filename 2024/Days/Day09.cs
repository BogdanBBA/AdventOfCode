using AoC2024.ForDay09;
using Disk = int[];

namespace AoC2024
{
    namespace ForDay09
    {
        static class ExtensionMethods09
        {
            public static string FormatData(this Disk disk)
                => string.Join(string.Empty, disk.Select(v => v == -1 ? "." : v.ToString()));

            public static string FormatHeader(this Disk disk, string header)
                => $"\n > {header} (length {disk.Length}):\n{disk.FormatData()}";

            public static bool IsFile(this int value)
                => value != -1;

            public static bool IsSpace(this int value)
                => !value.IsFile();

            public static Data GetAllSpans(this Disk disk)
            {
                List<BlockSpan> files = [];
                List<BlockSpan> spaces = [];
                bool wasSpace = disk[0].IsSpace();
                int start = 0, end = 0, lastId = disk[0];

                for (int index = 1; index < disk.Length; index++)
                {
                    int value = disk[index];
                    if (value.IsSpace())
                    {
                        if (wasSpace)
                        {
                            end = index;
                        }
                        else
                        {
                            files.Add(new(disk[index - 1], start, end));
                            start = end = index;
                            wasSpace = true;
                        }
                    }
                    else
                    {
                        if (wasSpace)
                        {
                            spaces.Add(new(-1, start, end));
                            start = end = index;
                            wasSpace = false;
                        }
                        else
                        {
                            if (disk[index] != disk[index - 1])
                            {
                                files.Add(new(disk[index - 1], start, end));
                                start = end = index;
                            }
                            else
                            {
                                end = index;
                            }
                        }
                    }
                }

                if (wasSpace)
                    spaces.Add(new(-1, start, end));
                else
                    files.Add(new(disk.Last(), start, end));

                return new(files.ToDictionary(f => f.ID), spaces);
            }
        }

        public class BlockSpan(int id, int startIndex, int endIndex)
        {
            public int ID { get; } = id;
            public int StartIndex { get; set; } = startIndex;
            public int EndIndex { get; set; } = endIndex;
            public int Length => EndIndex - StartIndex + 1;
        }

        public class Data(Dictionary<int, BlockSpan> fileSpanDict, List<BlockSpan> spaceSpans)
        {
            public static Disk ExpandDiskRepresentation(string text)
            {
                int diskSize = text.Select(c => c - '0').Sum();
                Disk disk = [.. Enumerable.Range(0, diskSize).Select(_ => -1)];
                for (int compressedIndex = 0, diskIndex = 0, fileId = 0; compressedIndex < text.Length; compressedIndex++)
                {
                    int currentCompressedValue = text[compressedIndex] - '0';
                    if (compressedIndex % 2 == 0)
                    {
                        for (int miniDiskIndex = diskIndex; miniDiskIndex < diskIndex + currentCompressedValue; miniDiskIndex++)
                            disk[miniDiskIndex] = fileId;
                        fileId++;
                    }
                    diskIndex += currentCompressedValue;
                }
                return disk;
            }

            public static Disk ExpandDiskRepresentation(Data data)
            {
                int totalFileLength = data.FileSpanDict.Keys.Sum(k => data.FileSpanDict[k].Length);
                int totalSpaceLength = data.SpaceSpans.Sum(s => s.Length);
                Disk disk = [.. Enumerable.Range(0, totalFileLength + totalSpaceLength).Select(_ => -1)];
                foreach (BlockSpan fileSpan in data.FileSpanDict.Values)
                {
                    for (int index = fileSpan.StartIndex; index <= fileSpan.EndIndex; index++)
                        disk[index] = fileSpan.ID;
                }
                return disk;
            }

            public Dictionary<int, BlockSpan> FileSpanDict { get; } = fileSpanDict;
            public List<BlockSpan> SpaceSpans { get; private set; } = spaceSpans;

            public void GroupContiguousSpaces()
            {
                SpaceSpans = [.. SpaceSpans.OrderBy(s => s.StartIndex)];
                for (int i = 0; i < SpaceSpans.Count - 1; i++)
                {
                    if (SpaceSpans[i].EndIndex == SpaceSpans[i + 1].StartIndex - 1)
                    {
                        SpaceSpans[i].EndIndex += SpaceSpans[i + 1].Length;
                        SpaceSpans.RemoveAt(i + 1);
                        i--;
                    }
                }
            }

            public Disk ToDisk()
              => ExpandDiskRepresentation(this);

            public long CalculateChecksum()
            {
                Disk disk = ToDisk();
                return Enumerable.Range(0, disk.Length).Select(index => (long)(disk[index] == -1 ? 0 : disk[index] * index)).Sum();
            }
        }
    }

    public class Day09 : DayProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
        private static void MoveFiles_Part1(Disk disk, bool detailedLogging)
        {
            while (true)
            {
                int lastFileIndex = disk.LastIndexOf(v => v.IsFile());
                int firstSpaceIndex = disk.FirstIndexOf(v => v.IsSpace());
                if (firstSpaceIndex > lastFileIndex) break;

                disk[firstSpaceIndex] = disk[lastFileIndex];
                disk[lastFileIndex] = -1;

                if (detailedLogging) $" - {disk.FormatData()}".Log();
            }
        }

        private static void MoveFiles_Part2(Data data, string? detailedLoggingFile)
        {
            static int GetFirstFittingSpaceBlockIndex(List<BlockSpan> spaceSpans, BlockSpan fileSpan)
            {
                for (int index = 0; index < spaceSpans.Count; index++)
                {
                    BlockSpan space = spaceSpans[index];
                    if (space.EndIndex > fileSpan.StartIndex)
                        break;
                    if (space.Length >= fileSpan.Length)
                        return index;
                }
                return -1;
            }

            static void MoveWholeFile(int fileID, int spaceSpanIndex, Data data)
            {
                // init
                BlockSpan fileSpan = data.FileSpanDict[fileID];
                int fileStartIndex = fileSpan.StartIndex, length = fileSpan.Length;
                int spaceStartIndex = data.SpaceSpans[spaceSpanIndex].StartIndex;

                // retract space
                if (data.SpaceSpans[spaceSpanIndex].Length > length)
                    data.SpaceSpans[spaceSpanIndex].StartIndex += length;
                else
                    data.SpaceSpans.RemoveAt(spaceSpanIndex);

                // change file
                data.FileSpanDict[fileID] = new(fileID, spaceStartIndex, spaceStartIndex + length - 1);

                // expand space in place of file
                data.SpaceSpans.Add(new(-1, fileStartIndex, fileStartIndex + length - 1));
                data.GroupContiguousSpaces();
            }

            $" > Files ({data.FileSpanDict.Keys.Count}): {string.Join(", ", data.FileSpanDict.Keys.Select(k => $"{k}:{data.FileSpanDict[k].StartIndex}-{data.FileSpanDict[k].EndIndex}"))}".Log();
            $" > Spaces ({data.SpaceSpans.Count}): {string.Join(", ", data.SpaceSpans.Select(s => $"{s.StartIndex}-{s.EndIndex}"))}".Log();
            foreach (int fileID in data.FileSpanDict.Keys.OrderByDescending(id => id))
            {
                int spaceSpanIndex = GetFirstFittingSpaceBlockIndex(data.SpaceSpans, data.FileSpanDict[fileID]);
                if (spaceSpanIndex != -1)
                {
                    MoveWholeFile(fileID, spaceSpanIndex, data);
                    if (detailedLoggingFile is not null) $" - {data.ToDisk().FormatData()}".Log();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value")]
        public override void Run()
        {
            (string Test1, string Test2, string FullData) = ("12345", "2333133121414131402", ReadFromFile_String("09"));
            string dataToTest = FullData;

            Data data = Data.ExpandDiskRepresentation(dataToTest).GetAllSpans();
            data.ToDisk().FormatHeader("Unformatted disk").Log();
            MoveFiles_Part2(data, null);
            data.ToDisk().FormatHeader("Disk after formatting").Log();
            $"\n > The checksum for the formatted disk is: {data.CalculateChecksum()}.\n".Log();
        }
    }
}