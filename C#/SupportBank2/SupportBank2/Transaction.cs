namespace SupportBank2
{
    public class Transaction
    {
        public string Date { get; set; }
        public float Amount { get; set; }
        public string Narrative { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Id { get; set; }
        public Transaction(string date, float amount, string narrative, string from, string to, int id)

        {
            Date = date;
            Amount = amount;
            Narrative = narrative;
            From = from;
            To = to;
            Id = id;
        }
    }
}