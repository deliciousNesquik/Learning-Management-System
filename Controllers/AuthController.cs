using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using LMS.Services;

namespace LMS.Controllers;

[ApiController]
[Route("account")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string login, [FromForm] string password)
    {
        var principal = await authService.AuthenticateUser(login, password);

        if (principal is { ClaimsPrincipal: null, ErrorMessage: not null })
            return Redirect($"/login?error={principal.ErrorMessage}");
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal.ClaimsPrincipal,
            new AuthenticationProperties { IsPersistent = true });

        return Redirect("/");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}