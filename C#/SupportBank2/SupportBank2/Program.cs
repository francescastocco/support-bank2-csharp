using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace SupportBank2
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = File.ReadAllLines("./Data/Transactions2014.csv");

            List<Transaction> transactionList = new List<Transaction>();

            for (int i = 1; i < file.Length; i++)
            {
                var cells = file[i].Split(",");
                float amountFloat = float.Parse(cells[4]);

                var transaction = new Transaction(cells[0], amountFloat, cells[3], cells[1], cells[2], i);
                Console.WriteLine(transaction.Date + " " + transaction.Amount + " " + transaction.Narrative + " " + transaction.From + " " + transaction.To);

                transactionList.Add(transaction);
            }
            Console.WriteLine(transactionList.Count);
            //transactionList.ForEach(i => Console.WriteLine(i.Date, i.Amount, i.Narrative, i.From, i.To));
        }
    }
}
