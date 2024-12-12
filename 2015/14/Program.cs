using System.Text.RegularExpressions;

namespace AoC2015.Day14
{
    public class Reindeer
    {
        public string Name { get; private set; }
        public int Speed { get; private set; }
        public int FlyDuration { get; private set; }
        public int RestDuration { get; private set; }

        public bool IsFlying { get; private set; }
        public int DistanceFlown { get; private set; }
        public int _secondsDoingCurrentAction;

        public Reindeer(string line)
        {
            Match match = Regex.Match(line, @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.");
            Name = match.Groups[1].Value;
            Speed = int.Parse(match.Groups[2].Value);
            FlyDuration = int.Parse(match.Groups[3].Value);
            RestDuration = int.Parse(match.Groups[4].Value);
            Reset();
        }

        public void Reset()
        {
            IsFlying = true;
            DistanceFlown = 0;
            _secondsDoingCurrentAction = 0;
        }

        public void Step()
        {
            _secondsDoingCurrentAction++;
            if (IsFlying)
            {
                DistanceFlown += Speed;
            }
            if (IsFlying && _secondsDoingCurrentAction >= FlyDuration || !IsFlying && _secondsDoingCurrentAction >= RestDuration)
            {
                IsFlying = !IsFlying;
                _secondsDoingCurrentAction = 0;
            }
        }

        public string ToRaceStateString(int place)
            => $"{place}. {Name}: has flown {DistanceFlown} km, is now {(IsFlying ? "flying" : "resting")}";

        public string ToRaceStateString_NewRules(int points, int place)
            => $"{place}. {Name}: has {points} pts, after having flown {DistanceFlown} km, is now {(IsFlying ? "flying" : "resting")}";

        public override string ToString()
            => $"{Name}: {Speed} km/s ({FlyDuration}s), rests {RestDuration}s";
    }

    public class Program : IDayProgram
    {
        public override int GetCurrentDay => 14;
        public override int GetCurrentPart => 2;

        private static void RunRace(List<Reindeer> reindeers, int duration)
        {
            reindeers.ForEach(reindeer => reindeer.Reset());
            for (int second = 1; second <= duration; second++)
            {
                reindeers.ForEach(reindeer => reindeer.Step());
            }
            List<Reindeer> sortedReindeers = [.. reindeers.OrderByDescending(reindeer => reindeer.DistanceFlown)];
            Console.WriteLine($" > After the {duration}s race, the reindeer look as follows:");
            Console.WriteLine(string.Join("\n", Enumerable.Range(1, reindeers.Count).Select(place => sortedReindeers[place - 1].ToRaceStateString(place))));
        }

        private static void RunRace_NewRules(List<Reindeer> reindeers, int duration)
        {
            Dictionary<Reindeer, int> points = reindeers.ToDictionary(r => r, _ => 0);
            reindeers.ForEach(reindeer => reindeer.Reset());
            for (int second = 1; second <= duration; second++)
            {
                reindeers.ForEach(reindeer => reindeer.Step());
                List<Reindeer> sortedReindeers = [.. reindeers.OrderByDescending(reindeer => reindeer.DistanceFlown)];
                for (int i = 0; sortedReindeers[i].DistanceFlown == sortedReindeers[0].DistanceFlown; i++)
                    points[sortedReindeers[i]]++;
            }
            Console.WriteLine($" > After the {duration}s race, the reindeer look as follows:");
            int place = 0;
            Console.WriteLine(string.Join("\n", points.OrderByDescending(p => p.Value).Select(p => p.Key.ToRaceStateString_NewRules(p.Value, ++place))));
        }

        public override void Run()
        {
            string[] lines = File.ReadAllLines(GetInputFilePath(GetCurrentPart == 0 ? 0 : 1));
            List<Reindeer> reindeers = lines.Select(line => new Reindeer(line)).ToList();
            int raceDuration = GetCurrentPart == 0 ? 1000 : 2503;
            if (GetCurrentPart <= 1)
                RunRace(reindeers, raceDuration);
            else
                RunRace_NewRules(reindeers, raceDuration);
        }
    }
}