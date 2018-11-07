function launchOneDrivePicker() {
  var ClientID = "@Options.Value.ClientId";

  var odOptions = {
    clientId: ClientID,
    action: "query",
    multiSelect: false,
    advanced: {
      queryParameters: "select=id,name,size,file,folder,photo,@@microsoft.graph.downloadUrl",
      redirectUri: '@Options.Value.BaseUrl/OneDriveFilePickerCallback.html'
    },
    success: function (files) {
      var data = files;
      var fileName = data.value[0]["name"];
      var filePath = data.value[0]["@@microsoft.graph.downloadUrl"];

      document.getElementById('selectedFileName').innerHTML = '<strong>' + fileName + '</strong>';
      document.getElementById('selectedFileUrl').innerText = filePath.substr(0, filePath.indexOf('tempauth') + 15) + '...';
    },
    cancel: function () {
      /* cancel handler */
    },
    error: function (e) {
      /* error handler */
      alert(e);
    }
  };
  OneDrive.open(odOptions);
}

// PeoplePicker code
App.RenderPeoplePicker();