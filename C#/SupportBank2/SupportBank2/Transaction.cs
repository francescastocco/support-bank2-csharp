using System;
using NLog;

namespace SupportBank2
{
    public class Transaction
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public string Narrative { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public Transaction(string[] cells)
        {
            Date = DateTime.Parse(cells[0]);
            Narrative = cells[3];
            From = cells[1];
            To = cells[2];
            Amount = float.Parse(cells[4]);
            Logger.Debug($"Creating transaction with values {cells[0]}, {cells[1]}, {cells[2]}, {cells[3]}, {cells[4]}");
        }
    }
}