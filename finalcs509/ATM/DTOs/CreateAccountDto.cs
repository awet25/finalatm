// <copyright file="CreateAccountDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.DTOs
{
    using ATMApp.Models;

    public class CreateAccountDto
    {
        public int ClientID { get; set; }

        public decimal IntialBalance { get; set; } = 0.0m;

        public AccountStatus Status { get; set; } = AccountStatus.Active;
    }
}
