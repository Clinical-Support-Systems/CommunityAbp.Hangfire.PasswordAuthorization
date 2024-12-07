using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace CommunityAbp.Hangfire.PasswordAuthorization.Samples;

[Area(PasswordAuthorizationRemoteServiceConsts.ModuleName)]
[RemoteService(Name = PasswordAuthorizationRemoteServiceConsts.RemoteServiceName)]
[Route("api/PasswordAuthorization/sample")]
public class SampleController : PasswordAuthorizationController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService;

    public SampleController(ISampleAppService sampleAppService)
    {
        _sampleAppService = sampleAppService;
    }

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
