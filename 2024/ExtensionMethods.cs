namespace AoC2024
{
    public static class ExtensionMethods
    {
        public static int ParseAsInt(this string value) => int.Parse(value);
        public static int ParseAsInt(this char value) => $"{value}".ParseAsInt();

        public static string Plural(this int count, string singular, bool includeCount = true)
        {
            string plural = count == 1 ? singular : $"{singular}s";
            return includeCount ? $"{count} {plural}" : plural;
        }

        public static void LogNNL(this string text)
            => Console.Write(text);

        public static void LogDNL(this string text)
            => Console.WriteLine(text + Environment.NewLine);

        public static void Log(this string text)
            => Console.WriteLine(text);
    }
}