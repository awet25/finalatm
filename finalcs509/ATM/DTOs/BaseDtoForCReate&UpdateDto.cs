// <copyright file="BaseDtoForCReate&amp;UpdateDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.DTOs
{
    using ATMApp.Models;

    public class BaseDto
    {
        public string Login { get; set; }

        public string PinCode { get; set; }

        public string HolderName { get; set; }

        public UserRole Role { get; set; } = UserRole.Client;
    }
}
