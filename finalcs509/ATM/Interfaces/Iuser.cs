// <copyright file="Iuser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Interfaces
{
    using ATMApp.Models;

    public interface Iuser
    {
        void AddUser(User user);

        List<User> GetAllPeople();
    }
}
