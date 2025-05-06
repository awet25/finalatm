// <copyright file="AdminServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IO;
using ATMApp.Data;
using ATMApp.DTOs;
using ATMApp.Interfaces;
using ATMApp.Models;
using ATMApp.Repositories;
using ATMApp.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;

public class AdminServiceTests
{
    private DbContextOptions<ATMContext> GetInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task AddUser_ValidUser_ReturnsTrue()
    {
        var userDto = new CreateUserDto
        {
            HolderName = "Alice",
            Login = "alice123",
            PinCode = "1234",
            Role = UserRole.Client,
            IntialBalance = 10,
            Status = AccountStatus.Active,
        };

        var mockValidator = new Mock<IValidator<CreateUserDto>>();
        mockValidator.Setup(v => v.Validate(userDto)).Returns(new ValidationResult());

        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(r => r.GetUserBylogin(userDto.Login)).ReturnsAsync((User)null);
        mockUserRepo.Setup(r => r.AddUser(It.IsAny<User>())).ReturnsAsync(new User
        {
            Id = 1,
            Login = userDto.Login,
            Role = UserRole.Client,
        });
        var mockAccountRepo = new Mock<IAccountRepository>();
        mockAccountRepo.Setup(r => r.CreateAccount(It.IsAny<Account>()))
                       .ReturnsAsync(new Account { Id = 100, ClientID = 1 });
        var adminService = new AdminServices(
            null,
            mockUserRepo.Object,
            Mock.Of<ITransactionRepository>(),
            mockAccountRepo.Object,
            mockValidator.Object);

        var result = await adminService.AddUser(userDto);

        Assert.True(result);
        mockUserRepo.Verify(r => r.AddUser(It.Is<User>(u => u.Login == userDto.Login)), Times.Once);
        mockAccountRepo.Verify(r => r.CreateAccount(It.Is<Account>(a => a.ClientID == 1)), Times.Once);
    }

    [Fact]
    public async Task AddUser_InvalidDto_ReturnsFalse()
    {
        var userDto = new CreateUserDto
        {
            HolderName = string.Empty,
            Login = string.Empty,
            PinCode = string.Empty,
            Role = UserRole.Client,
            IntialBalance = 0,
        };

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Login", "Login required"),
            new ValidationFailure("PinCode", "PIN required"),
        };

        var mockValidator = new Mock<IValidator<CreateUserDto>>();
        mockValidator.Setup(v => v.Validate(userDto)).Returns(new ValidationResult(failures));

        var adminService = new AdminServices(
            null,
            Mock.Of<IUserRepository>(),
            Mock.Of<ITransactionRepository>(),
            Mock.Of<IAccountRepository>(),
            mockValidator.Object);

        var result = await adminService.AddUser(userDto);

        Assert.False(result);
    }

    [Fact]
    public async Task AddUser_UserAlreadyExists_ReturnsFalse()
    {
        var userDto = new CreateUserDto
        {
            HolderName = "Bob",
            Login = "bob123",
            PinCode = "5678",
            Role = UserRole.Client,
            IntialBalance = 0,
        };

        var mockValidator = new Mock<IValidator<CreateUserDto>>();
        mockValidator.Setup(v => v.Validate(userDto)).Returns(new ValidationResult());

        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(r => r.GetUserBylogin(userDto.Login)).ReturnsAsync(new User { Login = "bob123" });

        var adminService = new AdminServices(
            null,
            mockUserRepo.Object,
            Mock.Of<ITransactionRepository>(),
            Mock.Of<IAccountRepository>(),
            mockValidator.Object);

        var result = await adminService.AddUser(userDto);

        Assert.False(result);
        mockUserRepo.Verify(r => r.AddUser(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task AddUser_AdminUser_ReturnsTrue()
    {
        var userDto = new CreateUserDto
        {
            HolderName = "Admin Guy",
            Login = "admin01",
            PinCode = "admin123",
            Role = UserRole.Admin,
            IntialBalance = 0, // irrelevant
            Status = AccountStatus.Active,
        };

        var mockValidator = new Mock<IValidator<CreateUserDto>>();
        mockValidator.Setup(v => v.Validate(userDto)).Returns(new ValidationResult());

        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(r => r.GetUserBylogin(userDto.Login)).ReturnsAsync((User)null);
        mockUserRepo.Setup(r => r.AddUser(It.IsAny<User>())).ReturnsAsync(new User
        {
            Id = 2,
            Login = userDto.Login,
            Role = UserRole.Admin,
        });

        var mockAccountRepo = new Mock<IAccountRepository>();

        var adminService = new AdminServices(
            null,
            mockUserRepo.Object,
            Mock.Of<ITransactionRepository>(),
            mockAccountRepo.Object,
            mockValidator.Object);

        var result = await adminService.AddUser(userDto);

        Assert.True(result);
        mockUserRepo.Verify(r => r.AddUser(It.Is<User>(u => u.Login == userDto.Login)), Times.Once);
        mockAccountRepo.Verify(r => r.CreateAccount(It.IsAny<Account>()), Times.Never); // ✅ no account creation
    }

    [Fact]
    public async Task DeleteUserAndAccount_ValidAccountAndConfirmation_ReturnsTrue()
    {
        // Arrange
        int accountId = 100;
        int clientId = 10;

        var account = new Account { Id = accountId, ClientID = clientId, IntialBalance = 0, Status = AccountStatus.Active };
        var user = new User { Id = clientId, HolderName = "Alice", Login = "Alice", PinCode = "12345", Role = UserRole.Client };

        // Simulate user typing the account number to confirm
        Console.SetIn(new StringReader(accountId.ToString()));

        // Use real in-memory context
        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase("TestDb_Delete")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new ATMContext(options);

        // Set up repos
        var mockUserRepo = new Mock<IUserRepository>();
        var mockAccountRepo = new Mock<IAccountRepository>();
        var mockTransactionRepo = new Mock<ITransactionRepository>();
        mockTransactionRepo.Setup(r => r.GetTransactionsByAccountId(accountId))
                           .ReturnsAsync(new List<Transaction>());
        mockAccountRepo.Setup(r => r.GetAccountById(accountId)).ReturnsAsync(account);
        mockUserRepo.Setup(r => r.GetUserById(clientId)).ReturnsAsync(user);
        mockAccountRepo.Setup(r => r.DeleteAccountById(accountId)).ReturnsAsync(true);
        mockUserRepo.Setup(r => r.DeleteUserbyId(clientId)).ReturnsAsync(true);

        var service = new AdminServices(
            context,
            mockUserRepo.Object,
            mockTransactionRepo.Object,
            mockAccountRepo.Object,
            Mock.Of<IValidator<CreateUserDto>>());

        // Act
        var result = await service.DeleteUserAndAccount(accountId, accountId.ToString());

        // Debug trace
        Console.WriteLine($"Expected accountId: {accountId}, UserId: {clientId}");
        Console.WriteLine($"DeleteUser returned: {await mockUserRepo.Object.DeleteUserbyId(clientId)}");
        Console.WriteLine($"DeleteAccount returned: {await mockAccountRepo.Object.DeleteAccountById(accountId)}");
        Console.WriteLine($"Final result: {result}");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateUser_ValidDto_ReturnsTrue()
    {
        // Arrange
        var accountId = 100;
        var clientId = 10;

        var account = new Account { Id = accountId, ClientID = clientId };
        var originalUser = new User
        {
            Id = clientId,
            Login = "olduser",
            HolderName = "Old Name",
            PinCode = "1111",
        };

        var dto = new UpdateUserDto
        {
            Id = accountId, // ← must match account ID
            Login = "newuser",
            HolderName = "New Name",
            PinCode = "12345",
        };

        var mockUserRepo = new Mock<IUserRepository>();
        var mockAccountRepo = new Mock<IAccountRepository>();

        mockAccountRepo.Setup(r => r.GetAccountById(accountId)).ReturnsAsync(account);
        mockUserRepo.Setup(r => r.GetUserById(clientId)).ReturnsAsync(originalUser);
        mockUserRepo.Setup(r => r.UpdateUser(It.IsAny<User>())).ReturnsAsync(true);
        mockAccountRepo.Setup(r => r.UpdateAccount(It.IsAny<Account>())).ReturnsAsync(account);

        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase("TestDb_UpdateUser")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var context = new ATMContext(options);

        var service = new AdminServices(
            context, mockUserRepo.Object,
            Mock.Of<ITransactionRepository>(), mockAccountRepo.Object,
            Mock.Of<IValidator<CreateUserDto>>());

        // Act
        var result = await service.UpdateUser(dto);

        // Assert
        Assert.True(result);
        mockUserRepo.Verify(
            r => r.UpdateUser(It.Is<User>(u =>
            u.Login == "newuser" &&
            u.HolderName == "New Name" &&
            u.PinCode == "12345")), Times.Once);
    }

    [Fact]
    public async Task GetUserByLogin_ValidLogin_ReturnsUser()
    {
        // Arrange
        var login = "alice";
        var expectedUser = new User
        {
            Id = 10,
            Login = login,
            HolderName = "Alice",
            PinCode = "12345",
            Role = UserRole.Client,
        };
        var options = new DbContextOptionsBuilder<ATMContext>()
             .UseInMemoryDatabase("TestDb_Delete")
             .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
             .Options;

        var context = new ATMContext(options);
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(r => r.GetUserWithAccountByLogin(login)).ReturnsAsync(expectedUser);

        var service = new AdminServices(
            context, mockUserRepo.Object,
            Mock.Of<ITransactionRepository>(), Mock.Of<IAccountRepository>(),
            Mock.Of<IValidator<CreateUserDto>>());

        // Act
        var result = await service.GetUserByLogin(login);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Login, result.Login);
    }

    [Fact]
    public async Task GetAccount_ValidId_ReturnsAccount()
    {
        // Arrange
        var accountId = 100;
        var user = new User { Id = 10, HolderName = "Alice", Login = "alice", PinCode = "12345" };
        var account = new Account
        {
            Id = accountId,
            ClientID = user.Id,
            IntialBalance = 500,
            Status = AccountStatus.Active,
            User = user,
        };

        var options = new DbContextOptionsBuilder<ATMContext>()
            .UseInMemoryDatabase("TestDb_GetAccount")
            .Options;

        using var context = new ATMContext(options);
        context.User.Add(user);
        context.Account.Add(account);
        await context.SaveChangesAsync();

        var service = new AdminServices(
            context,
            Mock.Of<IUserRepository>(),
            Mock.Of<ITransactionRepository>(),
            Mock.Of<IAccountRepository>(),
            Mock.Of<IValidator<CreateUserDto>>());

        // Act
        var result = await service.GetAccount(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(accountId, result.Id);
        Assert.Equal(user.Login, result.User.Login);
    }

    [Fact]
    public async Task GetUserById_NonExistent_ReturnsNull()
    {
        var options = this.GetInMemoryOptions("GetUserById_NonExistent");
        using var context = new ATMContext(options);
        var repo = new UserRepository(context);

        var result = await repo.GetUserById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUser_NonExistent_ReturnsFalse()
    {
        var options = this.GetInMemoryOptions("DeleteUser_NonExistent");
        using var context = new ATMContext(options);
        var repo = new UserRepository(context);

        var result = await repo.DeleteUserbyId(999);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUser_NonExistent_ReturnsFalse()
    {
        var options = this.GetInMemoryOptions("UpdateUser_NonExistent");
        using var context = new ATMContext(options);
        var repo = new UserRepository(context);

        var user = new User { Id = 999, Login = "ghost", HolderName = "Ghost", PinCode = "99999" };

        var result = await repo.UpdateUser(user);

        Assert.False(result);
    }

    [Fact]
    public async Task AddUser_ShouldReturnFalse_WhenCreationFails()
    {
        var options = this.GetInMemoryOptions("AddUserFailsTestDb");
        using var context = new ATMContext(options);

        var mockValidator = new Mock<IValidator<CreateUserDto>>();
        mockValidator.Setup(v => v.Validate(It.IsAny<CreateUserDto>())).Returns(new ValidationResult());

        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserBylogin(It.IsAny<string>())).ReturnsAsync((User)null);
        mockRepo.Setup(r => r.AddUser(It.IsAny<User>())).ReturnsAsync((User)null);

        var adminService = new AdminServices(
            context,
            mockRepo.Object,
            Mock.Of<ITransactionRepository>(),
            Mock.Of<IAccountRepository>(),
            mockValidator.Object);

        var userDto = new CreateUserDto
        {
            Login = "failuser",
            HolderName = "Fail",
            PinCode = "00000",
            Role = UserRole.Client,
        };

        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            var result = await adminService.AddUser(userDto);
            var output = sw.ToString();

            Assert.False(result);
            Assert.Contains("Failed to create User", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
