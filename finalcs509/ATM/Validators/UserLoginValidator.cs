// <copyright file="UserLoginValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Validators
{
    using ATMApp.DTOs;
    using ATMApp.Models;
    using FluentValidation;

    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            this.RuleFor(user => user.Login)
            .NotEmpty().WithMessage("Login is required.");

            this.RuleFor(user => user.PinCode)
            .NotEmpty().WithMessage("PIN code is required. ")
            .Length(5).WithMessage("PIN code must be exactly 5 digits.");
        }
    }
}
