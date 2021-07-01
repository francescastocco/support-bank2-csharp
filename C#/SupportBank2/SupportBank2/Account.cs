using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace SupportBank2
{
    public class Account
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public string Name { get; }
        public List<Transaction> TransactionHistoryOwe { get; set; } = new List<Transaction>();
        public List<Transaction> TransactionHistoryOwed { get; set; } = new List<Transaction>();

        public Account(string name)
        {
            Name = name;
        }

        public void PrintAllTransactions()
        {
           Console.WriteLine("Date \t\tAmount \t\tFrom \tTo \t\tNarrative");
           TransactionHistoryOwe.ForEach(transaction =>
            {
                Console.WriteLine($"{transaction.Date} \t{transaction.Amount} \t\t{transaction.From} \t{transaction.To} \t\t{transaction.Narrative}");
            });
            TransactionHistoryOwed.ForEach(transaction =>
            {
                Console.WriteLine($"{transaction.Date} \t{transaction.Amount} \t\t{transaction.From} \t{transaction.To} \t\t{transaction.Narrative}");
            });
            Console.WriteLine($"\nName: {Name} \nTotal Owing: {TotalOwe()} \nTotal Owed: {TotalOwed()}");
        }

        public float TotalOwe()
        {
            return TransactionHistoryOwe.Select(transaction => transaction.Amount).Sum();
        }

        public float TotalOwed()
        {
            var sum = TransactionHistoryOwed.Select(transaction => transaction.Amount).Sum();
            return sum;
        }
    }
}
