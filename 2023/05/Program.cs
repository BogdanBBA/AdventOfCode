using System.Globalization;
using System.Text.RegularExpressions;

internal class Program
{
    public class MapException(uint sourceStart, uint destStart, uint range)
    {
        public uint SourceStart { get; } = sourceStart;
        public uint SourceEnd { get; } = sourceStart + range - 1;
        public uint DestStart { get; } = destStart;
        public uint DestEnd { get; } = destStart + range - 1;
    }

    public class FarmMap(params MapException[] exceptions)
    {
        public MapException[] Exceptions { get; } = exceptions;

        public uint GetValueByID(uint id)
        {
            foreach (MapException exception in Exceptions)
            {
                if (exception.SourceStart <= id && id <= exception.SourceEnd)
                    return exception.DestStart + (id - exception.SourceStart);
            }
            return id;
        }
    }

    public class Almanac
    {
        public List<(uint start, uint range)> SeedIDs { get; }
        public FarmMap SeedToSoil { get; }
        public FarmMap SoilToFertilizer { get; }
        public FarmMap FertilizerToWater { get; }
        public FarmMap WaterToLight { get; }
        public FarmMap LightToTemperature { get; }
        public FarmMap TemperatureToHumidity { get; }
        public FarmMap HumidityToLocation { get; }

        public Almanac(string inputPath)
        {
            static List<(uint start, uint range)> ParseSeedIDs(string list)
            {
                List<uint> numbers = list.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(uint.Parse)
                    .ToList();
                return Enumerable.Range(0, numbers.Count / 2)
                    .Select(index => (numbers[index * 2], numbers[index * 2 + 1]))
                    .ToList();
            }

            static FarmMap ParseFarmMap(string content, string header)
            {
                string exceptionContent = Regex.Match(content, header + @":([\d \r\n]+)(\r\n\r\n|$)", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value;
                MapException[] mapExceptions = exceptionContent.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(line => line.Split(' ').Select(uint.Parse).ToArray())
                    .Select(numbers => new MapException(numbers[1], numbers[0], numbers[2]))
                    .ToArray();
                return new(mapExceptions);
            }

            string content = File.ReadAllText(inputPath);
            SeedIDs = ParseSeedIDs(Regex.Match(content, @"seeds:([\d ]+)\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline).Groups[1].Value);
            SeedToSoil = ParseFarmMap(content, "seed-to-soil map");
            SoilToFertilizer = ParseFarmMap(content, "soil-to-fertilizer map");
            FertilizerToWater = ParseFarmMap(content, "fertilizer-to-water map");
            WaterToLight = ParseFarmMap(content, "water-to-light map");
            LightToTemperature = ParseFarmMap(content, "light-to-temperature map");
            TemperatureToHumidity = ParseFarmMap(content, "temperature-to-humidity map");
            HumidityToLocation = ParseFarmMap(content, "humidity-to-location map");
        }

        public uint GetLocationIDBySeedID(uint seedID, bool printValues = true)
        {
            uint soilID = SeedToSoil.GetValueByID(seedID);
            uint fertilizerID = SoilToFertilizer.GetValueByID(soilID);
            uint waterID = FertilizerToWater.GetValueByID(fertilizerID);
            uint lightID = WaterToLight.GetValueByID(waterID);
            uint temperatureID = LightToTemperature.GetValueByID(lightID);
            uint humidityID = TemperatureToHumidity.GetValueByID(temperatureID);
            uint locationID = HumidityToLocation.GetValueByID(humidityID);
            if (printValues) Console.WriteLine($" - Seed {seedID}, soil {soilID}, fertilizer {fertilizerID}, water {waterID}, light {lightID}, temperature {temperatureID}, humidity {humidityID}, location {locationID}");
            return locationID;
        }
    }

    private static uint IterateAndReturnMinLocationID(Almanac almanac)
    {
        uint printFrequency = 2500000;
        uint totalSeedIdCount = almanac.SeedIDs.Select(pair => pair.range).Aggregate((total, next) => total + next);
        Console.WriteLine($" > Total seeds: {totalSeedIdCount}");
        DateTime lastMoment = DateTime.Now;
        uint countSoFar = 0, lastMomentCount = 0;

        uint resultMinLocationID = uint.MaxValue;
        foreach ((uint start, uint range) in almanac.SeedIDs)
        {
            for (uint currentSeedID = start, end = start + range; currentSeedID < end; currentSeedID++)
            {
                bool shouldPrint = currentSeedID % printFrequency == 0;
                uint currentLocationID = almanac.GetLocationIDBySeedID(currentSeedID, shouldPrint);
                if (currentLocationID < resultMinLocationID) resultMinLocationID = currentLocationID;
                countSoFar++; lastMomentCount++;

                if (shouldPrint)
                {
                    DateTime now = DateTime.Now;
                    double sec = now.Subtract(lastMoment).TotalSeconds;
                    uint rem = totalSeedIdCount - countSoFar;
                    Console.WriteLine($" // processed {lastMomentCount} IDs in {sec:F2}s; {rem / (lastMomentCount / sec):F2}s time left for remaining {rem} IDs");
                    lastMoment = now;
                    lastMomentCount = 0;
                }
            }
        }
        return resultMinLocationID;
    }

    private static void Main(string[] _)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        Almanac almanac = new("input1.txt");
        Console.WriteLine($" > The seed IDs are: {string.Join(", ", almanac.SeedIDs.Select(pair => $"{pair.start} (+{pair.range})"))}");
        Console.WriteLine($" > The lowest location ID is: {IterateAndReturnMinLocationID(almanac)}");
    }
}