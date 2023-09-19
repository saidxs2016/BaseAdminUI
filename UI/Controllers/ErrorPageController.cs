using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers;

public class ErrorPageController : Controller
{
    public IActionResult Error(string status, string details)
    {
        ViewBag.status = status;
        ViewBag.details = details;
        return View();
    }
}