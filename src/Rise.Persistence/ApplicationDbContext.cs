using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.CampusLife;
using Rise.Domain.Dashboard;
using Rise.Domain.Education;
using Rise.Domain.Infrastructure;
using Rise.Domain.Entities;
using Rise.Domain.Departments;
using Rise.Domain.Users;
using Rise.Domain.Events;
using Rise.Domain.News;
using Rise.Domain.Notifications;

namespace Rise.Persistence;

/// <summary>
/// Entrance to the database, inherits from IdentityDbContext and is basically a Unit Of Work and Repository pattern combined.
/// A <see cref="DbSet"/> is a repository for a specific type of entity.
/// The <see cref="ApplicationDbContext"/> is the Unit Of Work pattern
/// Will look very similar when switching database providers.
/// See https://hogent-web.github.io/csharp/chapters/09/slides/index.html#1
/// See https://enterprisecraftsmanship.com/posts/should-you-abstract-database/
/// </summary>
/// <param name="opts"></param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : IdentityDbContext<IdentityUser>(opts)
{

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Resto> Restos => Set<Resto>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<StudyField>  StudyFields => Set<StudyField>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Deadline> Deadlines => Set<Deadline>();
    public DbSet<StudyField> Studyfields => Set<StudyField>();
    public DbSet<StudentDeadline> StudentDeadlines => Set<StudentDeadline>();
    public DbSet<Shortcut> Shortcuts => Set<Shortcut>();
    public DbSet<News> News => Set<News>();
    public DbSet<UserShortcut> UserShortcuts => Set<UserShortcut>();
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<StudentClub> StudentClubs => Set<StudentClub>();
    public DbSet<StudentDeal> StudentDeals => Set<StudentDeal>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<ClassGroup> Classgroups => Set<ClassGroup>();
    public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // All columns in the database have a maxlength of 4000.
        // in NVARACHAR 4000 is the maximum length that can be indexed by a database.
        // Some columns need more length, but these can be set on the configuration level for that Entity in particular.
        configurationBuilder.Properties<string>().HaveMaxLength(256);
        // All decimals columns should have 2 digits after the comma
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
       // configurationBuilder.Properties<string>()
       // .HaveMaxLength(4000)
        //.HaveAnnotation("IsIdentity", false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Applying all types of IEntityTypeConfiguration in the Persistence project.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        //modelBuilder.ApplyConfiguration(new MenuItemConfiguration());

        //modelBuilder.Entity<MenuItem>(b => { b.HasKey(p => p.Id); });
    }
    
    
}