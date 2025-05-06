// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Models
{
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public class User
    {
        public int Id { get; set; }

        public string HolderName { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string PinCode { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Client;

        public Account? Account { get; set; }
    }

    public enum UserRole
    {
        Client,
        Admin,
    }
}
