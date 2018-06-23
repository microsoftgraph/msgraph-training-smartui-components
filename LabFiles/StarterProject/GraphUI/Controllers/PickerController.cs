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