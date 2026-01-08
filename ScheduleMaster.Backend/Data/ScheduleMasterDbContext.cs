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
        public DbSet<StudioUser> StudiosUsers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupsUsers { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventGroup> EventsGroups { get; set; }
        public DbSet<StudioCategory> StudiosCategories { get; set; }

        // Настройка схемы (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // users
            modelBuilder.Entity<User>().ToTable("users")
                .HasKey(user => user.Id);

            // modelBuilder.Entity<User>()
            //     .HasIndex(user => user.Email)
            //     .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(user => user.ChatId)
                .IsUnique();

            // studio_categories
            modelBuilder.Entity<StudioCategory>().ToTable("studio_categories")
                .HasKey(studio_categories => studio_categories.Id);

            // studios
            modelBuilder.Entity<Studio>().ToTable("studios")
                .HasKey(studio => studio.Id);

            modelBuilder.Entity<Studio>()
                .HasOne<StudioCategory>()
                .WithMany()
                .HasForeignKey(studio => studio.StudioCategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // groups
            modelBuilder.Entity<Group>().ToTable("groups")
                .HasKey(group => group.Id);

            modelBuilder.Entity<Group>()
                .HasOne<Studio>()
                .WithMany()
                .HasForeignKey(group => group.StudioId)
                .OnDelete(DeleteBehavior.Cascade);

            // studios_users
            modelBuilder.Entity<StudioUser>().ToTable("studios_users")
                .HasKey(studios_users => new { studios_users.StudentId, studios_users.StudioId });

            modelBuilder.Entity<StudioUser>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(studios_users => studios_users.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudioUser>()
                .HasOne<Studio>()
                .WithMany()
                .HasForeignKey(studios_users => studios_users.StudioId)
                .OnDelete(DeleteBehavior.Cascade);

            // groups_users
            modelBuilder.Entity<GroupUser>().ToTable("groups_users")
                .HasKey(groups_users => new { groups_users.StudentId, groups_users.GroupId });

            modelBuilder.Entity<GroupUser>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(groups_users => groups_users.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupUser>()
                .HasOne<Group>()
                .WithMany()
                .HasForeignKey(groups_users => groups_users.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // events
            modelBuilder.Entity<Event>().ToTable("events")
                .HasKey(ev => ev.Id);

            // events_groups
            modelBuilder.Entity<EventGroup>().ToTable("events_groups")
                .HasKey(events_groups => new { events_groups.EventId, events_groups.GroupId });

            modelBuilder.Entity<EventGroup>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(events_groups => events_groups.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventGroup>()
                .HasOne<Group>()
                .WithMany()
                .HasForeignKey(events_groups => events_groups.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

