using Volo.Abp.Modularity;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

/* Inherit from this class for your domain layer tests.
 * See SampleManager_Tests for example.
 */
public abstract class PasswordAuthorizationDomainTestBase<TStartupModule> : PasswordAuthorizationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
