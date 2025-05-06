// <copyright file="BalanceResultDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.DTOs
{
    public class BalanceResultDto
    {
        public bool Success { get; set; }

        public required string Message { get; set; }

        public decimal? Balance { get; set; }

        public int? AccountId { get; set; }
    }
}
