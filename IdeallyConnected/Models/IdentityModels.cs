﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data; // DataSet, etc. Represents ADO.NET
using System;
using System.Collections.Generic;

namespace IdeallyConnected.Models
{
    public partial class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            //Skills = new HashSet<Skill>();
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;            
        }
        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            //Configuration.ProxyCreationEnabled = false;
            //Configuration.LazyLoadingEnabled = false;
        }
        
        //public DbSet<Skill> Skills { get; set; }
        public DbSet<Programming> Programmings { get; set; }
        public DbSet<Design> Designs { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ApplicationUser.UserMapper());
            //modelBuilder.Entity<ApplicationUser>().HasMany(us
            base.OnModelCreating(modelBuilder);
        }
        
        /*
        internal object Entity<T>(T entity)
        {
            throw new NotImplementedException();
        }
        */
    }



}