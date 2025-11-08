using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using UrlShortenerService.MVC.Data.Entities.Identities;
using UrlShortenerService.MVC.ViewModels.Identities;

namespace UrlShortenerService.MVC.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;

        public AuthenticationController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
        }

        public IActionResult Index() => View();

        // ================= LOGIN GET =================
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            var errorMessage = TempData["ErrorMessage"] as string;
            if (!string.IsNullOrEmpty(errorMessage))
                ModelState.AddModelError(string.Empty, errorMessage);

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var loginVM = new LoginVM
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                ReturnUrl = returnUrl
            };

            return View(loginVM);
        }

        // ================= LOGIN POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            loginVM.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (user != null)
                {
                    if (!_userManager.Options.SignIn.RequireConfirmedAccount ||
                        await _userManager.IsEmailConfirmedAsync(user))
                    {
                        var result = await _signInManager.PasswordSignInAsync(
                            loginVM.Email,
                            loginVM.Password,
                            loginVM.RememberMe,
                            lockoutOnFailure: false);

                        if (result.Succeeded)
                        {
                            return LocalRedirect(loginVM.ReturnUrl ?? "/");
                        }
                    }

                    return RedirectToAction(nameof(RegisterConfirmation), new
                    {
                        email = loginVM.Email,
                        returnUrl = loginVM.ReturnUrl
                    });
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            TempData["ErrorMessage"] = "Unsuccessful Login!";
            return View(loginVM);
        }

        // ================= REGISTER GET =================
        public async Task<IActionResult> Register(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            var registerVM = new RegisterVM
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
            };

            return View(registerVM);
        }


        // ================= REGISTER POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            registerVM.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser(); // no FullName, no Address

                await _userStore.SetUserNameAsync(user, registerVM.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, registerVM.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    return RedirectToAction(nameof(RegisterConfirmation), new
                    {
                        email = registerVM.Email,
                        returnUrl = registerVM.ReturnUrl
                    });
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);
        }


        // ================= EMAIL CONFIRMATION SCREEN =================
        public async Task<IActionResult> RegisterConfirmation(string email, string? returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound($"Unable to load user with email '{email}'.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var vm = new RegisterConfirmationVM
            {
                Email = email,
                UserId = userId,
                Code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code)),
                ReturnUrl = returnUrl
            };

            return View(vm);
        }


        // ================= EMAIL CONFIRMATION EXECUTE =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterConfirmation(RegisterConfirmationVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.FindByIdAsync(vm.UserId);
            if (user == null)
                return NotFound($"Unable to load user with ID '{vm.UserId}'.");

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(vm.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            TempData["StatusMessage"] = result.Succeeded ?
                "Thank you for confirming your email." :
                "Email confirmation failed.";

            return LocalRedirect(vm.ReturnUrl);
        }

        // ================= PRIVATE METHODS =================
        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("Identity requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
