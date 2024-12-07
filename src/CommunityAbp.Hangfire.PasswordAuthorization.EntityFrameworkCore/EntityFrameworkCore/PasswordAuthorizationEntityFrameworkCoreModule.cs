using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace CommunityAbp.Hangfire.PasswordAuthorization.EntityFrameworkCore;

[DependsOn(
    typeof(PasswordAuthorizationDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class PasswordAuthorizationEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<PasswordAuthorizationDbContext>(options =>
        {
                /* Add custom repositories here. Example:
                 * options.AddRepository<Question, EfCoreQuestionRepository>();
                 */
        });
    }
}
