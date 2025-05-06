// <copyright file="UpdateUserDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.DTOs
{
    using ATMApp.Models;

    public class UpdateUserDto
    {
        public int Id { get; set; }

        public string? HolderName { get; set; }

        public string? Login { get; set; }

        public string? PinCode { get; set; }

        public AccountStatus? Status { get; set; }
    }
}
