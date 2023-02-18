using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorWithAuthenticatorModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        [BindProperty]
        public AuthenticatorMFA AuthenticatorMFA { get; set; }
        public LoginTwoFactorWithAuthenticatorModel(SignInManager<IdentityUser> signInManager)
        {
            AuthenticatorMFA = new AuthenticatorMFA();
            _signInManager = signInManager;
        }
        public void OnGet(bool rememberMe)
        {
            AuthenticatorMFA.SecurityCode = string.Empty;
            AuthenticatorMFA.RememberMe = rememberMe;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid is false) return Page();

           await _signInManager.TwoFactorAuthenticatorSignInAsync(AuthenticatorMFA.SecurityCode, AuthenticatorMFA.RememberMe, false);
        }
    }

    public class AuthenticatorMFA
    {
        [Required]
        [Display(Name ="Code")]
        public string SecurityCode { get; set; }

        public bool RememberMe { get; set; }
    }
}
