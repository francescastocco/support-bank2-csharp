using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
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
            ConfigureLogging();
            var transactions = GetTransactions();
            var accounts = GetAccounts(transactions);
            PopulateTransactionInAccounts(accounts, transactions);
            RequestAllAccounts(transactions);
            RequestSpecificAccount(accounts);
        }

        private static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = "../../../Log/SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, target));
            LogManager.Configuration = config;
        }

        private static List<Transaction> GetTransactions()
        {
            Console.Write("Which year do you want to see?");
            var year = Console.ReadLine();
            var transactions = new List<Transaction>();
            if (year != "2014" && year != "2015" && year != "2013" && year != "2012")
            {
                Console.WriteLine("No transactions exist for this year");
                Logger.Warn($"Incorrect Year Entered! User input: '{year}'");
                return GetTransactions();
            }
            if (year == "2014")
            {
                transactions = ParseCSV("./Data/Transactions2014.csv");
            }
            if (year == "2015")
            {
                transactions = ParseCSV("./Data/DodgyTransactions2015.csv");
            }
            if (year == "2013")
            {
                transactions = ParseJson("./Data/Transactions2013.json");
            }
            if (year == "2012")
            {
                transactions = ParseXML("./Data/Transactions2012.xml");
            }
            return transactions;
        }

        private static List<Transaction> ParseCSV(string path)
        {
            Logger.Info($"Parsing {path}");
            var file = File.ReadAllLines(path);
            return file.Skip(1).Where(line => IsTransactionValid(line.Split(","))).Select(line => new Transaction(line.Split(","))).ToList();
        }

        private static List<Transaction> ParseJson(string path)
        {
            Logger.Info($"Parsing {path}");
            var file = File.ReadAllText(path);

            var generator = new JSchemaGenerator();
            var schema = generator.Generate(typeof(Transaction));
                       
            var attemptedTransactions = JsonConvert.DeserializeObject<List<JObject>>(file);
            var transactions = new List<Transaction>();
            foreach (var attemptedTransaction in attemptedTransactions)
            {
                if (attemptedTransaction.IsValid(schema))
                {
                    transactions.Add(attemptedTransaction.ToObject<Transaction>());
                }
                else
                {
                    var error = "";
                    foreach (var property in attemptedTransaction.Properties())
                    {
                        error += $"{property.Name} - {property.Value} ";
                    }
                    Logger.Error($"Failed to create transaction from JObject {error}");
                }
            }
            return transactions;
        }

        private static List<Transaction> ParseXML(string path)
        {
            Logger.Info($"Parsing {path}");
            var xml = new XmlDocument();
            xml.Load(path);
            return;
        }

        private static bool IsTransactionValid(string[] cells)
        {
            var validAmount = float.TryParse(cells[4], out _);
            var validDate = DateTime.TryParse(cells[0], out _);
            var error = "";
            if (!validAmount)
            {
                error += "Amount can't be parsed. ";
            }
            if (!validDate)
            {
                error += "Date can't be parsed. ";
            }
            if (!string.IsNullOrEmpty(error))
            {
                Logger.Error($"{error}for transaction values {cells[0]}, {cells[1]}, {cells[2]}, {cells[3]}, {cells[4]}");
            }
            return validAmount && validDate;
        }

        private static List<Account> GetAccounts(List<Transaction> transactions)
        {
            return transactions.Select(transaction => transaction.From).Distinct().Select(name => new Account(name)).ToList();
        }

        private static void PopulateTransactionInAccounts(List<Account> accounts, List<Transaction> transactions)
        {
            Logger.Info("Populating transactions in accounts");
            accounts.ForEach(account =>
            {
                account.TransactionHistoryOwe = transactions.FindAll(transaction => transaction.From == account.Name);
                account.TransactionHistoryOwed = transactions.FindAll(transaction => transaction.To == account.Name);
            });
        }

        private static void RequestAllAccounts(List<Transaction> transactions)
        {
            Console.Write("Would you like a list of all the accounts? y/n?");
            var listAllAccounts = Console.ReadLine();
            Logger.Info($"User input: '{listAllAccounts}'");
            if (listAllAccounts.ToLower() == "y")
            {
                Console.WriteLine("Date \t\tAmount \tFrom \tTo \t\tNarrative");
                transactions.ForEach(transaction =>
                {
                    Console.WriteLine($"{transaction.Date} \t{transaction.Amount} \t\t{transaction.From} \t{transaction.To} \t\t{transaction.Narrative}");
                });
            }
        }

        private static void RequestSpecificAccount(List<Account> accounts)
        {
            Console.Write("Would you like a specific account y/n?");
            var accountRequest = Console.ReadLine();
            Logger.Info($"User input: '{accountRequest}'");
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
                    RequestSpecificAccount(accounts);
                }
            }
        }

    }
}


