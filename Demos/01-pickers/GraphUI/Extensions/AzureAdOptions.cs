/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

namespace GroupsReact.Extensions
{
  public class AzureAdOptions
  {
    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string Instance { get; set; }

    public string Domain { get; set; }

    public string TenantId { get; set; }

    public string CallbackPath { get; set; }

    public string BaseUrl { get; set; }

    public string Scopes { get; set; }

    public string GraphResourceId { get; set; }

    public string GraphScopes { get; set; }
  }
}
