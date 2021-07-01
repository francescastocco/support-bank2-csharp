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

            var transactions = file.Skip(1).Select(line => new Transaction(line.Split(","))).ToList();
            var accounts = transactions.Select(transaction => transaction.From).Distinct().Select(name => new Account(name)).ToList();

            accounts.ForEach(account =>
            {
                account.TransactionHistoryOwe = transactions.FindAll(transaction => transaction.From == account.Name);
                account.TransactionHistoryOwed = transactions.FindAll(transaction => transaction.To == account.Name);
            });

            Console.Write("Would you like a list of all the accounts? y/n?");
            var listAllAccounts = Console.ReadLine();

            if (listAllAccounts.ToLower() == "y")
            {
                Console.WriteLine("Date \t\t Amount \t From \t To \t\t Narrative");
                transactions.ForEach(transaction =>
                {
                    Console.WriteLine(transaction.Date + "\t" + transaction.Amount + "\t\t" + transaction.From + "\t" + transaction.To + "\t\t" + transaction.Narrative);
                });
            }

            Console.Write("Would you like a specific account y/n?");
            var accountRequest = Console.ReadLine();

            if (accountRequest.ToLower() == "y")
            {
                Console.Write("Who's account would you like to see?");
                var accountName = Console.ReadLine();
                var account = accounts.Find(account => account.Name.ToLower() == accountName.ToLower());
                if (account != null)
                {
                    Console.WriteLine("Date \t\t Amount \t From \t To \t\t Narrative");
                    account.TransactionHistoryOwe.ForEach(transaction =>
                    {
                        Console.WriteLine(transaction.Date + "\t" + transaction.Amount + "\t\t" + transaction.From + "\t" + transaction.To + "\t\t" + transaction.Narrative);
                    });
                    Console.WriteLine("Name: " + account.Name + ". Total Owing: " + account.TotalOwe() + ". Total Owed: " + account.TotalOwed());
                }
                else
                {
                    Console.WriteLine($"{accountName} does not exist");
                }
        }
        }

    }
}


