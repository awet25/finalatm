// <copyright file="ClientService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Services
{
    using System.Threading.Tasks;
    using ATMApp.Interfaces;
    using ATMApp.Models;

    public class ClientService : IClientService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IAccountRepository accountRepository;

        public ClientService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
        }

        /// <summary>
        /// This methode will return  a bool after a client try to deposite money.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<bool> Deposit(int clientId, decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Deposit amount must be greater than zero.");
                return false;
            }

            var account = await this.accountRepository.GetAccountByClientID(clientId);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
                return false;
            }

            if (account.Status.Equals(AccountStatus.Disabled))
            {
                Console.WriteLine("Sorry this Account was Disabled pls visit our office or call us");
                return false;
            }

            account.IntialBalance += amount;
            await this.accountRepository.UpdateAccount(account);

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Type = TransactionType.Deposit,
            };
            await this.transactionRepository.AddTransaction(transaction);

            Console.WriteLine($" Cash Deposited Successfully ");
            Console.WriteLine($"Account #{account.Id}");
            Console.WriteLine($"Date:{DateTime.Now.ToString("MM/dd/yyyy")}");
            Console.WriteLine($"Deposited : {amount}");
            Console.WriteLine($"Balance :{account.IntialBalance:F2}");

            return true;
        }

        /// <summary>
        /// This methode will return  will display the current balance of the account.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task GetBalance(int clientId)
        {
            var account = await this.accountRepository.GetAccountByClientID(clientId);
            if (account == null)
            {
                Console.WriteLine("Account not found.");
            }
            else if (account.Status.Equals(AccountStatus.Disabled))
            {
                Console.WriteLine("Sorry this Account was Disabled pls visit our office or call us");
                return;
            }
            else
            {
                Console.WriteLine(" Account Info");
                Console.WriteLine($"Account #{account.Id}");
                Console.WriteLine($"Date:{DateTime.Now.ToString("MM/dd/yyyy")}");
                Console.WriteLine($"Balance :{account.IntialBalance:F2}");
            }
        }

        /// <summary>
        /// This methode will return a list of transactions  done by User using accountId.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<List<Transaction>> GetTransactionHistory(int accountId)
        {
            return await this.transactionRepository.GetTransactionsByAccountId(accountId);
        }

        /// <summary>
        /// This methode will allow the client to withdraw money.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<bool> Withdraw(int clientID, decimal amount)
        {
            var account = await this.accountRepository.GetAccountByClientID(clientID);
            if (account == null)
            {
                Console.WriteLine($"Client {clientID} does not exist");
                return false;
            }
            else if (account.Status.Equals(AccountStatus.Disabled))
            {
                Console.WriteLine("Sorry this Account was Disabled pls visit our office or call us");
                return false;
            }

            if (account.IntialBalance < amount)
            {
                Console.WriteLine("Insufficient balance.");
                return false;
            }

            account.IntialBalance -= amount;
            await this.accountRepository.UpdateAccount(account);
            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Type = TransactionType.Withdrawal,
            };
            await this.transactionRepository.AddTransaction(transaction);
            Console.WriteLine($" Cash Successfully Withdrawn ");
            Console.WriteLine($"Account #{account.Id}");
            Console.WriteLine($"Date:{DateTime.Now.ToString("MM/dd/yyyy")}");
            Console.WriteLine($"Withdrawn : {amount}");
            Console.WriteLine($"Balanace :{account.IntialBalance:F2}");
            return true;
        }
    }
}
