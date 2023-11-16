using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using oopd_project.DBContext.DBModels;

namespace oopd_project
{
	public class DataBaseContext : DbContext
	{
        public DbSet<Administrator> Administrators { get; set; } = null!;
        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Coach> Coaches { get; set; } = null!;
        public DbSet<Schedule> Schedule { get; set; } = null!;
        public DbSet<SubscriptionClass> Subscription_Classes { get; set; } = null!;
        public DbSet<Subscription_Type> Subscription_Types { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Support> Support { get; set; } = null!;
        public DbSet<UserRole> User_Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        public DataBaseContext()
        {
            bool res = Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            optionsBuilder.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(8, 1, 0)));
        }
    }
}

