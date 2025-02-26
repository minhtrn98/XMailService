using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using XMailService.Application.Interfaces;
using XMailService.Application.Models;

namespace XMailService.Infrastructure.Services;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private CurrentUser? _currentUser;

    public CurrentUser GetCurrentUser()
    {
        if (_currentUser is not null)
        {
            return _currentUser;
        }

        string id = GetSingleClaimValue(ClaimTypes.NameIdentifier);
        return _currentUser ??= new CurrentUser(id);
    }

    private string GetSingleClaimValue(string claimType) =>
        httpContextAccessor.HttpContext!.User.Claims
            .Single(claim => claim.Type == claimType)
            .Value;
}
