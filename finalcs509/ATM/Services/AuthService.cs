// <copyright file="AuthService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Services
{
    using ATMApp.DTOs;
    using ATMApp.Interfaces;
    using ATMApp.Models;
    using ATMApp.Repositories;
    using ATMApp.Validators;
    using FluentValidation;

    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IValidator<UserLoginDTO> validator;

        public AuthService(IUserRepository userRepository, IValidator<UserLoginDTO> validator)
        {
            this.userRepository = userRepository;
            this.validator = validator;
        }

        /// <summary>
        /// This methode will login our users.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<User> Login(UserLoginDTO userLogin)
        {
            var validationResult = this.validator.Validate(userLogin);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return null;
            }

            var user = await this.userRepository.GetUserBylogin(userLogin.Login);
            if (user != null && user.PinCode == userLogin.PinCode)
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// This methode will close the program.
        /// </summary>
        public void Exit()
        {
            Console.WriteLine("User Logged out.");
            return;
        }
    }
}
