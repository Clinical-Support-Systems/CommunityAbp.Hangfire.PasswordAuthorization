using Volo.Abp.Modularity;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(PasswordAuthorizationApplicationModule),
    typeof(PasswordAuthorizationDomainTestModule)
    )]
public class PasswordAuthorizationApplicationTestModule : AbpModule
{

}
