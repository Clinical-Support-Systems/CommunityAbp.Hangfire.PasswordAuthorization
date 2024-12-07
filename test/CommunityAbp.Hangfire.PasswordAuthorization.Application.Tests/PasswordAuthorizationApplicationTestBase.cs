using Volo.Abp.Modularity;

namespace CommunityAbp.Hangfire.PasswordAuthorization;

/* Inherit from this class for your application layer tests.
 * See SampleAppService_Tests for example.
 */
public abstract class PasswordAuthorizationApplicationTestBase<TStartupModule> : PasswordAuthorizationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
