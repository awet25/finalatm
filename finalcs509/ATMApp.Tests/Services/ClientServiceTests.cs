// <copyright file="ClientServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using ATMApp.Interfaces;
using ATMApp.Models;
using ATMApp.Services;
using Moq;
using Xunit;

public class ClientServiceTests
{
    [Fact]
    public async Task Deposit_ValidAmount_ReturnsTrueAndUpdatesBalance()
    {
        var clientId = 1;
        var depositAmount = 100m;
        var account = new Account
        {
            Id = 10,
            ClientID = clientId,
            IntialBalance = 500m,
            Status = AccountStatus.Active,
        };
        var mockAccountRepo = new Mock<IAccountRepository>();
        mockAccountRepo.Setup(r => r.GetAccountByClientID(clientId)).ReturnsAsync(account);
        mockAccountRepo.Setup(r => r.UpdateAccount(account)).ReturnsAsync(account);
        var mockTransactionRepo = new Mock<ITransactionRepository>();
        mockTransactionRepo.Setup(r => r.AddTransaction(It.IsAny<Transaction>()))
                           .Returns(Task.CompletedTask);

        var service = new ClientService(mockAccountRepo.Object, mockTransactionRepo.Object);

        var result = await service.Deposit(clientId, depositAmount);

        Assert.True(result);
        Assert.Equal(600m, account.IntialBalance);
        mockAccountRepo.Verify(r => r.UpdateAccount(account), Times.Once);
        mockTransactionRepo.Verify(r => r.AddTransaction(It.IsAny<Transaction>()), Times.Once);
    }

    [Fact]
    public async Task Deposit_NegativeAmount_ReturnsFalse()
    {
        var service = new ClientService(Mock.Of<IAccountRepository>(), Mock.Of<ITransactionRepository>());
        var result = await service.Deposit(1, -50m);
        Assert.False(result);
    }

    [Fact]
    public async Task Deposit_AccountNotFound_ReturnsFalse()
    {
        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(r => r.GetAccountByClientID(It.IsAny<int>())).ReturnsAsync((Account)null);

        var service = new ClientService(mockRepo.Object, Mock.Of<ITransactionRepository>());
        var result = await service.Deposit(1, 100m);
        Assert.False(result);
    }

    [Fact]
    public async Task Deposit_AccountDisabled_ReturnsFalse()
    {
        var account = new Account
        {
            Id = 10,
            ClientID = 1,
            IntialBalance = 300,
            Status = AccountStatus.Disabled,
        };

        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(r => r.GetAccountByClientID(It.IsAny<int>())).ReturnsAsync(account);

        var service = new ClientService(mockRepo.Object, Mock.Of<ITransactionRepository>());
        var result = await service.Deposit(1, 100m);
        Assert.False(result);
    }

    [Fact]
    public async Task Withdraw_ValidAmount_ReturnsTrueAndDeductsBalance()
    {
        var clientId = 1;
        var withdrawAmount = 100m;

        var account = new Account
        {
            Id = 10,
            ClientID = clientId,
            IntialBalance = 200m,
            Status = AccountStatus.Active,
        };

        var mockAccountRepo = new Mock<IAccountRepository>();
        mockAccountRepo.Setup(r => r.GetAccountByClientID(clientId)).ReturnsAsync(account);
        mockAccountRepo.Setup(r => r.UpdateAccount(account)).ReturnsAsync(account);

        var mockTransactionRepo = new Mock<ITransactionRepository>();
        mockTransactionRepo.Setup(r => r.AddTransaction(It.IsAny<Transaction>())).Returns(Task.CompletedTask);

        var service = new ClientService(mockAccountRepo.Object, mockTransactionRepo.Object);

        var result = await service.Withdraw(clientId, withdrawAmount);

        Assert.True(result);
        Assert.Equal(100m, account.IntialBalance);
    }

    [Fact]
    public async Task Withdraw_AccountNotFound_ReturnsFalse()
    {
        var mockAccountRepo = new Mock<IAccountRepository>();
        mockAccountRepo.Setup(r => r.GetAccountByClientID(It.IsAny<int>())).ReturnsAsync((Account)null);

        var service = new ClientService(mockAccountRepo.Object, Mock.Of<ITransactionRepository>());

        var result = await service.Withdraw(1, 50);
        Assert.False(result);
    }

    [Fact]
    public async Task Withdraw_AccountDisabled_ReturnsFalse()
    {
        var account = new Account
        {
            Id = 5,
            ClientID = 1,
            IntialBalance = 100,
            Status = AccountStatus.Disabled,
        };

        var mockAccountRepo = new Mock<IAccountRepository>();
        mockAccountRepo.Setup(r => r.GetAccountByClientID(1)).ReturnsAsync(account);

        var service = new ClientService(mockAccountRepo.Object, Mock.Of<ITransactionRepository>());

        var result = await service.Withdraw(1, 50);
        Assert.False(result);
    }

    [Fact]
    public async Task Withdraw_InsufficientBalance_ReturnsFalse()
    {
        var account = new Account
        {
            Id = 5,
            ClientID = 1,
            IntialBalance = 40,
            Status = AccountStatus.Active,
        };

        var mockAccountRepo = new Mock<IAccountRepository>();
        mockAccountRepo.Setup(r => r.GetAccountByClientID(1)).ReturnsAsync(account);

        var service = new ClientService(mockAccountRepo.Object, Mock.Of<ITransactionRepository>());

        var result = await service.Withdraw(1, 100);
        Assert.False(result);
    }

    [Fact]
    public async Task GetBalance_ValidAccount_PrintsBalance()
    {
        var account = new Account
        {
            Id = 1,
            ClientID = 10,
            IntialBalance = 300m,
            Status = AccountStatus.Active,
        };

        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(r => r.GetAccountByClientID(10)).ReturnsAsync(account);

        var service = new ClientService(mockRepo.Object, Mock.Of<ITransactionRepository>());

        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        await service.GetBalance(10);
        Console.SetOut(originalOut);
        var output = sw.ToString();
        Assert.Contains("Account Info", output);
        Assert.Contains("Account #1", output);
        Assert.Contains($"Balance :{account.IntialBalance:F2}", output);
    }

    [Fact]
    public async Task GetBalance_AccountNotFound_PrintsNotFound()
    {
        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(r => r.GetAccountByClientID(It.IsAny<int>())).ReturnsAsync((Account)null);

        var service = new ClientService(mockRepo.Object, Mock.Of<ITransactionRepository>());
        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        await service.GetBalance(99);
        Console.SetOut(originalOut);
        var output = sw.ToString();
        Assert.Contains("Account not found.", output);
    }

    [Fact]
    public async Task GetBalance_AccountDisabled_PrintsDisabledMessage()
    {
        var account = new Account
        {
            Id = 2,
            ClientID = 5,
            IntialBalance = 100,
            Status = AccountStatus.Disabled,
        };

        var mockRepo = new Mock<IAccountRepository>();
        mockRepo.Setup(r => r.GetAccountByClientID(5)).ReturnsAsync(account);

        var service = new ClientService(mockRepo.Object, Mock.Of<ITransactionRepository>());
        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        await service.GetBalance(5);

        Console.SetOut(originalOut);
        var output = sw.ToString();
        Assert.Contains("Sorry this Account was Disabled", output);
    }

    [Fact]
    public async Task GetTransactionHistory_ReturnsTransactionList()
    {
        var accountId = 10;
        var mockTransactions = new List<Transaction>
    {
        new Transaction { Id = 1, AccountId = accountId, Amount = 100, Type = TransactionType.Deposit },
        new Transaction { Id = 2, AccountId = accountId, Amount = 50, Type = TransactionType.Withdrawal },
    };

        var mockAccountRepo = new Mock<IAccountRepository>(); // not used in this method
        var mockTransactionRepo = new Mock<ITransactionRepository>();
        mockTransactionRepo.Setup(r => r.GetTransactionsByAccountId(accountId)).ReturnsAsync(mockTransactions);

        var service = new ClientService(mockAccountRepo.Object, mockTransactionRepo.Object);

        var result = await service.GetTransactionHistory(accountId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Type == TransactionType.Deposit);
        Assert.Contains(result, t => t.Type == TransactionType.Withdrawal);
    }
}
