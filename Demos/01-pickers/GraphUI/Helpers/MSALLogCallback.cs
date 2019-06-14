/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Text;

namespace GroupsReact.Helpers
{
  public class MSALLogCallback
  {
    private static StringBuilder log = new StringBuilder();
    public string GetLog()
    {
      var result = log.ToString();
      log.Clear();
      return result;
    }

    public void Log(LogLevel level, string message, bool containsPii)
    {
      if (!containsPii)
      {
        string requestId = Activity.Current?.Id;
        log.AppendLine($"{requestId} - {level.ToString()} - {message}");
      }
    }

  }
}
