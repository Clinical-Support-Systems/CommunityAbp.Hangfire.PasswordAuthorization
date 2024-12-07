using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Samples;

public class SampleAppService : PasswordAuthorizationAppService, ISampleAppService
{
    public Task<SampleDto> GetAsync()
    {
        return Task.FromResult(
            new SampleDto
            {
                Value = 42
            }
        );
    }

    [Authorize]
    public Task<SampleDto> GetAuthorizedAsync()
    {
        return Task.FromResult(
            new SampleDto
            {
                Value = 42
            }
        );
    }
}
