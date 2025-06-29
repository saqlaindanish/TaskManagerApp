using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Models.Entities;

namespace TaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)  : base(options) {}


        public DbSet<TaskItem> Tasks { get; set; } // DbSet for Task entity
        public DbSet<Notification> Notifications { get; set; } // Notification entity
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Additional model configuration can go here

            // delete user's tasks if user is deleted
            builder.Entity<TaskItem>()
                    .HasOne(t => t.User)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Notification>()
                    .HasOne(n=>n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.NoAction);


            // set taskId to null in notitcation if task is deleted 
            builder.Entity<Notification>()
                    .HasOne(n => n.Task)
                    .WithMany(t => t.Notifications)
                    .HasForeignKey(n => n.TaskId)
                    .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
