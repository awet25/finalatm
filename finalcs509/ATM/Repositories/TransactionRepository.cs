// <copyright file="TransactionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Repositories
{
    using ATMApp.Data;
    using ATMApp.Interfaces;
    using ATMApp.Models;
    using Microsoft.EntityFrameworkCore;

    public class TransactionRepository : ITransactionRepository
    {
        private readonly ATMContext context;

        public TransactionRepository(ATMContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// This methode will add a transaction to an Account when ever the client draw or deposite money.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task AddTransaction(Models.Transaction transaction)
        {
            await this.context.Transactions.AddAsync(transaction);
            await this.context.SaveChangesAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByAccountId(int accountId)
        {
            return await this.context.Transactions
                  .Where(t => t.AccountId == accountId)
                  .ToListAsync() ?? new List<Models.Transaction>();
        }
    }
}
