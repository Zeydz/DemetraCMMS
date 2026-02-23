using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_projektuppgift.Controllers;

/* Requires login*/
[Authorize] 
public class DashboardController : Controller
{
    /* This is where users land when successfull login*/
    public IActionResult Index()
    {
        return View();
    }
}