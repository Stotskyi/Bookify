using System.Net.Http.Json;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Users;
using Bookify.Infrastructure.Authentication.Models;

namespace Bookify.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string PasswordCredentialType = "password";

    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> RegisterAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var userRepresentialModel = UserRepresentationModel.FromUser(user);

        userRepresentialModel.Credentials = new CredentialRepresentationModel[]
        {
            new()
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "users",
            userRepresentialModel,
            cancellationToken);

        return ExtractIdentityIdFromLocationheader(response);
    }

    private static string ExtractIdentityIdFromLocationheader(HttpResponseMessage httpResponseMessage)
    {
        const string userSegmentName = "users/";

        var locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header was not found.");
        }

        var userSegmentValueIndex = locationHeader.IndexOf(
            userSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        var userIdentityId = locationHeader.Substring(userSegmentValueIndex + userSegmentName.Length);

        return userIdentityId;
    }
}