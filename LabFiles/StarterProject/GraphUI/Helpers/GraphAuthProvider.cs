/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Linq;
using Microsoft.Extensions.Options;
using GroupsReact.Extensions;

namespace GroupsReact.Helpers
{
  public class GraphAuthProvider : IGraphAuthProvider
  {
    public const string ObjectIdentifierType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    public const string TenantIdType = "http://schemas.microsoft.com/identity/claims/tenantid";
    public const string AdminConsentFormat = "https://login.microsoftonline.com/{0}/adminconsent?client_id={1}&state={2}&redirect_uri={3}";


    private readonly IMemoryCache _memoryCache;
    private TokenCache _userTokenCache;

    // Properties used to get and manage an access token.
    private readonly string _appId;
    private readonly ClientCredential _credential;
    private readonly string[] _scopes;
    private readonly string _redirectUri;

    public GraphAuthProvider(IMemoryCache memoryCache, IOptions<AzureAdOptions> options)  //IConfiguration configuration
    {
      var azureOptions = options.Value;
      _appId = azureOptions.ClientId;
      _credential = new ClientCredential(azureOptions.ClientSecret);
      _scopes = azureOptions.GraphScopes.Split(new[] { ' ' });
      _redirectUri = azureOptions.BaseUrl;// + azureOptions.CallbackPath;

      _memoryCache = memoryCache;
    }

    // Gets an access token. First tries to get the access token from the token cache.
    // Using password (secret) to authenticate. Production apps should use a certificate.
    public async Task<string> GetUserAccessTokenAsync(string userId)
    {
      _userTokenCache = new InMemoryTokenCache(userId, _memoryCache).GetCacheInstance();

      var cca = new ConfidentialClientApplication(
          _appId,
          _redirectUri,
          _credential,
          _userTokenCache,
          null);

      if (!cca.Users.Any()) throw new ServiceException(new Error
      {
        Code = "TokenNotFound",
        Message = "User not found in token cache. Maybe the server was restarted."
      });

      try
      {
        var result = await cca.AcquireTokenSilentAsync(_scopes, cca.Users.First());
        return result.AccessToken;
      }

      // Unable to retrieve the access token silently.
      catch (Exception)
      {
        throw new ServiceException(new Error
        {
          Code = GraphErrorCode.AuthenticationFailure.ToString(),
          Message = "Caller needs to authenticate. Unable to retrieve the access token silently."
        });
      }
    }
  }

  public interface IGraphAuthProvider
  {
    Task<string> GetUserAccessTokenAsync(string userId);
  }
}
