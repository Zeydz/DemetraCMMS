using System.Security.Claims;
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
    public async Task<IActionResult> Login(LoginViewModel model,
        string? returnUrl = null)
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
            var user =
                await _userManager.FindByNameAsync(model.Email);

            /* Add FullName as a claim, since ASP.NET doesn't know about it*/
            if (user != null && !string.IsNullOrEmpty(user.FullName))
            {
                var existingClaim =
                    (await _userManager.GetClaimsAsync(user))
                    .FirstOrDefault(claim =>
                        claim.Type == "FullName");

                if (existingClaim == null)
                {
                    await _userManager.AddClaimAsync(user,
                        new Claim("FullName", user.FullName));
                }
                else if (existingClaim.Value != user.FullName)
                {
                    /* Update if changed */
                    await _userManager.ReplaceClaimAsync(user,
                        existingClaim,
                        new Claim("FullName", user.FullName));
                }

                /* Refresh the sign-in cookie to update*/
                await _signInManager.RefreshSignInAsync(user);
            }


            /* Return back to Dashboard or safe URL*/
            if (!string.IsNullOrEmpty(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        /* Login failed. Just a generic error message.*/
        ModelState.AddModelError(string.Empty,
            "Invalid email or password.");
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
        return RedirectToAction("Index", "Public");
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
            FullName = model.Name
        };

        /* CreateAsync hashes the password and stores it */
        var result =
            await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            /*Assign the default "User" role to every new registration.
            This role can view and submit tickets but not access admin pages.*/
            await _userManager.AddToRoleAsync(user, "User");

            /* Store full name */
            await _userManager.AddClaimAsync(user,
                new Claim("FullName", user.FullName));


            /* Automatically sign in the user after registering. isPersistent decides whether the cookie
             should get removed or not after closing browser*/
            await _signInManager.SignInAsync(user,
                isPersistent: false);

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

    /* External login, Google*/
    /* I followed this tutorial: https://www.linkedin.com/pulse/add-sign-google-your-aspnet-mvc-app-step-by-step-tutorial-trupja-lvtie*/
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogin(string provider,
        string? returnUrl = null)
    {
        /*Request a redirect to the external login provider*/
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback),
            "Account",
            new { returnUrl });
        
        var properties = _signInManager
            .ConfigureExternalAuthenticationProperties(provider,
                redirectUrl);

        /* Redirect user to Google Login page*/
        return Challenge(properties, provider);
    }

    /*Callback after Google authentication*/
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(
        string? returnUrl = null,
        string? remoteError = null)
    {
        /*Set default return URL to Dashboard if not provided*/
        returnUrl = returnUrl ?? Url.Action("Index", "Dashboard");

        if (remoteError != null)
        {
            TempData["Error"] =
                $"Error from external provider: {remoteError}";
            return RedirectToAction(nameof(Login));
        }

        /*Get external login info*/
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            TempData["Error"] =
                "Error loading external login information.";
            return RedirectToAction(nameof(Login));
        }

        /*Sign in the user with external login provider*/
        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            /*User already exists and logged in successfully*/
            return LocalRedirect(returnUrl);
        }

        /*User doesn't exist, create new account*/
        /*Principal contains the claims from Google*/
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(email))
        {
            TempData["Error"] = "Email not available from Google.";
            return RedirectToAction(nameof(Login));
        }

        /*Create new user*/
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = name ?? email,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user);

        if (createResult.Succeeded)
        {
            /*Add external login info to user*/
            await _userManager.AddLoginAsync(user, info);

            /*Assign default role*/
            await _userManager.AddToRoleAsync(user, "User");

            /*Sign in the user*/
            await _signInManager.SignInAsync(user,
                isPersistent: false);

            return RedirectToAction("Index", "Dashboard");
        }

        /*Failed to create user*/
        foreach (var error in createResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        TempData["Error"] = "Could not create account.";
        return RedirectToAction(nameof(Login));
    }
}