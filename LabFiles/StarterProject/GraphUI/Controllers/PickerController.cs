/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroupsReact.Controllers
{
  public class PickerController : Controller
  {
    [Authorize]
    public IActionResult Index()
    {
      return View();
    }
  }
}