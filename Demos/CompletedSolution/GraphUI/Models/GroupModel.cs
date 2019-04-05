using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GroupsReact.Models
{
  public class GroupModel
  {
    [JsonProperty("key")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string GroupType { get; set; }
    public string Mail { get; set; }
    public string Thumbnail { get; set; }
    public string Visibility { get; set; }
    public string Classification { get; set; }
    public DateTimeOffset? CreatedDateTime { get; set; }
    public string Policy { get; set; }
    public DateTimeOffset? RenewedDateTime { get; set; }

    public string DriveWebUrl { get; set; }
    public string MailboxWebUrl
    {
      get
      {
        return $"https://outlook.office.com/owa/?path=/group/{Mail}/mail";
      }
    }

    public AdaptiveCards.AdaptiveCard InfoCard { get; set; }
    public List<DriveItem> DriveRecentItems { get; set; }
    public Conversation LatestConversation { get; set; }

    public GroupModel()
    {
      Mail = "";
      Thumbnail = "";
      Visibility = "";
      DriveWebUrl = "";
      DriveRecentItems = new List<DriveItem>();
    }
  }

  public class DriveItem
  {
    public string Title { get; set; }
    public string FileType { get; set; }
    public string WebUrl { get; set; }
    public string ThumbnailUrl { get; set; }

    public DriveItem() { }

    public DriveItem(Microsoft.Graph.DriveItem graphItem)
    {
      var fileParts = graphItem.Name.Split(".");
      if (fileParts.Length > 1)
      {
        Title = fileParts[0];
        FileType = fileParts[1];
      }
      else
      {
        Title = graphItem.Name;
        if (graphItem.Folder != null) { FileType = "folder"; }
        if (graphItem.Package !=null) { FileType = "onetoc"; }
      }
      WebUrl = graphItem.WebUrl;
    }
  }

  public class Conversation
  {
    public string Topic { get; set; }
    public DateTimeOffset? LastDeliveredDateTime { get; set; }
    public List<string> UniqueSenders { get; set; }

    public string LastDelivered
    {
      get
      {
        return LastDeliveredDateTime.Value.ToString("ddd, MMM d, yyyy");
      }
    }
    public Conversation()
    {
      UniqueSenders = new List<string>();
    }
  }
}

