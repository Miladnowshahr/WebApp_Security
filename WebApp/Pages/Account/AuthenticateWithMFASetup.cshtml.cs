using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebApp.Pages.Account
{
    [Authorize]
    public class AuthenticateWithMFASetupModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public SetupMFAViewModel MFAViewModel { get; set; }

        [BindProperty]
        public bool Succeeded { get; set; }
        public AuthenticateWithMFASetupModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            MFAViewModel.Key = key;
            MFAViewModel.QRCodeBytes = GenerateQRCodeBytes("My Web App", key, user.Email);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid is false)
                return Page();
            var user = await _userManager.GetUserAsync(User);
           if( await _userManager.VerifyTwoFactorTokenAsync(user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                MFAViewModel.SecurityCode))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
            }
            else
            {
                ModelState.AddModelError("AuthenticatorSetup", "Some went wrong with authenticator setup");
            }
            return Page();
        }
        private byte[] GenerateQRCodeBytes(string provider,string key,string email)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData =qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{email}?secret={key}&issuer={provider}",QRCodeGenerator.ECCLevel.Q);
            //QrCode Dosen't support Dot Net 7
            // var qrCode = new QrCode(qrCodeData);
            // var qrCodeImage = qrCode.GetGraphic(20);
            //return BitmapToBitArray(qrCodeImage);

            return new byte[] { };
        }

        private byte[] BitmapToBitArray(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream,ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

    public class SetupMFAViewModel
    {
        public string Key { get; set; }
        [Required]
        public string SecurityCode { get; set; }

        public byte[] QRCodeBytes { get; set; }
    }
}
