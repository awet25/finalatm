// <copyright file="AdminServices.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Services
{
    using ATMApp.Data;
    using ATMApp.DTOs;
    using ATMApp.Interfaces;
    using ATMApp.Models;
    using ATMApp.Repositories;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class AdminServices : IAdminservices
    {
        private readonly IUserRepository userRepository;
        private readonly ATMContext context;
        private readonly IAccountRepository accountRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IValidator<CreateUserDto> userValidator;

        public AdminServices(ATMContext aTMContext, IUserRepository userRepository,
        ITransactionRepository transactionRepository, IAccountRepository accountRepository, IValidator<CreateUserDto> userValidator)
        {
            this.context = aTMContext;
            this.userRepository = userRepository;
            this.userValidator = userValidator;
            this.accountRepository = accountRepository;
            this.transactionRepository = transactionRepository;
        }
          /// <summary>
/// This methode will add user in AdminSerivce.cs
/// </summary>
        public async Task<bool> AddUser(CreateUserDto userDto)
        {
            var validationResult = this.userValidator.Validate(userDto);
            if (!validationResult.IsValid)
            {
                foreach (var err in validationResult.Errors)
                {
                    Console.WriteLine(err.ErrorMessage);
                }

                return false;
            }

            var existingUser = await this.userRepository.GetUserBylogin(userDto.Login);
            if (existingUser != null)
            {
                Console.WriteLine("User with this login already exists.");
                return false;
            }

            var newUser = new User
            {
                HolderName = userDto.HolderName,
                Login = userDto.Login,
                PinCode = userDto.PinCode,
                Role = userDto.Role,
            };
            var createdUser = await this.userRepository.AddUser(newUser);
            if (createdUser == null)
            {
                Console.WriteLine("Failed to create User");
                return false;
            }
            else if (createdUser.Role == UserRole.Admin)
            {
                Console.WriteLine($"Account successfully created- the account number assigned is :{createdUser.Id}");
                return true;
            }
            else if (createdUser.Role == UserRole.Client)
            {
                var account = new Account
                {
                    ClientID = createdUser.Id,
                    Status = userDto.Status,
                    IntialBalance = userDto.IntialBalance,
                };
                var createdAccount = await this.accountRepository.CreateAccount(account);
                if (createdAccount == null)
                {
                    Console.WriteLine("sorry Account wasn't created");
                    return false;
                }

                Console.WriteLine($"Account successfully created- the account number assigned is :{createdAccount.Id}");
                return true;
            }

            return false;
        }
               /// <summary>
/// This methode will Delete User and their account
/// </summary>
        public async Task<bool> DeleteUserAndAccount(int accountId, string confirmationInput)
        {
            using var transaction = await this.context.Database.BeginTransactionAsync();
            try
            {
                var account = await this.accountRepository.GetAccountById(accountId);
                if (account == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var user = await this.userRepository.GetUserById(account.ClientID);
                if (user == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                if (!int.TryParse(confirmationInput, out int inputAgain) || inputAgain != accountId)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var transactions = await this.transactionRepository.GetTransactionsByAccountId(accountId);
                if (transactions.Any())
                {
                    this.context.Transactions.RemoveRange(transactions);
                    await this.context.SaveChangesAsync();
                }

                bool accountDeleted = await this.accountRepository.DeleteAccountById(accountId);
                bool userDeleted = await this.userRepository.DeleteUserbyId(account.ClientID);

                if (!accountDeleted || !userDeleted)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
           /// <summary>
/// This methode will return  a User after we add the user to the database
/// </summary>
        public async Task<Account> GetAccount(int id)
        {
            var existingAccount = await this.context.Account.Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);
            if (existingAccount == null)
            {
                Console.WriteLine("Account don't exist");
                return null;
            }

            return existingAccount;
        }
          /// <summary>
/// This methode will return  a User by Login in AdminService
/// </summary>
        public async Task<User> GetUserByLogin(string login)
        {
            // var existingUser=  await _context.User.Include(u=>u.Account)
            //      .FirstOrDefaultAsync(u=>u.Login==login);
            // if (existingUser==null){
            //     Console.WriteLine("User with this login already exists.");
            //     return null;
            // }
            // return existingUser;
            var user = await this.userRepository.GetUserWithAccountByLogin(login);
            if (user == null)
            {
                Console.WriteLine("User not found.");
                return null;
            }

            return user;
        }
              /// <summary>
/// This methode will a bool after  updating a User in AdminService
/// </summary>
        public async Task<bool> UpdateUser(UpdateUserDto updateUserDto)
        {
            using var transaction = await this.context.Database.BeginTransactionAsync();
            try
            {
                var account = await this.accountRepository.GetAccountById(updateUserDto.Id);
                if (account == null)
                {
                    Console.WriteLine("Account not found.");
                    await transaction.RollbackAsync();
                    return false;
                }

                var user = await this.userRepository.GetUserById(account.ClientID);
                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    await transaction.RollbackAsync();
                    return false;
                }

                if (!string.IsNullOrEmpty(updateUserDto.HolderName))
                {
                    user.HolderName = updateUserDto.HolderName;
                }

                if (!string.IsNullOrEmpty(updateUserDto.Login))
                {
                    user.Login = updateUserDto.Login;
                }

                if (!string.IsNullOrEmpty(updateUserDto.PinCode))
                {
                    if (updateUserDto.PinCode.Length != 5)
                    {
                        Console.WriteLine("PinCode must be exactly 5 characters long.");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    user.PinCode = updateUserDto.PinCode;
                }

                var updatedUser = await this.userRepository.UpdateUser(user);
                if (!updatedUser)
                {
                    Console.WriteLine("Failed to update user.");
                    await transaction.RollbackAsync();
                    return false;
                }

                if (updateUserDto.Status != null)
                {
                    account.Status = updateUserDto.Status.Value;

                    var updatedAccount = await this.accountRepository.UpdateAccount(account);
                    if (updatedAccount == null)
                    {
                        Console.WriteLine("Failed to update account status.");
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await transaction.CommitAsync();
                Console.WriteLine($"User {user.HolderName} and account {account.Id} were successfully updated.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error updating user and account: {ex.Message}");
                return false;
            }
        }
    }
}
