using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using WebApp_Security.Models;

namespace WebApp_Security.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Credential.UserName =="milad" && Credential.Password=="nematpour")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,"milad"),
                    new Claim(ClaimTypes.Email,"a@b.com"),
                    new Claim("Department","HR"),
                    new Claim("Admin","True"),
                    new Claim("Manager","True"),
                    new Claim("EmployeementDate","2022-01-01")
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrinciple = new ClaimsPrincipal(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync("MyCookieAuth", claimsPrinciple,authProperties);

                return RedirectToPage("/Index");
            }
            return Page();
        }
    }

    
}
