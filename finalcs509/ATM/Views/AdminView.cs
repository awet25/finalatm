// <copyright file="AdminView.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Views
{
    using ATMApp.DTOs;
    using ATMApp.Interfaces;
    using ATMApp.Models;

    public class AdminView
    {
        private readonly IAdminservices adminServices;
        private readonly IAuthService authService;
        private readonly Func<string> input;

        public AdminView(IAdminservices adminServices, IAuthService authService, Func<string> input)
        {
            this.adminServices = adminServices;
            this.authService = authService;
            this.input = input;
        }

        public async Task Show()
        {
            while (true)
            {
                Console.WriteLine("\nAdmin Menu:");
                Console.WriteLine("1 - Create New  user ");
                Console.WriteLine("2 - Delete Existing Account");
                Console.WriteLine("3 - Update Account info ");
                Console.WriteLine("4 - Search for Account ");
                Console.WriteLine("5 - Exit");
                Console.WriteLine("Enter your choice");
                if (!int.TryParse(this.input(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("you have choosen to create new account");
                        await this.CreateUser();
                        break;
                    case 2:
                        Console.WriteLine("follow instructions to delete Account");
                        await this.DeleteAccount();
                        break;
                    case 3:
                        Console.WriteLine("Follow instruction to updateUser info");
                        await this.UpdateAccount();
                        break;
                    case 4:
                        Console.WriteLine("Follow instruction to search for an account");
                        await this.SearchForAccount();
                        break;
                    case 5:
                        this.Exit();
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        /// <summary>
        /// Will create a User.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<bool> CreateUser()
        {
            CreateUserDto newuser = HandleCreateUserInput();
            return await this.adminServices.AddUser(newuser);
        }

        public async Task<bool> DeleteAccount()
        {
            Console.WriteLine("Enter Id for the Account you want to delete:");
            if (!int.TryParse(this.input(), out int accountId))
            {
                Console.WriteLine("Invalid account Id!");
                return false;
            }

            Console.Write($"Please confirm deletion by re-entering account number ({accountId}): ");
            string? confirmationInput = this.input();

            return await this.adminServices.DeleteUserAndAccount(accountId, confirmationInput);
        }

        public async Task<bool> UpdateAccount()
        {
            UpdateUserDto updateduser = HandleInputToUpudate();
            return await this.adminServices.UpdateUser(updateduser);
        }

        /// <summary>
        /// search for account.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task SearchForAccount()
        {
            Console.WriteLine("Enter Account ID to look for account ");
            string stringId = this.input();
            while (!int.TryParse(stringId, out int id) || id <= 0)
            {
                Console.WriteLine("Invalid Input please enter a valid Id");
                stringId = this.input();
            }

            var account = await this.adminServices.GetAccount(int.Parse(stringId));
            if (account == null)
            {
                Console.WriteLine("sorry account not found");
                return;
            }

            Console.WriteLine("The Account information is :");
            Console.WriteLine($"Account # {account.Id}");
            Console.WriteLine($"Holder:  {account.User.HolderName}");
            Console.WriteLine($"Balance: {account.IntialBalance}");
            Console.WriteLine($"Status: {account.Status}");
            Console.WriteLine($"Login: {account.User.Login}");
            Console.WriteLine($"Pin Code: {account.User.PinCode}");
        }

        /// <summary>
        /// Exit the program.
        /// </summary>
        public void Exit()
        {
            Console.WriteLine("Exiting Admin menu...");
            this.authService.Exit();
        }

        public static CreateUserDto HandleCreateUserInput()
        {
            // Keep using Console.ReadLine here since static methods don't have access to _input
            // You may refactor this as needed
            return new CreateUserDto();
        }

        public static UpdateUserDto HandleInputToUpudate()
        {
            return new UpdateUserDto();
        }

        public static bool AskUserToEdit(string field)
        {
            Console.Write($"Do you want to update {field}? (yes/no): ");
            string response = Console.ReadLine()?.Trim().ToLower();
            return response == "yes" || response == "y";
        }
    }
}
