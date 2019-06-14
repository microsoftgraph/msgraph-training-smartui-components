/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GroupsReact.Models;
using GroupsReact.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Graph;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GroupsReact.Controllers
{
  [Authorize]
  public class HomeController : ControllerBase
  {
    private readonly IGraphSdkHelper _graphSdkHelper;
    private MSALLogCallback _msalLog;

    public HomeController(IGraphSdkHelper graphSdkHelper, IMemoryCache memoryCache, MSALLogCallback msalLog)
      : base(memoryCache)
    {
      _graphSdkHelper = graphSdkHelper;
      _msalLog = msalLog;
    }

    public async Task<IActionResult> Index(string email)
    {
      UserModel userModel = null;

      if (User.Identity.IsAuthenticated)
      {
        // Get user's id for token cache.
        var identifier = User.FindFirst(Startup.ObjectIdentifierType)?.Value;

        userModel = base.GetUserModelFromCache(identifier);

        if (userModel == null)
        {
          // Get users's email.
          email = email ?? User.FindFirst("preferred_username").Value;

          // Initialize the GraphServiceClient.
          var graphClient = _graphSdkHelper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);

          try
          { 
            var userData = await GraphService.GetUserJson(graphClient, email, HttpContext);
            var userObject = JsonConvert.DeserializeObject<User>(userData);

            userModel = new UserModel
            {
              Id = identifier,
              Name = userObject.DisplayName,
              Email = userObject.Mail
            };

            var pic = await GraphService.GetPictureBase64(graphClient, email, HttpContext);
            userModel.PictureBase64 = pic;

            // dont store an empty model
            if (!string.IsNullOrEmpty(userModel.Name))
            {
              base.SaveUserModelInCache(userModel);
            }
          }
          catch (ServiceException e)
          {
            switch (e.Error.Code)
            {
              case "Authorization_RequestDenied":
                return new RedirectResult("/Account/PermissionsRequired");
              default:
                return new RedirectResult($"/Home/Error?msg={e.Error.Message}");
            }
          }
        }

        base.CopyUserModelToViewData(identifier);

        System.Diagnostics.Debug.WriteLine(_msalLog.GetLog());
      }

      return View();
    }

    [AllowAnonymous]
    public IActionResult Error(string msg)
    {
      var model = new ErrorViewModel
      {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        Message = msg
      };
      return View(model);
    }
  }
}
