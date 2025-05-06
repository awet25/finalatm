// <copyright file="AccountRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq;
using System.Threading.Tasks;
using ATMApp.Data;
using ATMApp.Models;
using ATMApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class AccountRepositoryTests
{
    private async Task<(ATMContext, AccountRepository)> GetInMemoryRepo()
    {
        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ATMContext(options);
        var repo = new AccountRepository(context);

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        return (context, repo);
    }

    [Fact]
    public async Task CreateAccount_AddsAccount()
    {
        var (context, repo) = await this.GetInMemoryRepo();
        var account = new Account { ClientID = 1, IntialBalance = 500m, Status = AccountStatus.Active };

        var result = await repo.CreateAccount(account);

        Assert.NotNull(result);
        Assert.Equal(1, context.Account.Count());
        Assert.Equal(account.ClientID, result.ClientID);
    }

    [Fact]
    public async Task GetAccountByClientID_ReturnsAccount()
    {
        var (context, repo) = await this.GetInMemoryRepo();
        var account = new Account { ClientID = 2, IntialBalance = 1000m };
        await context.Account.AddAsync(account);
        await context.SaveChangesAsync();

        var result = await repo.GetAccountByClientID(2);

        Assert.NotNull(result);
        Assert.Equal(1000m, result.IntialBalance);
    }

    [Fact]
    public async Task GetAccountById_ReturnsCorrectAccount()
    {
        var (context, repo) = await this.GetInMemoryRepo();
        var account = new Account { Id = 3, ClientID = 3, IntialBalance = 300m };
        await context.Account.AddAsync(account);
        await context.SaveChangesAsync();

        var result = await repo.GetAccountById(3);

        Assert.NotNull(result);
        Assert.Equal(3, result.ClientID);
    }

    [Fact]
    public async Task UpdateAccount_ModifiesValues()
    {
        var (context, repo) = await this.GetInMemoryRepo();
        var account = new Account { Id = 4, ClientID = 4, IntialBalance = 400m };
        await context.Account.AddAsync(account);
        await context.SaveChangesAsync();

        account.IntialBalance = 900m;
        var result = await repo.UpdateAccount(account);

        Assert.NotNull(result);
        Assert.Equal(900m, result.IntialBalance);
    }

    [Fact]
    public async Task DeleteAccountById_RemovesAccount()
    {
        var (context, repo) = await this.GetInMemoryRepo();
        var account = new Account { Id = 5, ClientID = 5, IntialBalance = 750m };
        await context.Account.AddAsync(account);
        await context.SaveChangesAsync();

        var result = await repo.DeleteAccountById(5);

        Assert.True(result);
        Assert.Equal(0, context.Account.Count());
    }
}
