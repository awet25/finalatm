// <copyright file="UserRepositoryTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using ATMApp.Data;
using ATMApp.Models;
using ATMApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class UserRepositoryTests
{
    private ATMContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB every time
            .Options;
        return new ATMContext(options);
    }

    [Fact]
    public async Task AddUser_ShouldAddUserToDatabase()
    {
        var context = this.GetDbContext();
        var repo = new UserRepository(context);
        var user = new User { Login = "alice", HolderName = "Alice", PinCode = "12345" };

        var result = await repo.AddUser(user);

        Assert.NotNull(result);
        Assert.Equal("alice", result.Login);
    }

    [Fact]
    public async Task GetUserByLogin_ShouldReturnCorrectUser()
    {
        var context = this.GetDbContext();
        context.User.Add(new User { Login = "bob", HolderName = "Bob", PinCode = "54321" });
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);
        var result = await repo.GetUserBylogin("bob");

        Assert.NotNull(result);
        Assert.Equal("Bob", result.HolderName);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser()
    {
        var context = this.GetDbContext();
        var user = new User { Login = "carl", HolderName = "Carl", PinCode = "11111" };
        context.User.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);
        var result = await repo.GetUserById(user.Id);

        Assert.NotNull(result);
        Assert.Equal("carl", result.Login);
    }

    [Fact]
    public async Task UpdateUser_ShouldChangeData()
    {
        var context = this.GetDbContext();
        var user = new User { Login = "dave", HolderName = "Old", PinCode = "22222" };
        context.User.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);
        user.HolderName = "New";
        var success = await repo.UpdateUser(user);
        var updated = await repo.GetUserById(user.Id);

        Assert.True(success);
        Assert.Equal("New", updated.HolderName);
    }

    [Fact]
    public async Task DeleteUserById_ShouldRemoveUser()
    {
        var context = this.GetDbContext();
        var user = new User { Login = "eve", HolderName = "Eve", PinCode = "33333" };
        context.User.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);
        var success = await repo.DeleteUserbyId(user.Id);

        Assert.True(success);
        Assert.Null(await repo.GetUserById(user.Id));
    }

    [Fact]
    public async Task GetUserWithAccountByLogin_ShouldReturnUserWithAccount()
    {
        var context = this.GetDbContext();
        var user = new User { Login = "frank", HolderName = "Frank", PinCode = "44444" };
        var account = new Account { ClientID = user.Id, IntialBalance = 1000, User = user };
        user.Account = account;
        context.User.Add(user);
        context.Account.Add(account);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);
        var result = await repo.GetUserWithAccountByLogin("frank");

        Assert.NotNull(result);
        Assert.NotNull(result.Account);
        Assert.Equal(1000, result.Account.IntialBalance);
    }

    [Fact]
    public async Task GetUserByLogin_ShouldReturnUser_WhenUserExists()
    {
        var context = this.GetDbContext();
        var user = new User { Login = "bob", HolderName = "Bob", PinCode = "54321" };
        context.User.Add(user);
        await context.SaveChangesAsync();

        var repo = new UserRepository(context);

        var result = await repo.GetUserBylogin("bob");

        Assert.NotNull(result);
        Assert.Equal("bob", result.Login);
    }
}
