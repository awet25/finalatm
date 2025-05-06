// <copyright file="IUserRepositories.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Repositories
{
    using ATMApp.DTOs;
    using ATMApp.Models;

    public interface IUserRepository
    {
        Task<User> GetUserBylogin(string login);

        Task<User> GetUserById(int id);

        Task<User> AddUser(User newUser);

        Task<bool> DeleteUserbyId(int userId);

        Task<bool> UpdateUser(User user);

        Task<User?> GetUserWithAccountByLogin(string login);
    }
}
