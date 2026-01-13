using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data;

public class DatabaseContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IServiceProvider serviceProvider) : base(options)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var interceptor = _serviceProvider.GetService<PostgresConnectionInterceptor>();
        if (interceptor != null)
            optionsBuilder.AddInterceptors(interceptor);
        
        base.OnConfiguring(optionsBuilder);
    }
    
    public DbSet<Administrator> Administrators { get; set; }
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
}