// <copyright file="CreateUserDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.DTOs
{
    using ATMApp.Models;

    public class CreateUserDto : BaseDto
    {
        public decimal IntialBalance { get; set; } = 0.0m;

        public AccountStatus Status { get; set; } = AccountStatus.Active;
    }
}
