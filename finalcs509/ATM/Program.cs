// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// service.AddScoped<IAuthService, AuthService>();
//                 service.AddScoped<IUserRepository, UserRepository>();
//                 service.AddScoped<IAccountRepository, AccountRepository>();
//                 service.AddScoped<ITransactionRepository, TransactionRepository>();
//                 service.AddScoped<IClientService, ClientService>();
//                 service.AddValidatorsFromAssemblyContaining<AddNewuserValidator>();
//                 service.AddScoped<IValidator<CreateUserDto>, AddNewuserValidator>();
//                 service.AddScoped<IAdminservices, AdminServices>();
//                 service.AddScoped<AdminView>(provider =>
// {
//     var adminServices = provider.GetRequiredService<IAdminservices>();
//     var authService = provider.GetRequiredService<IAuthService>();
//     return new AdminView(adminServices, authService, Console.ReadLine);
// });

// service.AddScoped<ClientView>(
//                     provider =>
// {
//     var clientService = provider.GetRequiredService<IClientService>();
//     var authService = provider.GetRequiredService<IAuthService>();
//     return new ClientView(clientService, authService, Console.ReadLine);
// }
//                 );
//             }).Build();

// using (var scope = builder.Services.CreateScope())
//             {
//                 var serviceProvider = scope.ServiceProvider;
//                 var authService = serviceProvider.GetRequiredService<IAuthService>();
//                 var adminView = serviceProvider.GetRequiredService<AdminView>();
//                 var clientView = serviceProvider.GetRequiredService<ClientView>();

// Console.WriteLine("Welcome to our ATM system!");

// while (true)
//                 {
//                     Console.Write("Enter your login: ");
//                     string login = Console.ReadLine();

// Console.Write("Enter your pincode: ");
//                     string pinCode = Console.ReadLine();

// var userLoginDto = new UserLoginDTO
//                     {
//                         Login = login,
//                         PinCode = pinCode
//                     };

// User isAuthenticated = await authService.Login(userLoginDto);

// if (isAuthenticated == null)
//                     {
//                         Console.WriteLine("Invalid credentials. Please try again.");
//                         continue;
//                     }

// if (isAuthenticated.Role == UserRole.Admin)
//                     {
//                         Console.WriteLine($"Welcome Admin {isAuthenticated.HolderName}!");
//                         await adminView.Show();
//                     }
//                     else if (isAuthenticated.Role == UserRole.Client)
//                     {
//                         Console.WriteLine($"Welcome Client {isAuthenticated.HolderName}!");
//                         await clientView.Show(isAuthenticated);
//                     }

// // 🛑 If user logs out, restart login loop
//                     Console.WriteLine("Returning to login screen...\n");
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error: {ex.Message}");
//         }
//     }
// }
using System;
using System.IO;
using System.Threading.Tasks;
using ATMApp.Data;
using ATMApp.DTOs;
using ATMApp.Interfaces;
using ATMApp.Models;
using ATMApp.Repositories;
using ATMApp.Services;
using ATMApp.Validators;
using ATMApp.Views;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            using var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            await RunAsync(Console.ReadLine, Console.Out, services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDbContext<ATMContext>(options =>
                    options.UseMySql(
                        "server=localhost;database=atmdb;user=root;password=9361;",
                        new MySqlServerVersion(new Version(8, 0, 41))));

                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IAccountRepository, AccountRepository>();
                services.AddScoped<ITransactionRepository, TransactionRepository>();
                services.AddScoped<IClientService, ClientService>();
                services.AddScoped<IAdminservices, AdminServices>();

                services.AddScoped<AdminView>(provider =>
                {
                    var adminServices = provider.GetRequiredService<IAdminservices>();
                    var authService = provider.GetRequiredService<IAuthService>();
                    return new AdminView(adminServices, authService, Console.ReadLine);
                });

                services.AddScoped<ClientView>(provider =>
                {
                    var clientService = provider.GetRequiredService<IClientService>();
                    var authService = provider.GetRequiredService<IAuthService>();
                    return new ClientView(clientService, authService, Console.ReadLine);
                });

                services.AddValidatorsFromAssemblyContaining<AddNewuserValidator>();
                services.AddScoped<IValidator<CreateUserDto>, AddNewuserValidator>();
            });

    public static async Task RunAsync(Func<string> input, TextWriter output, IServiceProvider serviceProvider)
    {
        var authService = serviceProvider.GetRequiredService<IAuthService>();
        var adminView = serviceProvider.GetRequiredService<AdminView>();
        var clientView = serviceProvider.GetRequiredService<ClientView>();

        await output.WriteLineAsync("Welcome to our ATM system!");

        while (true)
        {
            await output.WriteAsync("Enter your login: ");
            string login = input();

            await output.WriteAsync("Enter your pincode: ");
            string pinCode = input();

            var userLoginDto = new UserLoginDTO
            {
                Login = login,
                PinCode = pinCode,
            };

            User isAuthenticated = await authService.Login(userLoginDto);

            if (isAuthenticated == null)
            {
                await output.WriteLineAsync("Invalid credentials. Please try again.");
                continue;
            }

            if (isAuthenticated.Role == UserRole.Admin)
            {
                await output.WriteLineAsync($"Welcome Admin {isAuthenticated.HolderName}!");
                await adminView.Show();
            }
            else if (isAuthenticated.Role == UserRole.Client)
            {
                await output.WriteLineAsync($"Welcome Client {isAuthenticated.HolderName}!");
                await clientView.Show(isAuthenticated);
            }

            await output.WriteLineAsync("Returning to login screen...\n");
        }
    }
}
