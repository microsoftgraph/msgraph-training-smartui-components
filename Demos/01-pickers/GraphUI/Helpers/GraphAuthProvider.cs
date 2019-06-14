/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using GroupsReact.Extensions;

namespace GroupsReact.Helpers
{
  public class GraphAuthProvider : IGraphAuthProvider
  {
    public const string AdminConsentFormat = "https://login.microsoftonline.com/{0}/adminconsent?client_id={1}&state={2}&redirect_uri={3}";

    private readonly IConfidentialClientApplication _app;
    private readonly string[] _scopes;

    public GraphAuthProvider(IConfiguration configuration)
    {
      var azureOptions = new AzureAdOptions();
      configuration.Bind("AzureAd", azureOptions);

      _app = ConfidentialClientApplicationBuilder.Create(azureOptions.ClientId)
          .WithClientSecret(azureOptions.ClientSecret)
          .WithAuthority(AzureCloudInstance.AzurePublic, AadAuthorityAudience.AzureAdAndPersonalMicrosoftAccount)
          .WithRedirectUri(azureOptions.BaseUrl + azureOptions.CallbackPath)
          .Build();

      _scopes = azureOptions.GraphScopes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public async Task<string> GetUserAccessTokenAsync(string userId)
    {
      var account = await _app.GetAccountAsync(userId);
      if (account == null) throw new ServiceException(new Error
      {
        Code = "TokenNotFound",
        Message = "User not found in token cache. Maybe the server was restarted."
      });

      try
      {
        var result = await _app.AcquireTokenSilent(_scopes, account).ExecuteAsync();
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

    public async Task<AuthenticationResult> GetUserAccessTokenByAuthorizationCode(string authorizationCode)
    {
      return await _app.AcquireTokenByAuthorizationCode(_scopes, authorizationCode).ExecuteAsync();
    }
  }

  public interface IGraphAuthProvider
  {
    Task<string> GetUserAccessTokenAsync(string userId);
    Task<AuthenticationResult> GetUserAccessTokenByAuthorizationCode(string authorizationCode);
  }
}
