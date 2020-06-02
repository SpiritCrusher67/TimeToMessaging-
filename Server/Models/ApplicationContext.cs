using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TTMLibrary.Models;

namespace Server.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
                : base(options)
        {
           var g= Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasKey(t => new { t.UserLogin, t.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.Groups)
                .HasForeignKey(ug => ug.UserLogin);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.Users)
                .HasForeignKey(ug => ug.GroupId);

            modelBuilder.Entity<User>()
                .HasKey(u => u.Login);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Users);

            modelBuilder.Entity<UserUser>()
                .HasKey(uu => uu.UserId);

            modelBuilder.Entity<UserUser>()
                .HasKey(uu => uu.FriendId);

            modelBuilder.Entity<UserUser>()
                .HasOne(uu => uu.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uu => uu.UserId);

            modelBuilder.Entity<UserUser>()
                  .HasOne(uu => uu.Friend)
                  .WithMany(u => u.Users)
                  .HasForeignKey(uu => uu.FriendId);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invites)
                .HasForeignKey(i => i.UserLogin);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Sender)
                .WithMany(u => u.SendedInvites)
                .HasForeignKey(i => i.SenderLogin);

            User User1 = new User { Login = "1", Password = "1" };
            User User2 = new User { Login = "2", Password = "2" };
            modelBuilder.Entity<User>()
                .HasData(new User[] { User1, User2 });
        }
    }

    
}
