using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank2
{
    internal class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = "./Log/SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            var transactions = GetTransactions();
            var accounts = GetAccounts(transactions);

            Logger.Info("Hello world");

            RequestAccounts(transactions);
            RequestSpecificAccount(accounts);
        }

        public static List<Transaction> GetTransactions() {
            Console.Write("Which year do you want to see?");
            var year = Console.ReadLine();
            var file = Array.Empty<string>();
            if (year == "2014")
            {
               file = File.ReadAllLines("./Data/Transactions2014.csv");
            } else if(year == "2015")
            {
                file = File.ReadAllLines("./Data/DodgyTransactions2015.csv");
            } else
            {
                Console.WriteLine("No transactions exist for this year");
                Logger.Info("Incorrect Year Entered!");
                System.Environment.Exit(0);
            }
            
            return file.Skip(1).Select(line => new Transaction(line.Split(","))).ToList();
        }

        public static List<Account> GetAccounts(List<Transaction> transactions)
        {
            var accounts = transactions.Select(transaction => transaction.From).Distinct().Select(name => new Account(name)).ToList();

            accounts.ForEach(account =>
            {
                account.TransactionHistoryOwe = transactions.FindAll(transaction => transaction.From == account.Name);
                account.TransactionHistoryOwed = transactions.FindAll(transaction => transaction.To == account.Name);
            });

            return accounts;
        }

        public static void RequestAccounts(List<Transaction> transactions)
        {
            Console.Write("Would you like a list of all the accounts? y/n?");
            var listAllAccounts = Console.ReadLine();

            if (listAllAccounts.ToLower() == "y")
            {
                Console.WriteLine("Date \t\tAmount \tFrom \tTo \t\tNarrative");
                transactions.ForEach(transaction =>
                {
                    Console.WriteLine($"{transaction.Date} \t{transaction.Amount} \t\t{transaction.From} \t{transaction.To} \t\t{transaction.Narrative}");
                });
            }
        }

        public static void RequestSpecificAccount(List<Account> accounts)
        {
            Console.Write("Would you like a specific account y/n?");
            var accountRequest = Console.ReadLine();

            if (accountRequest.ToLower() == "y")
            {
                Console.Write("Who's account would you like to see?");
                var accountName = Console.ReadLine();
                var account = accounts.Find(account => account.Name.ToLower() == accountName.ToLower());
                if (account != null)
                {
                    account.PrintAllTransactions();
                }
                else
                {
                    Console.WriteLine($"{accountName} does not exist");
                    Logger.Info("Incorrect account name entered!");
                    System.Environment.Exit(0);
                }
            }
        }

    }
}


