using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OTSISampleTemplate.Services.Abstractions;
using OTSISampleTemplate.Services.Models;
using OTSISampleTemplate.Web.Models;

namespace OTSISampleTemplate.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthService authService, ILogger<AccountController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // If user is already authenticated, redirect to Home
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var authRequest = new LoginRequest
        {
            Username = model.Username,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var authResult = await _authService.AuthenticateAsync(authRequest);

        if (!authResult.Success || string.IsNullOrEmpty(authResult.Token))
        {
            model.ErrorMessage = authResult.Message;
            return View(model);
        }

        // Store JWT in Secure HTTP-Only Cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(2)
        };

        Response.Cookies.Append("access_token", authResult.Token, cookieOptions);

        _logger.LogInformation("User {Username} successfully logged in with JWT token.", model.Username);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ApiLogin([FromBody] LoginRequest request)
    {
        var result = await _authService.AuthenticateAsync(request);
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpGet]
    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("access_token");
        return RedirectToAction("Login", "Account");
    }
}
