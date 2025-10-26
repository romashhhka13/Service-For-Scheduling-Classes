using Microsoft.EntityFrameworkCore;
using ScheduleMaster.Models;


namespace ScheduleMaster.Data
{
    public class ScheduleMasterDbContext : DbContext
    {
        public ScheduleMasterDbContext(DbContextOptions<ScheduleMasterDbContext> options) : base(options) { }

        // Таблицы в БД
        public DbSet<User> Users { get; set; }
        public DbSet<Studio> Studios { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<StudioMembership> StudioMemberships { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        // Настройка схемы (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // users
            modelBuilder.Entity<User>().ToTable("users")
                .HasKey(user => user.Id);

            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            // studios
            modelBuilder.Entity<Studio>().ToTable("studios")
                .HasKey(studio => studio.Id);

            modelBuilder.Entity<Studio>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(studio => studio.AdministratorId)
                .OnDelete(DeleteBehavior.Restrict);


            // groups
            modelBuilder.Entity<Group>().ToTable("groups")
                .HasKey(group => group.Id);

            modelBuilder.Entity<Group>()
                .HasOne<Studio>()
                .WithMany()
                .HasForeignKey(group => group.StudioId);

            // studio_memberships
            modelBuilder.Entity<StudioMembership>().ToTable("studio_memberships")
                .HasKey(studio_membership => new { studio_membership.StudentId, studio_membership.StudioId });

            modelBuilder.Entity<StudioMembership>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(studio_membership => studio_membership.StudentId);

            modelBuilder.Entity<StudioMembership>()
                .HasOne<Studio>()
                .WithMany()
                .HasForeignKey(studio_membership => studio_membership.StudioId);

            // group_memberships
            modelBuilder.Entity<GroupMembership>().ToTable("group_memberships")
                .HasKey(group_membership => new { group_membership.StudentId, group_membership.GroupId });

            modelBuilder.Entity<GroupMembership>()
                .HasIndex(group_membership => new { group_membership.StudentId, group_membership.GroupId })
                .IsUnique();

            modelBuilder.Entity<GroupMembership>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(group_membership => group_membership.StudentId);

            modelBuilder.Entity<GroupMembership>()
                .HasOne<Group>()
                .WithMany()
                .HasForeignKey(group_membership => group_membership.GroupId);

            // schedules
            modelBuilder.Entity<Schedule>().ToTable("schedules")
                .HasKey(schedule => schedule.Id);

            modelBuilder.Entity<Schedule>()
                .HasOne<Group>()
                .WithMany()
                .HasForeignKey(schedule => schedule.GroupId);
        }

    }
}

