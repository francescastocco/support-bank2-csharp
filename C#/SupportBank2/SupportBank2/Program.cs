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
            RequestAccountBalances(accounts);
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
            switch (year)
            {
                case "2012":
                    transactions = ParseXML("./Data/Transactions2012.xml");
                    break;
                case "2013":
                    transactions = ParseJson("./Data/Transactions2013.json");
                    break;
                case "2014":
                    transactions = ParseCSV("./Data/Transactions2014.csv");
                    break;
                case "2015":
                    transactions = ParseCSV("./Data/DodgyTransactions2015.csv");
                    break;
                default:
                    Console.WriteLine("No transactions exist for this year");
                    Logger.Warn($"Incorrect Year Entered! User input: '{year}'");
                    return GetTransactions();
            }
            return transactions;
        }

        private static List<Transaction> ParseCSV(string path)
        {
            Logger.Info($"Parsing {path}");
            var file = File.ReadAllLines(path);
            return file
                .Skip(1)
                .Where(line => IsTransactionValid(line.Split(",")))
                .Select(line => new Transaction(line.Split(",")))
                .ToList();
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
            var file = new XmlDocument();
            file.Load(path);
            var nodes = file.SelectNodes("TransactionList/SupportTransaction");
            var transactions = new List<Transaction>();

            foreach (XmlElement node in nodes)
            {
                var value = new List<string>();
                value.Add(DateTime
                    .FromOADate(Convert.ToDouble(node.GetAttribute("Date")))
                    .ToString("dd'/'MM'/'yyyy")
                    );
                value.Add(node.SelectSingleNode("Parties/From")?.InnerText);
                value.Add(node.SelectSingleNode("Parties/To")?.InnerText);
                value.Add(node.SelectSingleNode("Description")?.InnerText);
                value.Add(node.SelectSingleNode("Value")?.InnerText);

                if (IsTransactionValid(value))
                {
                    transactions.Add(new Transaction(value));
                }
            }

            return transactions;
        }

        private static bool IsTransactionValid(IList<string> cells)
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

        private static void RequestAccountBalances(List<Account> accounts)
        {
            Console.Write("Would you like a summary of account balances? y/n");
            var userInput = Console.ReadLine();
            Logger.Info($"User input: '{userInput}'");
            if (userInput.ToLower() == "y")
            {
                Console.WriteLine("Name \t\tAmount Owing \t\tAmount Owed");
                accounts.ForEach(account =>
                {
                    Console.WriteLine($"{account.Name} \t\t{Account.GetTotals(account.TransactionHistoryOwe)} \t\t{Account.GetTotals(account.TransactionHistoryOwe)}");
                });
            }
        }

        private static void RequestSpecificAccount(List<Account> accounts)
        {
            Console.Write("Would you like a specific account? y/n");
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
