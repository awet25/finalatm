// <copyright file="UserLoginDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.DTOs
{
    public class UserLoginDTO
    {
        public required string Login { get; set; }

        public required string PinCode { get; set; }
    }
}
