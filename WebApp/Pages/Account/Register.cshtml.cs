using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;
        public RegisterModel(UserManager<IdentityUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public RegisterViewModel Register { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();


            //create the user

            var user = new IdentityUser
            {
                Email = Register.Email,
                UserName = Register.Email,
            };
            var identityResult = await _userManager.CreateAsync(user, Register.Password);

            if (identityResult.Succeeded)
            {
                var confirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = Url.PageLink(pageName: "/Account/EmailConfirmation", values: new { UserId = user.Id, Token = confirmation });

                await _emailService.SendAsync("miladnowshahr@gmail.com", user.Email, "Please confirm your email",
                    $"Please click on this link to confirm your email address {confirmationLink}");


                return RedirectToPage("/account/login");
            }
            else
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return Page();
            }

        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email addrss")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
