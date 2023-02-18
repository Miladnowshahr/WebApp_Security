using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly SignInManager<IdentityUser> _signInManager;
        public LoginTwoFactorModel(UserManager<IdentityUser> userManager, IEmailService emailService, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _emailService = emailService;
            _signInManager = signInManager;
            MFA = new EmailMFA();
        }

        [BindProperty]
        public EmailMFA MFA { get; set; }

        public async Task<IActionResult> OnGetAsync(string email,bool rememberMe)
        {

            IdentityUser? identityUser = await _userManager.FindByEmailAsync(email);

            this.MFA.SecurityCode=string.Empty;
            this.MFA.RememberMe = rememberMe;
            var securityCode = await _userManager.GenerateTwoFactorTokenAsync(identityUser, "Email");

            await _emailService.SendAsync("", identityUser.Email, "Two factor Login", $"Your security code is {securityCode}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            
            var result = await _signInManager.TwoFactorSignInAsync("Email", MFA.SecurityCode, MFA.RememberMe,false);
            if (result.Succeeded)
                return RedirectToPage("/index");

            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("Login2FA", "You are locked out") ;
                }
                else
                {

                }
                return Page();
            }
            


        }
    }
    public class EmailMFA
    {
        [Required]
        [Display(Name ="Security Code")]
        public string SecurityCode { get; set; }
        public bool RememberMe { get; set; }    
    }
}
