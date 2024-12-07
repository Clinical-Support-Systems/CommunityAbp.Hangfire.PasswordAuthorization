using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(PasswordAuthorizationDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class PasswordAuthorizationApplicationContractsModule : AbpModule
{

}
