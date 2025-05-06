// <copyright file="ATMContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ATMApp.Data
{
    using ATMApp.Models;
    using Microsoft.EntityFrameworkCore;

    public class ATMContext : DbContext
    {
        public ATMContext(DbContextOptions<ATMContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        public DbSet<Account> Account { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
            .HasOne(a => a.User)
            .WithOne(u => u.Account)
            .HasForeignKey<Account>(a => a.ClientID)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
