// <copyright file="AuthServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using ATMApp.DTOs;
using ATMApp.Interfaces;
using ATMApp.Models;
using ATMApp.Repositories;
using ATMApp.Services;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

public class AuthServiceTests
{
    [Fact]
    public async Task Login_validCredentials_ReturnsUser()
    {
        var loginDto = new UserLoginDTO { Login = "john", PinCode = "12345" };
        var mockValidator = new Mock<IValidator<UserLoginDTO>>();
        mockValidator.Setup(v => v.Validate(loginDto)).Returns(new ValidationResult());

        var expectedUser = new User { Id = 1, Login = "john", PinCode = "12345", HolderName = "john", Role = UserRole.Client };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserBylogin("john")).ReturnsAsync(expectedUser);
        var authService = new AuthService(mockRepo.Object, mockValidator.Object);
        var result = await authService.Login(loginDto);
        Assert.NotNull(result);
        Assert.Equal("john", result.Login);
    }

    [Fact]
    public async Task Login_InvalidPin_ReturnsNull()
    {
        var loginDto = new UserLoginDTO { Login = "john", PinCode = "wrong" };
        var mockValidator = new Mock<IValidator<UserLoginDTO>>();
        mockValidator.Setup(v => v.Validate(loginDto)).Returns(new ValidationResult());

        var expectedUser = new User { Id = 1, Login = "john", PinCode = "12345", HolderName = "john", Role = UserRole.Client };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserBylogin("john")).ReturnsAsync(expectedUser);
        var authService = new AuthService(mockRepo.Object, mockValidator.Object);
        var result = await authService.Login(loginDto);
        Assert.Null(result);
    }

    [Fact]
    public async Task Login_InValidModel_ReturnsNull()
    {
        var loginDto = new UserLoginDTO { Login = string.Empty, PinCode = string.Empty };
        var validationFailures = new List<ValidationFailure>
           {
            new ValidationFailure("Login", "Login is required"),
            new ValidationFailure("PinCode", "PinCode is required"),
           };

        var mockValidator = new Mock<IValidator<UserLoginDTO>>();
        mockValidator.Setup(v => v.Validate(loginDto)).Returns(new ValidationResult(validationFailures));

        var expectedUser = new User { Id = 1, Login = "john", PinCode = "12345", HolderName = "john", Role = UserRole.Client };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(r => r.GetUserBylogin("john")).ReturnsAsync(expectedUser);
        var authService = new AuthService(mockRepo.Object, mockValidator.Object);
        var result = await authService.Login(loginDto);
        Assert.Null(result);
    }
}
