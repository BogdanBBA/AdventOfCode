namespace AoC12
{
    public class Record
    {
        public int OriginalID { get; private set; }
        public List<RecordItem> RecordItems { get; private set; }
        public int[] DamagedUnitCounts { get; private set; }

        public Record(int id, string line, bool quintuplicate)
        {
            OriginalID = id;
            string itemsPart = line.Split(' ')[0], countsPart = line.Split(' ')[1];
            if (quintuplicate)
            {
                itemsPart = string.Join("?", Enumerable.Range(0, 5).Select(_ => itemsPart));
                countsPart = string.Join(",", Enumerable.Range(0, 5).Select(_ => countsPart));
            }
            RecordItems = itemsPart.Select(ch => ch.ToRecordItem()).ToList();
            DamagedUnitCounts = countsPart.Split(',').Select(int.Parse).ToArray();
        }

        public Record(Record record, int index, RecordItem recordItem)
        {
            OriginalID = record.OriginalID;
            RecordItems = new(record.RecordItems)
            {
                [index] = recordItem
            };
            DamagedUnitCounts = record.DamagedUnitCounts;
        }

        public int CountUnknownItems => RecordItems.Count(item => item == RecordItem.Unknown);

        public ulong EstimatePossibleArrangements => (ulong)Math.Pow(2, CountUnknownItems);

        public override string ToString() => $"[{OriginalID,4}] {string.Join(string.Empty, RecordItems.Select(item => item.ToFormatted()))} : {string.Join(",", DamagedUnitCounts)}";
    }
}