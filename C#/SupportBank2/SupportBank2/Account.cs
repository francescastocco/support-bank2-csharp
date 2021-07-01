using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank2
{
    public class Account
    {
        public string Name { get; }
        public List<Transaction> TransactionHistoryOwe { get; set; } = new List<Transaction>();
        public List<Transaction> TransactionHistoryOwed { get; set; } = new List<Transaction>();

        public Account(string name)
        {
            Name = name;
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
