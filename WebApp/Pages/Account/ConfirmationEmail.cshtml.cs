using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class ConfirmationEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public string Message { get; set; }
        public ConfirmationEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet(string userId, string token)
        {
            var user= await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                Message = "Failed to validate email.";
                return Page();
            }


            IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user,token);
        
            if (identityResult.Succeeded)
            {
                this.Message = "Email addess is successfully confirm, now you can access the user";
                return Page();
            }
            return Page();

        }
    }
}
