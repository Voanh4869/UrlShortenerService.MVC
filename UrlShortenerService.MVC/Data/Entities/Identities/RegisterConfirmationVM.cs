using Microsoft.AspNetCore.Mvc;

namespace UrlShortenerService.MVC.ViewModels.Identities
{
    [Bind("Email, ReturnUrl, Code, UserId")]
    public class RegisterConfirmationVM
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string? EmailConfirmationUrl { get; set; }

        public string ReturnUrl { get; set; } = "~/";
        public string? Code { get; set; }
        public string? UserId { get; set; }

    }
}
