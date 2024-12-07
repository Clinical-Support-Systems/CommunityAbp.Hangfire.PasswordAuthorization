using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace CommunityAbp.Hangfire.PasswordAuthorization.EntityFrameworkCore;

[ConnectionStringName(PasswordAuthorizationDbProperties.ConnectionStringName)]
public interface IPasswordAuthorizationDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
}
