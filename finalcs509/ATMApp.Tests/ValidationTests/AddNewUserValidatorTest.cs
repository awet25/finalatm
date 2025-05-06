// <copyright file="AddNewUserValidatorTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Tests.Validators
{
    using ATMApp.DTOs;
    using ATMApp.Models;
    using ATMApp.Validators;
    using FluentValidation.TestHelper;
    using Xunit;

    public class AddNewUserValidatorTest
    {
        private readonly AddNewuserValidator validator;

        public AddNewUserValidatorTest()
        {
            this.validator = new AddNewuserValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Login_Is_Empty()
        {
            var model = new CreateUserDto { Login = string.Empty };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Login)
                .WithErrorMessage("Login is required.");
        }

        [Fact]
        public void Should_Have_Error_When_PinCode_Is_Empty()
        {
            var model = new CreateUserDto { PinCode = string.Empty };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.PinCode)
                .WithErrorMessage("PIN code is required.");
        }

        [Fact]
        public void Should_Have_Error_When_PinCode_Is_Not_5_Digits()
        {
            var model = new CreateUserDto { PinCode = "123" };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.PinCode)
                .WithErrorMessage("PIN code must be exactly 5 digits.");
        }

        [Fact]
        public void Should_Have_Error_When_HolderName_Is_Empty()
        {
            var model = new CreateUserDto { HolderName = string.Empty };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.HolderName)
                .WithErrorMessage("Holder Name is required.");
        }

        [Fact]
        public void Should_Have_Error_When_HolderName_Exceeds_Max_Length()
        {
            var model = new CreateUserDto { HolderName = new string('A', 51) };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.HolderName)
                .WithErrorMessage("Holder Name cannot exceed 50 characters.");
        }

        [Fact]
        public void Should_Have_Error_When_Role_Is_Invalid()
        {
            var model = new CreateUserDto { Role = (UserRole)999 };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Role)
                .WithErrorMessage("Invalid role. Choose either 'Admin' or 'Client'.");
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Invalid()
        {
            var model = new CreateUserDto { Status = (AccountStatus)999 };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Status)
                .WithErrorMessage("Invalid status. Choose either 'Active','Inactive'");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            var model = new CreateUserDto
            {
                Login = "ValidLogin",
                PinCode = "12345",
                HolderName = "Valid Holder",
                Role = UserRole.Client,
                Status = AccountStatus.Active,
            };
            var result = this.validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
