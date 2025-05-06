// <copyright file="IAuthService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Interfaces
{
    using ATMApp.DTOs;
    using ATMApp.Models;

    public interface IAuthService
    {
        Task<User> Login(UserLoginDTO userLogin);

        void Exit();
    }
}
