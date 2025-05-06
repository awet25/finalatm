// <copyright file="IAccountRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Interfaces
{
    using ATMApp.DTOs;
    using ATMApp.Models;

    public interface IAccountRepository
    {
        Task<Account> CreateAccount(Account account);

        Task<Account> GetAccountByClientID(int clientId);

        Task<Account> GetAccountById(int id);

        Task<Account> UpdateAccount(Account account);

        Task<bool> DeleteAccountById(int accountId);
    }
}
