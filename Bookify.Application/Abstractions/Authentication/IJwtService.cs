using Bookify.Domain.Abstractions;

namespace Bookify.Application.Abstractions.Authentication;

public interface IJwtService
{
    Task<Result<string>> GetAccessTokenAsync(string requestEmail, string requestPassword, CancellationToken cancellationToken = default);
}