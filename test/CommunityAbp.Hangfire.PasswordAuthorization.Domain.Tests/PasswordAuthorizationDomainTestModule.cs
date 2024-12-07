using Volo.Abp.Modularity;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(PasswordAuthorizationDomainModule),
    typeof(PasswordAuthorizationTestBaseModule)
)]
public class PasswordAuthorizationDomainTestModule : AbpModule
{

}
