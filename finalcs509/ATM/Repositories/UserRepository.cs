// <copyright file="UserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Repositories
{
    using ATMApp.Data;
    using ATMApp.Interfaces;
    using ATMApp.Models;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.EntityFrameworkCore;
    using Org.BouncyCastle.Security;

    public class UserRepository : IUserRepository
    {
        private readonly ATMContext context;

        public UserRepository(ATMContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// This methode will return  a User by login.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<User> GetUserBylogin(string login)
        {
            return await this.context.User.FirstOrDefaultAsync(x => x.Login == login);
        }

        /// <summary>
        /// This methode will return  a User by Id.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<User> GetUserById(int id)
        {
            return await this.context.User.FindAsync(id);
        }

        /// <summary>
        /// This methode will return  a User after we add the user to the database.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<User> AddUser(User user)
        {
            var createdUser = await this.context.User.AddAsync(user);
            await this.context.SaveChangesAsync();
            return createdUser.Entity;
        }

        /// <summary>
        /// This methode will return  a bool after we delete the user by Id.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<bool> DeleteUserbyId(int id)
        {
            var user = await this.GetUserById(id);
            if (user == null)
            {
                return false;
            }

            this.context.User.Remove(user);
            await this.context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// This methode will return a bool  after  updating a user.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<bool> UpdateUser(User user)
        {
            var existingUser = await this.context.User.FindAsync(user.Id);
            if (existingUser == null)
            {
                return false;
            }

            this.context.Entry(existingUser).CurrentValues.SetValues(user);
            await this.context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// This methode will return  a User with their account by Login.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<User?> GetUserWithAccountByLogin(string login)
        {
            return await this.context.User
          .Include(u => u.Account)
          .FirstOrDefaultAsync(u => u.Login == login);
        }
    }
}
