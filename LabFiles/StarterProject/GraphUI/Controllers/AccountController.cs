using System;
using GroupsReact.Extensions;
using GroupsReact.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace GroupsReact.Controllers
{
  [Route("[controller]/[action]")]
  public class AccountController : ControllerBase
  {
    private AzureAdOptions azureAdOptions;

    public AccountController(IOptions<AzureAdOptions> options, IMemoryCache memoryCache)
      :base(memoryCache)
    {
      azureAdOptions = options.Value;
    }

    [HttpGet]
    public IActionResult SignIn()
    {
      var redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
      return Challenge(
          new AuthenticationProperties { RedirectUri = redirectUrl },
          OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult SignOut()
    {
      var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
      return SignOut(
          new AuthenticationProperties { RedirectUri = callbackUrl },
          CookieAuthenticationDefaults.AuthenticationScheme,
          OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult SignedOut()
    {
      if (User.Identity.IsAuthenticated)
      {
        // Redirect to home page if the user is authenticated.
        return RedirectToAction(nameof(HomeController.Index), "Home");
      }

      return View();
    }


    public ActionResult ConnectAADTenant()
    {
      // Redirect the admin to grant your app permissions
      string url = String.Format(GraphAuthProvider.AdminConsentFormat, azureAdOptions.TenantId, azureAdOptions.ClientId, "whatever_you_want", azureAdOptions.BaseUrl + "/Account/AADTenantConnected");
      return new RedirectResult(url);
    }

    // When the admin completes granting the permissions, they will be redirected here.
    [Authorize]
    public void AADTenantConnected(string state, string tenant, string admin_consent, string error, string error_description)
    {
      if (error != null)
      {
        // If the admin did not grant permissions, ask them to do so again
        Response.Redirect("/Account/PermissionsRequired?error=" + error_description);
        return;
      }

      // Note: Here the state parameter will contain whatever you passed in the outgoing request. You can
      // use this state to encode any information that you wish to track during execution of this request.

      Response.Redirect("/Groups");
    }

    public ActionResult PermissionsRequired(string error)
    {
      // Get user's id for token cache.
      var identifier = User.FindFirst(Startup.ObjectIdentifierType)?.Value;
      base.CopyUserModelToViewData(identifier);

      ViewBag.Error = error;
      return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
      return View();
    }
  }
}
