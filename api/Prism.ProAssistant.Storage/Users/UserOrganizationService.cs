using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Prism.Core.Exceptions;
using Prism.Infrastructure.Authentication;
using Prism.Infrastructure.Providers;

namespace Prism.ProAssistant.Storage.Users;

public interface IUserOrganizationService
{
    string? GetUserId();
    Task<string> GetUserOrganization();
    string? GetName();
}

public class UserOrganizationService : IUserOrganizationService
{
    private readonly IDistributedCache _cache;
    private readonly IGlobalStateProvider _globalStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserOrganizationService> _logger;

    public UserOrganizationService(ILogger<UserOrganizationService> logger, IHttpContextAccessor httpContextAccessor, IDistributedCache cache, IGlobalStateProvider globalStateProvider)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _cache = cache;
        _globalStateProvider = globalStateProvider;
    }

    public async Task<string> GetUserOrganization()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
        {
            return "demo";
        }

        var userId = GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new NotFoundException("The collection was not found because the user has no id.");
        }

        if (await _cache.GetAsync($"organization-{userId}") is { } organizationCached)
        {
            return Encoding.UTF8.GetString(organizationCached);
        }

        _logger.LogInformation("The user {userId} was not found in the cache, querying the database.", userId);

        var userContainer = await _globalStateProvider.GetGlobalContainerAsync<UserOrganization>();
        var user = await userContainer.ReadAsync(userId);

        if (user != null)
        {
            var organization = user.Organization;
            await _cache.SetStringAsync($"organization-{userId}", organization, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return organization;
        }

        _logger.LogWarning("The user {userId} was not found in the database, defaulting to the demo database.", userId);

        await userContainer.WriteAsync(userId, new UserOrganization
        {
            Id = userId,
            Organization = "demo"
        });

        return "demo";
    }

    public string? GetName()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
    }

    public string? GetUserId()
    {
        if (_httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}