// <copyright file="ITransactionRepositiry.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Interfaces
{
    using ATMApp.Models;

    public interface ITransactionRepository
    {
        Task AddTransaction(Transaction transaction);

        Task<List<Transaction>> GetTransactionsByAccountId(int accountId);
    }
}
