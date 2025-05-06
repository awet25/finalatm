// <copyright file="IAdminServices.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Interfaces
{
    using ATMApp.DTOs;
    using ATMApp.Models;

    public interface IAdminservices
    {
        Task<bool> AddUser(CreateUserDto createUserDto);

        Task<User> GetUserByLogin(string login);

        Task<Account> GetAccount(int id);

        Task<bool> DeleteUserAndAccount(int userId, string confirmationInput);

        Task<bool> UpdateUser(UpdateUserDto updateUserDto);
    }
}
