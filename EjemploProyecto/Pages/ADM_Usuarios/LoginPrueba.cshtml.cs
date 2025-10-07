using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace EjemploProyecto.Pages.PRUEBA_Usuarios
{
    public class LoginPruebaModel : PageModel
    {
        [BindProperty] public string Username { get; set; }

        public IActionResult OnGet(string? expired)
        {
            ViewData["Expired"] = expired == "1";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, string.IsNullOrEmpty(Username) ? "usuario" : Username)
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/InconsistenciasM/Index");
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToPage("/PRUEBA_Usuarios/Login");
        }

    }
}
