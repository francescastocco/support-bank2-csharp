namespace SupportBank2
{
    public class Transaction
    {
        public string Date { get; set; }
        public float Amount { get; set; }
        public string Narrative { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public Transaction(string[] cells)
        {
            Date = cells[0];
            Amount = float.Parse(cells[4]);
            Narrative = cells[3];
            From = cells[1];
            To = cells[2];
        }
    }
}