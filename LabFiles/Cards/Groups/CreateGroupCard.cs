    private AdaptiveCard CreateGroupCard(Models.GroupModel group)
    {
      AdaptiveCard groupCard = new AdaptiveCard()
      {
        Type = "AdaptiveCard",
        Version = "1.0"
      };

      AdaptiveContainer infoContainer = new AdaptiveContainer();
      AdaptiveColumnSet infoColSet = new AdaptiveColumnSet();

      bool noPic = String.IsNullOrEmpty(group.Thumbnail);

      if (!noPic)
      {
        AdaptiveColumn picCol = new AdaptiveColumn() { Width = AdaptiveColumnWidth.Auto };
        picCol.Items.Add(new AdaptiveImage() { Url = new Uri(group.Thumbnail), Size = AdaptiveImageSize.Small, Style = AdaptiveImageStyle.Default });
        infoColSet.Columns.Add(picCol);
      }

      AdaptiveColumn txtCol = new AdaptiveColumn() { Width = AdaptiveColumnWidth.Stretch };
      var titleBlock = new AdaptiveTextBlock() { Text = NullSafeString(group.Name), Weight = AdaptiveTextWeight.Bolder };
      if (noPic) { titleBlock.Size = AdaptiveTextSize.Large; }
      txtCol.Items.Add(titleBlock);

      txtCol.Items.Add(new AdaptiveTextBlock() { Text = NullSafeString(group.Description), Spacing = AdaptiveSpacing.None, IsSubtle = true });
      infoColSet.Columns.Add(txtCol);
      infoContainer.Items.Add(infoColSet);

      groupCard.Body.Add(infoContainer);

      AdaptiveContainer factContainer = new AdaptiveContainer();
      AdaptiveFactSet factSet = new AdaptiveFactSet();

      if (!String.IsNullOrEmpty(group.Classification))
      {
        factSet.Facts.Add(new AdaptiveFact()
        {
          Title = "Classification",
          Value = group.Classification
        });
      }
      if (!String.IsNullOrEmpty(group.Visibility))
      {
        factSet.Facts.Add(new AdaptiveFact()
        {
          Title = "Visibility",
          Value = group.Visibility
        });
      }

      if (!String.IsNullOrEmpty(group.GroupType))
      {
        factSet.Facts.Add(new AdaptiveFact()
        {
          Title = "Type",
          Value = NullSafeString(group.GroupType)
        });
      }

      if (group.CreatedDateTime.HasValue)
      {
        factSet.Facts.Add(new AdaptiveFact()
        {
          Title = "Created",
          Value = $"{{{{DATE({group.CreatedDateTime.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")},SHORT)}}}}"
        });

      }

      if (!String.IsNullOrEmpty(group.Policy) && group.RenewedDateTime.HasValue)
      {

        factSet.Facts.Add(new AdaptiveFact()
        {
          Title = "Policy",
          Value = NullSafeString(group.Policy)
        });
        factSet.Facts.Add(new AdaptiveFact()
        {
          Title = "Renewed",
          Value = $"{{{{DATE({group.RenewedDateTime.Value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ssZ")},SHORT)}}}}"
        });
      }

      factContainer.Items.Add(factSet);
      groupCard.Body.Add(factContainer);

      return groupCard;
    }
