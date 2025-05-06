// <copyright file="Transaction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Models
{
    using Microsoft.AspNetCore.OutputCaching;

    public class Transaction
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public decimal Amount { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public TransactionType Type { get; set; }

        public Account Account { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Display,
    }
}
