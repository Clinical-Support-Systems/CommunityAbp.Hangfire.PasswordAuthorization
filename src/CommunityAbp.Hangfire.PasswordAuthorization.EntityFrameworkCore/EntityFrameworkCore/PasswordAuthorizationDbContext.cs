using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace CommunityAbp.Hangfire.PasswordAuthorization.EntityFrameworkCore;

[ConnectionStringName(PasswordAuthorizationDbProperties.ConnectionStringName)]
public class PasswordAuthorizationDbContext : AbpDbContext<PasswordAuthorizationDbContext>, IPasswordAuthorizationDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * public DbSet<Question> Questions { get; set; }
     */

    public PasswordAuthorizationDbContext(DbContextOptions<PasswordAuthorizationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePasswordAuthorization();
    }
}
