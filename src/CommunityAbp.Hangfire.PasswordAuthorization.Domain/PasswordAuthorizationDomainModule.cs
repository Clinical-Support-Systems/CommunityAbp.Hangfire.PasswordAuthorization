using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(PasswordAuthorizationDomainSharedModule)
)]
public class PasswordAuthorizationDomainModule : AbpModule
{

}
