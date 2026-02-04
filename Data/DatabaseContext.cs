using LMS.Data.Configuration;
using LMS.Data.Entities;
using LMS.Data.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options, IServiceProvider serviceProvider)
    : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var interceptor = serviceProvider.GetService<PostgresConnectionInterceptor>();
        if (interceptor != null)
            optionsBuilder.AddInterceptors(interceptor);
        
        base.OnConfiguring(optionsBuilder);
    }
    
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UsersRole { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<LegalForm> LegalForms { get; set; }
    public DbSet<Moderator> Moderators { get; set; }
    public DbSet<Branch> Branches { get; set; }
    public DbSet<BranchesModerators> BranchesModerators { get; set; }
    public DbSet<BranchesDirectors> BranchesDirectors { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<AuditHistory> AuditHistories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionCourseList> SubscriptionsCourses { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<AssessmentCourse> AssessmentsCourses { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseCategory> CourseCategories { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialCourse>  MaterialsCourses { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    
}