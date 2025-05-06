// <copyright file="ClientViewTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Threading.Tasks;
using ATMApp.Interfaces;
using ATMApp.Models;
using ATMApp.Views;
using Moq;
using Xunit;

public class ClientViewTest
{
    [Fact]
    public async Task Show_Should_InvokeWithdrawMoney()
    {
        // Arrange
        var mockClient = new Mock<IClientService>();
        var mockAuth = new Mock<IAuthService>();
        var inputLines = new Queue<string>(["1", "100", "4"]); // Withdraw then Exit
        var view = new ClientView(mockClient.Object, mockAuth.Object, () => inputLines.Dequeue());

        var user = new User { Id = 1 };
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            await view.Show(user);

            // Assert
            mockClient.Verify(s => s.Withdraw(1, 100), Times.Once);
            Assert.Contains("you have choosen to Withdraw Money", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            output.Dispose();
        }
    }

    [Fact]
    public async Task Show_Should_InvokeDeposite()
    {
        // Arrange
        var mockClient = new Mock<IClientService>();
        var mockAuth = new Mock<IAuthService>();
        var inputLines = new Queue<string>(["2", "200", "4"]); // Deposit then Exit
        var view = new ClientView(mockClient.Object, mockAuth.Object, () => inputLines.Dequeue());

        var user = new User { Id = 1 };
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            await view.Show(user);

            // Assert
            mockClient.Verify(s => s.Deposit(1, 200), Times.Once);
            Assert.Contains("you have choose to Deposite Money", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            output.Dispose();
        }
    }

    [Fact]
    public async Task Show_Should_InvokeDisplayAccount()
    {
        // Arrange
        var mockClient = new Mock<IClientService>();
        var mockAuth = new Mock<IAuthService>();
        var inputLines = new Queue<string>(["3", "4"]); // Display Balance then Exit
        var view = new ClientView(mockClient.Object, mockAuth.Object, () => inputLines.Dequeue());

        var user = new User { Id = 1 };
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            await view.Show(user);

            // Assert
            mockClient.Verify(s => s.GetBalance(1), Times.Once);
            Assert.Contains("You have choose to View you money", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            output.Dispose();
        }
    }

    [Fact]
    public async Task Show_Should_Exit_When_Option4_Selected()
    {
        // Arrange
        var mockClient = new Mock<IClientService>();
        var mockAuth = new Mock<IAuthService>();
        var inputLines = new Queue<string>(["4"]); // Exit immediately
        var view = new ClientView(mockClient.Object, mockAuth.Object, () => inputLines.Dequeue());

        var user = new User { Id = 1 };
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            await view.Show(user);

            // Assert
            mockAuth.Verify(a => a.Exit(), Times.Once);
            Assert.Contains("Exiting Client menu", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            output.Dispose();
        }
    }

    [Fact]
    public async Task Show_Should_Handle_InvalidInput_Then_Exit()
    {
        // Arrange
        var mockClient = new Mock<IClientService>();
        var mockAuth = new Mock<IAuthService>();
        var inputLines = new Queue<string>(["invalid", "4"]); // Invalid input then Exit
        var view = new ClientView(mockClient.Object, mockAuth.Object, () => inputLines.Dequeue());

        var user = new User { Id = 1 };
        var originalOut = Console.Out;
        var output = new StringWriter();
        Console.SetOut(output);

        try
        {
            // Act
            await view.Show(user);

            // Assert
            Assert.Contains("Invalid input. Please enter a number.", output.ToString());
            Assert.Contains("Exiting Client menu", output.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
            output.Dispose();
        }
    }
}
