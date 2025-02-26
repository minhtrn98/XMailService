using XMailService.Application.Models;

namespace XMailService.Application.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}
