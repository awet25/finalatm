// <copyright file="TransactionRepoTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using ATMApp.Data;
using ATMApp.Models;
using ATMApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TransactionRepositoryTests
{
    [Fact]
    public async Task AddTransaction_SavesToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase("TestDb_AddTransaction")
            .Options;

        using var context = new ATMContext(options);
        var repository = new TransactionRepository(context);

        var transaction = new Transaction
        {
            Id = 1,
            AccountId = 100,
            Amount = 200,
            Type = TransactionType.Deposit,
            TimeStamp = DateTime.UtcNow,
        };

        // Act
        await repository.AddTransaction(transaction);

        // Assert
        var result = await context.Transactions.FirstOrDefaultAsync(t => t.Id == 1);
        Assert.NotNull(result);
        Assert.Equal(200, result.Amount);
        Assert.Equal(TransactionType.Deposit, result.Type);
    }

    [Fact]
    public async Task GetTransactionsByAccountId_ReturnsCorrectTransactions()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase("TestDb_GetTransactions")
            .Options;

        using var context = new ATMContext(options);
        var repository = new TransactionRepository(context);

        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, AccountId = 101, Amount = 100, Type = TransactionType.Deposit },
            new Transaction { Id = 2, AccountId = 101, Amount = 50, Type = TransactionType.Withdrawal },
            new Transaction { Id = 3, AccountId = 102, Amount = 300, Type = TransactionType.Deposit },
        };

        context.Transactions.AddRange(transactions);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetTransactionsByAccountId(101);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(101, t.AccountId));
    }
}
