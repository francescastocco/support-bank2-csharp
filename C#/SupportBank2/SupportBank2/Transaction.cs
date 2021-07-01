using System;
using NLog;

namespace SupportBank2
{
    public class Transaction
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public string Date { get; set; }
        public float Amount { get; set; }
        public string Narrative { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public Transaction(string[] cells)
        {
            Date = cells[0];
            Narrative = cells[3];
            From = cells[1];
            To = cells[2];

            if (float.TryParse(cells[4], out float result))
            {
                Amount = result;
            }
            else
            {
                Logger.Error("Transaction amount was not a float");
            }
        }
    }
}