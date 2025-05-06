// <copyright file="ATMContextTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using ATMApp.Data;
using ATMApp.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ATMContextTests
{
    private DbContextOptions<ATMContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task CanInsertUserWithAccount()
    {
        var options = this.GetInMemoryOptions();

        using (var context = new ATMContext(options))
        {
            var user = new User { HolderName = "Test User", Login = "test", PinCode = "1234", Role = UserRole.Client };
            var account = new Account { IntialBalance = 500, Status = AccountStatus.Active, User = user };

            context.User.Add(user);
            context.Account.Add(account);
            await context.SaveChangesAsync();
        }

        using (var context = new ATMContext(options))
        {
            var user = await context.User.Include(u => u.Account).FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.NotNull(user.Account);
            Assert.Equal("Test User", user.HolderName);
        }
    }

    [Fact]
    public async Task DeletingUser_CascadesToAccountAndTransactions()
    {
        var options = this.GetInMemoryOptions();

        using (var context = new ATMContext(options))
        {
            var user = new User { HolderName = "Cascade User", Login = "cascade", PinCode = "0000", Role = UserRole.Client };
            var account = new Account { IntialBalance = 1000, Status = AccountStatus.Active, User = user };
            var transaction = new Transaction { Account = account, Amount = 100, Type = TransactionType.Deposit };

            context.User.Add(user);
            context.Account.Add(account);
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        }

        using (var context = new ATMContext(options))
        {
            var user = await context.User.Include(u => u.Account).ThenInclude(a => a.Transactions).FirstOrDefaultAsync();
            context.User.Remove(user);
            await context.SaveChangesAsync();
        }

        using (var context = new ATMContext(options))
        {
            Assert.False(context.User.Any());
            Assert.False(context.Account.Any());
            Assert.False(context.Transactions.Any());
        }
    }
}
