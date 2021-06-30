using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var file = File.ReadAllLines("./Data/Transactions2014.csv");

            var transactionList = new List<Transaction>();

            for (var i = 1; i < file.Length; i++)
            {
                var cells = file[i].Split(",");
                var transaction = new Transaction(cells);
                transactionList.Add(transaction);
            }

            var accountNames = transactionList.Select(transaction => transaction.From).Distinct();
            var accountsList = accountNames.Select(name => new Account(name)).ToList();

            accountsList.ForEach(account =>
            {
                account.TransactionHistoryOwe = transactionList.FindAll(transaction => transaction.From == account.Name);
                account.TransactionHistoryOwed = transactionList.FindAll(transaction => transaction.To == account.Name);

                Console.WriteLine("Name: " + account.Name + ". Total Owing: " + account.TotalOwe() + ". Total Owed: " + account.TotalOwed());
              
            });
        }
    }
}
