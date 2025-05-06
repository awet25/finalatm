// <copyright file="AdminViewTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ATMApp.DTOs;
using ATMApp.Interfaces;
using ATMApp.Models;
using ATMApp.Views;
using Moq;
using Xunit;

public class AdminViewTests
{
    [Fact]
    public async Task CreateUser_CallsAddUser_ReturnsTrue()
    {
        // Arrange
        var mockAdminServices = new Mock<IAdminservices>();
        var mockAuthService = new Mock<IAuthService>();
        var inputQueue = new Queue<string>();

        var view = new AdminView(mockAdminServices.Object, mockAuthService.Object, () => inputQueue.Dequeue());

        var dummyDto = new CreateUserDto();
        mockAdminServices.Setup(s => s.AddUser(It.IsAny<CreateUserDto>()))
                         .ReturnsAsync(true);

        // Act
        var result = await view.CreateUser();

        // Assert
        Assert.True(result);
        mockAdminServices.Verify(s => s.AddUser(It.IsAny<CreateUserDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_WithValidConfirmation_ReturnsTrue()
    {
        // Arrange
        var mockAdminServices = new Mock<IAdminservices>();
        var mockAuthService = new Mock<IAuthService>();
        var inputQueue = new Queue<string>(new[] { "123", "123" });

        var view = new AdminView(mockAdminServices.Object, mockAuthService.Object, () => inputQueue.Dequeue());

        mockAdminServices.Setup(s => s.DeleteUserAndAccount(123, "123")).ReturnsAsync(true);

        // Act
        var result = await view.DeleteAccount();

        // Assert
        Assert.True(result);
        mockAdminServices.Verify(s => s.DeleteUserAndAccount(123, "123"), Times.Once);
    }

    [Fact]
    public async Task UpdateAccount_CallsUpdateUser_ReturnsTrue()
    {
        // Arrange
        var mockAdminServices = new Mock<IAdminservices>();
        var mockAuthService = new Mock<IAuthService>();
        var view = new AdminView(mockAdminServices.Object, mockAuthService.Object, () => "ignored");

        mockAdminServices.Setup(s => s.UpdateUser(It.IsAny<UpdateUserDto>()))
                         .ReturnsAsync(true);

        // Act
        var result = await view.UpdateAccount();

        // Assert
        Assert.True(result);
        mockAdminServices.Verify(s => s.UpdateUser(It.IsAny<UpdateUserDto>()), Times.Once);
    }

    [Fact]
    public async Task SearchForAccount_ValidId_PrintsAccountInfo()
    {
        // Arrange
        var mockAdminServices = new Mock<IAdminservices>();
        var mockAuthService = new Mock<IAuthService>();
        var inputQueue = new Queue<string>(new[] { "5" });

        var account = new Account
        {
            Id = 5,
            IntialBalance = 1000,
            Status = AccountStatus.Active,
            User = new User { HolderName = "John", Login = "john123", PinCode = "1234" },
        };

        mockAdminServices.Setup(s => s.GetAccount(5)).ReturnsAsync(account);

        var view = new AdminView(mockAdminServices.Object, mockAuthService.Object, () => inputQueue.Dequeue());

        // Act
        await view.SearchForAccount();

        // Assert
        mockAdminServices.Verify(s => s.GetAccount(5), Times.Once);
    }

    [Fact]
    public void Exit_CallsAuthServiceExit()
    {
        // Arrange
        var mockAdminServices = new Mock<IAdminservices>();
        var mockAuthService = new Mock<IAuthService>();
        var view = new AdminView(mockAdminServices.Object, mockAuthService.Object, () => "ignored");

        // Act
        view.Exit();

        // Assert
        mockAuthService.Verify(s => s.Exit(), Times.Once);
    }

    [Fact]
    public async Task Show_WithValidChoices_CallsExpectedMethods()
    {
        // Arrange
        var mockAdminServices = new Mock<IAdminservices>();
        var mockAuthService = new Mock<IAuthService>();

        // Mock user actions: 1 (Create), 2 (Delete), 3 (Update), 4 (Search), 5 (Exit)
        var inputQueue = new Queue<string>(new[]
        {
            "1",  // Create
            "2",  // Delete
            "123", // Account Id
            "123", // Confirm deletion
            "3",  // Update
            "4",  // Search
            "999", // Account ID
            "5",   // Exit
        });

        Func<string> input = () => inputQueue.Dequeue();

        mockAdminServices.Setup(s => s.AddUser(It.IsAny<CreateUserDto>())).ReturnsAsync(true);
        mockAdminServices.Setup(s => s.DeleteUserAndAccount(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(true);
        mockAdminServices.Setup(s => s.UpdateUser(It.IsAny<UpdateUserDto>())).ReturnsAsync(true);
        mockAdminServices.Setup(s => s.GetAccount(It.IsAny<int>()))
            .ReturnsAsync(new Account
            {
                Id = 999,
                IntialBalance = 1000,
                Status = AccountStatus.Active,
                User = new User
                {
                    HolderName = "Test User",
                    Login = "testuser",
                    PinCode = "1234",
                },
            });

        var view = new AdminView(mockAdminServices.Object, mockAuthService.Object, input);

        // Act
        await view.Show();

        // Assert: All expected service methods should have been called
        mockAdminServices.Verify(s => s.AddUser(It.IsAny<CreateUserDto>()), Times.Once);
        mockAdminServices.Verify(s => s.DeleteUserAndAccount(123, "123"), Times.Once);
        mockAdminServices.Verify(s => s.UpdateUser(It.IsAny<UpdateUserDto>()), Times.Once);
        mockAdminServices.Verify(s => s.GetAccount(999), Times.Once);
        mockAuthService.Verify(s => s.Exit(), Times.Once);
    }

    // [Theory]
    // [InlineData("yes", true)]
    // [InlineData("y", true)]
    // [InlineData("no", false)]
    // [InlineData("n", false)]
    // [InlineData("YES", true)]
    // [InlineData(" YeS ", true)]
    // [InlineData("   no   ", false)]
    // public void AskUserToEdit_ReturnsExpectedResult(string input, bool expected)
    // {
    //     // Arrange
    //     var inputReader = new StringReader(input);
    //     var outputWriter = new StringWriter();

    // var originalIn = Console.In;
    //     var originalOut = Console.Out;

    // Console.SetIn(inputReader);
    //     Console.SetOut(outputWriter);

    // try
    //     {
    //         // Act
    //         bool result = AdminView.AskUserToEdit("field");

    // // Assert
    //         Assert.Equal(expected, result);
    //     }
    //     finally
    //     {
    //         // Reset to avoid side effects across tests
    //         Console.SetIn(originalIn);
    //         Console.SetOut(originalOut);

    // inputReader.Dispose();
    //         outputWriter.Dispose();
    //     }
    // }
}
