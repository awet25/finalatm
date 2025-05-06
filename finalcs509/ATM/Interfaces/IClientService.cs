// <copyright file="IClientService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Interfaces
{
    using System.Threading.Tasks;
    using ATMApp.Models;

    public interface IClientService
    {
        Task<bool> Deposit(int accountId, decimal amount);

        Task<bool> Withdraw(int clientID, decimal amount);

        Task GetBalance(int accountId);

        Task<List<Transaction>> GetTransactionHistory(int accountId);
    }
}
