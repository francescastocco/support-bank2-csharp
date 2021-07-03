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
            Logger.Debug($"Creating account, name: {name}");
        }

        public void PrintAllTransactions()
        {
           Console.WriteLine("Date \t\t\tAmount \t\tFrom \tTo \t\tNarrative");
            PrintTransactions(TransactionHistoryOwe);
            PrintTransactions(TransactionHistoryOwed);
            Console.WriteLine($"\nName: {Name} \nTotal Owing: {GetTotals(TransactionHistoryOwe)} \nTotal Owed: {GetTotals(TransactionHistoryOwed)}");
        }

        private static void PrintTransactions(List<Transaction> transactions)
        {
            transactions.ForEach(transaction =>
            {
                Console.WriteLine($"{transaction.Date} \t{transaction.Amount} \t\t{transaction.From} \t{transaction.To} \t\t{transaction.Narrative}");
            });
        }

        public static float GetTotals(List<Transaction> transactions)
        {
            return transactions.Select(transaction => transaction.Amount).Sum();
        }
    }
}
