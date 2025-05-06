// <copyright file="ClientView.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Views
{
    using ATMApp.Interfaces;
    using ATMApp.Models;
    using ATMApp.Services;

    public class ClientView
    {
        private readonly IClientService clientService;
        private readonly IAuthService authService;
        private readonly Func<string?> readLine;

        public ClientView(IClientService clientService, IAuthService authService, Func<string?>? readLine = null)
        {
            this.clientService = clientService;
            this.authService = authService;
            this.readLine = readLine ?? Console.ReadLine;
        }

        public async Task Show(User user)
        {
            while (true)
            {
                {
                    Console.WriteLine("Client Menu:");
                    Console.WriteLine("1 - Withdraw cash ");
                    Console.WriteLine("2 - Deposit Cash");
                    Console.WriteLine("3 - Display Balance");
                    Console.WriteLine("4 - Exit");
                    Console.WriteLine("Enter one of the above options");
                    int choice;
                    string? input = this.readLine();
                    if (!int.TryParse(input, out choice))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("you have choosen to Withdraw Money");
                            await this.WithdrawMoney(user);
                            break;
                        case 2:
                            Console.WriteLine("you have choose to Deposite Money");
                            await this.Deposite(user);
                            break;
                        case 3:
                            Console.WriteLine("You have choose to View you money");
                            await this.DisplayAccount(user);
                            break;

                        case 4:
                            Console.WriteLine("Exiting Client menu...");
                            this.Exit();

                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
            }
        }

        public async Task DisplayAccount(User user)
        {
            await this.clientService.GetBalance(user.Id);
        }

        public async Task WithdrawMoney(User user)
        {
            decimal amount;
            Console.WriteLine("Enter the amount you want to withdraw");
            string input = Console.ReadLine();

            while (!decimal.TryParse(input, out amount) || amount <= 0)
            {
                Console.WriteLine("Invalid input please enter a valid amount greater than 0.");
                input = Console.ReadLine();
            }

            await this.clientService.Withdraw(user.Id, amount);
        }

        public async Task Deposite(User user)
        {
            Console.WriteLine("Enter amount you want to deposite");

            string? input = this.readLine();

            if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Invalid amount entered.");
                return;
            }

            await this.clientService.Deposit(user.Id, amount);
        }

        public void Exit()
        {
            this.authService.Exit();
        }
    }
}
