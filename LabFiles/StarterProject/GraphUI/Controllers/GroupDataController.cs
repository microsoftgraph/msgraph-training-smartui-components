using System;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using GroupsReact.Helpers;
using GroupsReact.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Graph;

namespace GroupsReact.Controllers
{
  [Produces("application/json")]
  [Route("api/Group")]
  public class GroupDataController : ControllerBase
  {
    private readonly IGraphSdkHelper _graphSdkHelper;
    private MSALLogCallback _msalLog;

    public GroupDataController(IGraphSdkHelper graphSdkHelper, IMemoryCache memoryCache, MSALLogCallback msalLog)
      : base(memoryCache)
    {
      _graphSdkHelper = graphSdkHelper;
      _msalLog = msalLog;
    }

    [Route("Photo")]
    public async Task<ActionResult> Photo(string id, string userId)
    {
      // Initialize the GraphServiceClient.
      var graphClient = _graphSdkHelper.GetAuthenticatedClient(userId);

      string pic = default(string);
      try
      {
        pic = await GraphService.GetGroupPictureBase64(graphClient, id);
      }
      catch (ServiceException e)
      {
        return Json(new { Message = "An unknown error has occurred." });
      }

      System.Diagnostics.Debug.WriteLine(_msalLog.GetLog());

      return Json(new { id = id, photoUrl = pic });
    }


    [Route("Details")]
    public async Task<ActionResult> Details(string id, string userId)
    {
      // Initialize the GraphServiceClient.
      var graphClient = _graphSdkHelper.GetAuthenticatedClient(userId);

      GroupModel details = null;
      try
      {
        var group = await GraphService.GetGroupDetailsAsync(graphClient, id);
        var pic = await GraphService.GetGroupPictureBase64(graphClient, id);
        details = new GroupModel
        {
          Id = group.Id,
          Classification = group.Classification,
          CreatedDateTime = group.CreatedDateTime ?? null,
          RenewedDateTime = group.RenewedDateTime ?? null,
          Description = group.Description,
          GroupType = String.Join(' ', group.GroupTypes),
          Mail = group.Mail,
          Name = group.DisplayName,
          Visibility = group.Visibility,
          Thumbnail = pic
        };

        if (details.GroupType == "Unified")
        {
          var policies = await GraphService.GetGroupPolicyAsync(graphClient, id);
          var policy = policies.FirstOrDefault();
          if (policy != null)
          {
            details.Policy = $"{policy.GroupLifetimeInDays} Day expiration";
          }

          var drive = await GraphService.GetGroupDriveAsync(graphClient, id);
          details.DriveWebUrl = drive.WebUrl;
          var driveItems = await GraphService.GetDriveRecentItemsAsync(graphClient, drive.Id);

          if (driveItems.Count == 1)
          {
            var graphDriveItem = driveItems[0];
            var thumbnailUrl = await GraphService.GetDriveItemThumbnail(graphClient, drive.Id, graphDriveItem.Id);
            var driveItem = new Models.DriveItem(graphDriveItem);
            driveItem.ThumbnailUrl = thumbnailUrl;
            details.DriveRecentItems.Add(driveItem);
          }
          if (driveItems.Count > 1)
          {
            foreach (var item in driveItems)
            {
              details.DriveRecentItems.Add(new Models.DriveItem(item));
            }
          }

          var convo = await GraphService.GetGroupLatestConversationAsync(graphClient, id);
          details.LatestConversation = new Models.Conversation
          {
            Topic = convo.Topic,
            LastDeliveredDateTime = convo.LastDeliveredDateTime,
          };
          details.LatestConversation.UniqueSenders.AddRange(convo.UniqueSenders);
        }

        details.InfoCard = CreateGroupCard(details);
      }
      catch (ServiceException e)
      {
        return Json(new { Message = "An unknown error has occurred." });
      }

      System.Diagnostics.Debug.WriteLine(_msalLog.GetLog());

      return Json(details);
    }

    private AdaptiveCard CreateGroupCard(Models.GroupModel group)
    {
      AdaptiveCard groupCard = new AdaptiveCard()
      {
        Type = "AdaptiveCard",
        Version = "1.0"
      };

      return groupCard
    }

    private string NullSafeString(string value)
    {
      return String.IsNullOrEmpty(value) ? "" : value;
    }

  }
}