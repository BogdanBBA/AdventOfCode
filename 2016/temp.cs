namespace IceHockey
{
    public class Country(string code, string nameEnglish, string nameLocal)
    {
        public string Code { get; private set; } = code;
        public string NameEnglish { get; private set; } = nameEnglish;
        public string NameLocal { get; private set; } = nameLocal;

        public override string ToString() => $"{Code}";
    }

    public static class Countries
    {
        public static readonly Country CZE = new("CZE", "Czechia", "Česko");
        public static readonly Country DEN = new("DEN", "Denmark", "Danmark");
        public static readonly Country EES = new("EES", "Estonia", "Eesti");
        public static readonly Country FIN = new("FIN", "Finland", "Suomi");
        public static readonly Country LVA = new("LVA", "Latvia", "Latvija");
        public static readonly Country ROU = new("ROU", "Romania", "România");
        public static readonly Country SVK = new("SVK", "Slovakia", "Slovensko");
        public static readonly Country SWE = new("SWE", "Sweden", "Sverige");
    }

    public class City(Country country, string name)
    {
        public Country Country { get; private set; } = country;
        public string Name { get; private set; } = name;

        public override string ToString() => $"{Name}, {Country.Code}";
    }

    public static class Cities
    {
        public static readonly City CZE_Prague = new(Countries.CZE, "Prague");
        public static readonly City CZE_Ostrava = new(Countries.CZE, "Ostrava");
        public static readonly City DEN_Herning = new(Countries.DEN, "Herning");
        public static readonly City EES_Tallinn = new(Countries.EES, "Tallinn");
        public static readonly City FIN_Helsinki = new(Countries.FIN, "Helsinki");
        public static readonly City FIN_Tampere = new(Countries.FIN, "Tampere");
        public static readonly City LVA_Riga = new(Countries.LVA, "Riga");
        public static readonly City ROU_SfantuGheorghe = new(Countries.ROU, "Sfântu Gheorghe");
        public static readonly City SVK_Bratislava = new(Countries.SVK, "Bratislava");
        public static readonly City SWE_Stockholm = new(Countries.SWE, "Stockholm");
    }

    public class Level(int orderIndex, string displayName)
    {
        public int OrderIndex { get; set; } = orderIndex;
        public string DisplayName { get; set; } = displayName;
    }

    public static class Levels
    {
        public static readonly Level Top = new(0, "Top Division");
        public static readonly Level DivisionI_A = new(11, "Division I Group A");
        public static readonly Level DivisionI_B = new(12, "Division I Group B");
        public static readonly Level DivisionII_A = new(21, "Division II Group A");
        public static readonly Level DivisionII_B = new(22, "Division II Group B");
        public static readonly Level DivisionIII_A = new(31, "Division III Group A");
        public static readonly Level DivisionIII_B = new(32, "Division III Group B");
        public static readonly Level DivisionIV = new(40, "Division IV");
    }

    public class Tournament(int year, Level level, string displayName, City[] hostCities)
    {
        public int Year { get; private set; } = year;
        public Level Level { get; private set; } = level;
        public string DisplayName { get; private set; } = displayName;
        public City[] HostCities { get; private set; } = hostCities;
        public string HostCitiesListed => string.Join(", ", HostCities.Select(city => city.Name));
        public Country[] HostCountries => HostCities.Select(city => city.Country).Distinct().ToArray();
        public string HostCountriesListed => string.Join(" / ", HostCountries.Select(country => country.NameEnglish));

        public override string ToString() => $"[{Year}] {HostCitiesListed}";
    }

    public static class Tournaments
    {
        public static readonly Tournament Top2023 = new(2023, Levels.Top, "Finland / Latvia 2023", [Cities.FIN_Helsinki, Cities.LVA_Riga]);

        public static readonly Tournament Top2024 = new(2024, Levels.Top, "Czechia 2024", [Cities.CZE_Prague, Cities.CZE_Ostrava]);

        public static readonly Tournament Top2025 = new(2025, Levels.Top, "Sweden / Denmark 2025", [Cities.SWE_Stockholm, Cities.DEN_Herning]);
        public static readonly Tournament IA_2025 = new(2025, Levels.DivisionI_A, "Division I-A Romania 2025", [Cities.ROU_SfantuGheorghe]);
        public static readonly Tournament IB_2025 = new(2025, Levels.DivisionI_B, "Division I-B Estonia 2025", [Cities.EES_Tallinn]);
    }

    public class Phase(int orderIndex, string name)
    {
        public int OrderIndex { get; set; } = orderIndex;
        public string Name { get; set; } = name;
    }

    public static class Phases
    {
        public static readonly Phase Group = new(10, "Group");
    }

    public class Match(Tournament tournament, Phase phase, DateTime dateTime)
    {
        public Tournament Tournament { get; private set; } = tournament;
        public Phase Phase { get; private set; } = phase;
        public DateTime DateTime { get; private set; } = dateTime;
    }
}