// <copyright file="AccountRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Repositories
{
    using ATMApp.Data;
    using ATMApp.DTOs;
    using ATMApp.Interfaces;
    using ATMApp.Models;
    using Microsoft.EntityFrameworkCore;

    public class AccountRepository : IAccountRepository
    {
        private readonly ATMContext context;

        public AccountRepository(ATMContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// This methode will create an Account in Account Repo.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<Account> CreateAccount(Account newAccount)
        {
            var account = await this.context.Account.AddAsync(newAccount);
            await this.context.SaveChangesAsync();
            return account.Entity;
        }

        /// <summary>
        /// This methode will return  an Account by accountId in Account Repo.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<Account> GetAccountById(int id)
        {
            return await this.context.Account.FindAsync(id);
        }

        /// <summary>
        /// This methode will delete an Account by accountId in Account Repo.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<bool> DeleteAccountById(int accountId)
        {
            var account = await this.GetAccountById(accountId);
            if (account == null)
            {
                return false;
            }

            this.context.Account.Remove(account);
            await this.context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// This methode will update  an Account by accountId in Account Repo.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<Account> UpdateAccount(Account account)
        {
            var newaccount = this.context.Account.Update(account);
            await this.context.SaveChangesAsync();
            return account;
        }

        /// <summary>
        /// This methode will return  an Account by clientId in Account Repo.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<Account> GetAccountByClientID(int clientId)
        {
            var account = await this.context.Account
            .FirstOrDefaultAsync(a => a.ClientID == clientId);
            if (account == null)
            {
                return null;
            }

            return account;
        }
    }
}
