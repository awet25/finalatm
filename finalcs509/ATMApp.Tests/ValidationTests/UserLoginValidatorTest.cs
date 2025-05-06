// <copyright file="UserLoginValidatorTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Tests.Validators
{
    using ATMApp.DTOs;
    using ATMApp.Validators;
    using FluentValidation.TestHelper;

    public class UserLoginValidatorTest
    {
        private readonly UserLoginValidator validator;

        public UserLoginValidatorTest()
        {
            this.validator = new UserLoginValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Login_Is_Empty()
        {
            var model = new UserLoginDTO { Login = string.Empty, PinCode = "12345" };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.Login)
                  .WithErrorMessage("Login is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Login_Is_Provided()
        {
            var model = new UserLoginDTO { Login = "testuser", PinCode = "12345" };
            var result = this.validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(user => user.Login);
        }

        [Fact]
        public void Should_Have_Error_When_PinCode_Is_Empty()
        {
            var model = new UserLoginDTO { Login = "testuser", PinCode = string.Empty };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.PinCode)
                  .WithErrorMessage("PIN code is required. ");
        }

        [Fact]
        public void Should_Have_Error_When_PinCode_Is_Not_5_Digits()
        {
            var model = new UserLoginDTO { Login = "testuser", PinCode = "1234" };
            var result = this.validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(user => user.PinCode)
                  .WithErrorMessage("PIN code must be exactly 5 digits.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_PinCode_Is_5_Digits()
        {
            var model = new UserLoginDTO { Login = "testuser", PinCode = "12345" };
            var result = this.validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(user => user.PinCode);
        }
    }
}
