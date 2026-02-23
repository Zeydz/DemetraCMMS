using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using dotnet_projektuppgift.Models;
using dotnet_projektuppgift.ViewModels;

namespace dotnet_projektuppgift.Controllers;

public class AccountController : Controller
{
    /*SignInManager handles cookie-based authentication*/
    private readonly SignInManager<ApplicationUser> _signInManager;

    /* UserManager handles user records in the database:*/
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    

    /* LOGIN */
    /*GET /Account/Login*/
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        /* Check if user is already logged in */
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /* POST /Account/Login*/
    /* Called when the login form is submitted.*/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        /* Check if the form is valid*/
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        /* PasswodSignInAsync checks user by email, verifies password against hashed password and creates cookie.*/
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false 
        );

        if (result.Succeeded)
        {
             /* Return back to Dashboard or safe URL*/
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        /* Login failed. Just a generic error message.*/
        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    /* LOGOUT */
    /*POST /Account/Logout*/
    
    /* Logsout user. Only logout logged in user by removing Cookie from the browser. */
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
       /* Remove cookie and redirect user to Home*/
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    /* REGISTER */
    /*GET /Account/Register*/
    [HttpGet]
    public IActionResult Register()
    {
        /* Redirect to Dashboard if user is authenticated*/
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View("Login");
    }

     /*POST /Account/Register*/
     /*Called when the register form is submitted.*/
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        /* Check if form is valid*/
        if (!ModelState.IsValid)
        {
            return View("Login", model);
        }
        /* Create new ApplicationUser object*/
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
        };

        /* CreateAsync hashes the password and stores it */
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            /*Assign the default "User" role to every new registration.
            This role can view and submit tickets but not access admin pages.*/
            await _userManager.AddToRoleAsync(user, "User");

            /* Automatically sign in the user after registering. isPersistent decides whether the cookie
             should get removed or not after closing browser*/
            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Dashboard");
        }

        /*If CreateAsync failed (for example, email already taken, password too weak),
        add all the error messages to ModelState so they show in the view for the user.*/
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View("Login", model);
    }

    /* ACCESS DENIED*/

    /*GET /Account/AccessDenied*/
    /* Redirect here when a logged-in user tries to access a page they don't have perimssion for.
     ("User" role visiting an admin page)*/
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
