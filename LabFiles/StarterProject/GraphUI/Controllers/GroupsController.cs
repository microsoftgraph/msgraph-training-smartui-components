using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GroupsReact.Helpers;
using GroupsReact.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;

namespace GroupsReact.Controllers
{
  public class GroupsController : ControllerBase
  {
    private readonly IGraphSdkHelper _graphSdkHelper;
    private MSALLogCallback _msalLog;

    public GroupsController(IGraphSdkHelper graphSdkHelper, IMemoryCache memoryCache, MSALLogCallback msalLog)
      : base(memoryCache)
    {
      _graphSdkHelper = graphSdkHelper;
      _msalLog = msalLog;
    }

    [Authorize]
    // GET: Group
    public async Task<ActionResult> Index()
    {
      List<GroupListItemViewModel> data = new List<GroupListItemViewModel>();

      // Get user's id for token cache.
      var identifier = User.FindFirst(Startup.ObjectIdentifierType)?.Value;
      base.CopyUserModelToViewData(identifier);

      // Initialize the GraphServiceClient.
      var graphClient = _graphSdkHelper.GetAuthenticatedClient((ClaimsIdentity)User.Identity);

      try
      {
        var groupsData = await GraphService.GetGroups(graphClient, HttpContext);
        foreach (var group in groupsData)
        {
          data.Add(new GroupListItemViewModel
          {
            Key = group.Id,
            Description = group.Description,
            GroupType = String.Join(" ", group.GroupTypes),
            Name = group.DisplayName,
            MailNickname = group.MailNickname,
            Thumbnail = "",
            Visibility = group.Visibility
          });
        }
      }
      catch (ServiceException e)
      {
        switch (e.Error.Code)
        {
          case "Authorization_RequestDenied":
            return new RedirectResult("/Account/PermissionsRequired");
          default:
            //return JsonConvert.SerializeObject(new { Message = "An unknown error has occurred." }, Formatting.Indented);
            var x = e.ToString();
            break;
        }
      }

      System.Diagnostics.Debug.WriteLine(_msalLog.GetLog());

      return View(data);
    }

  }
}