// <copyright file="AddNewUserValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Validators
{
    using ATMApp.DTOs;
    using FluentValidation;

    public class AddNewuserValidator : AbstractValidator<CreateUserDto>
    {
        public AddNewuserValidator()
        {
            this.RuleFor(user => user.Login)
                .NotEmpty().WithMessage("Login is required.");

            this.RuleFor(user => user.PinCode)
                .NotEmpty().WithMessage("PIN code is required.")
                .Length(5).WithMessage("PIN code must be exactly 5 digits.");

            this.RuleFor(user => user.HolderName)
                .NotEmpty().WithMessage("Holder Name is required.")
                .MaximumLength(50).WithMessage("Holder Name cannot exceed 50 characters.");

            this.RuleFor(user => user.Role)
                .IsInEnum().WithMessage("Invalid role. Choose either 'Admin' or 'Client'.");

            this.RuleFor(user => user.Status).IsInEnum().WithMessage("Invalid status. Choose either 'Active','Inactive'");
        }
    }
}
