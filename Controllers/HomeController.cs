using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dotnet_projektuppgift.Models;

namespace dotnet_projektuppgift.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}