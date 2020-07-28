using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
                : base(options)
        {
            Database.EnsureCreated();
        }

        public ApplicationContext()
        {

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
                .HasMany(u => u.Users2);

            modelBuilder.Entity<UserUser>()
                .HasKey(uu => new { uu.UserId, uu.FriendId });

            modelBuilder.Entity<UserUser>()
                .HasOne(uu => uu.User)
                .WithMany(u => u.Users1)
                .HasForeignKey(uu => uu.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserUser>()
                  .HasOne(uu => uu.Friend)
                  .WithMany(u => u.Users2)
                  .HasForeignKey(uu => uu.FriendId)
                  .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invites)
                .HasForeignKey(i => i.UserLogin);

            modelBuilder.Entity<Invite>()
                .HasOne(i => i.Sender)
                .WithMany(u => u.SendedInvites)
                .HasForeignKey(i => i.SenderLogin);
               
        }
    }

    
}
