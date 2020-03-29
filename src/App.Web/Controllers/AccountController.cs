using App.Domain.Models.Identity;
using App.Helper;
using App.Services.Core.Interfaces;
using App.Services.Identity;
using App.Services.Messages;
using App.Web.Models.ViewModels.Hosting;
using App.Web.Models.ViewModels.Identity;
using App.Web.Models.ViewModels.Identity.AccountViewModels;
using App.Web.Utils;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ConfigHelper _config;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMailSenderService mailSender;
        private readonly ISmsSender smsSender;
        private readonly MailingHelper _mailingHelper;
        private readonly ViewRender view;
        //private readonly ILogger logger;
        private readonly HostConfiguration hostConfiguration;
        private readonly IIdentityServerInteractionService interaction;
        private readonly IPersistedGrantService _persistedGrantService;
        private readonly IUserProfileService _profile;
        private readonly ICandidateInfoService _candidate;
        private readonly ISrfRequestService _srf;
        private readonly ISrfEscalationRequestService _escalation;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMailSenderService mailSender,
            ViewRender view,
            IIdentityServerInteractionService interaction,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            IOptions<HostConfiguration> hostConfiguration,
            ConfigHelper config,
            ICandidateInfoService candidate,
            MailingHelper mailingHelper,
            IUserProfileService profile,
            ISrfRequestService srf,
            ISrfEscalationRequestService escalation,
            IPersistedGrantService persistedGrantService)

        {
            this.view = view;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mailSender = mailSender;
            this.smsSender = smsSender;
            this.hostConfiguration = hostConfiguration.Value;
            this.interaction = interaction;
            //this.logger = loggerFactory.CreateLogger<AccountController>();
            _config = config;
            _mailingHelper = mailingHelper;
            _persistedGrantService = persistedGrantService;
            _profile = profile;
            _candidate = candidate;
            _srf = srf;
            _escalation = escalation;
        }

        /// <summary>
        /// GET: /Account/Login
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.IsAuthenticated())
            {
                return RedirectToLocal(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: /Account/Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser();
            user = await userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var UserProfile = _profile.GetByUserId(user.Id);
                if (UserProfile == null)
                {
                    ModelState.AddModelError(string.Empty, "Your account is not registered");
                    return View(model);
                }
                else
                {
                    // Checking Srf Status
                    if (UserProfile.Roles.Trim().ToLower().Equals("Contractor".Trim().ToLower()))
                    {
                        var CheckCandidate = _candidate.GetAll().Where(x => x.AccountId.Equals(UserProfile.Id)).FirstOrDefault();
                        if (CheckCandidate == null)
                        {
                            ModelState.AddModelError(string.Empty, "You are not registered in Vacancy request.");
                            return View(model);
                        }
                        else
                        {
                            var SrfCheck = _srf.GetAll().Where(x => x.CandidateId.Equals(CheckCandidate.Id)).FirstOrDefault();
                            if (SrfCheck == null)
                            {
                                ModelState.AddModelError(string.Empty, "You are not registered in SRF request.");
                                return View(model);
                            }

                        }
                    }
                }

                if (UserProfile.IsActive == false)
                {
                    ModelState.AddModelError(string.Empty, "you account is closed by system");
                    return View(model);
                }
                else if (UserProfile.IsBlacklist == true)
                {
                    ModelState.AddModelError(string.Empty, "you account is blacklist by system");
                    return View(model);
                }
                else if (UserProfile.IsTerminate == true)
                {
                    ModelState.AddModelError(string.Empty, "you account is terminate by system");
                    return View(model);
                }
                else
                {
                    if (!await userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError("", "You must verify your account before proceeding. "
                                         + "Please check your email & follow the instruction.");
                        return View(model);
                    }

                    var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(SendCode), new { returnUrl = returnUrl, RememberMe = model.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        return View("Lockout");
                    }
                    else
                    {
                        await userManager.AccessFailedAsync(user);
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return View(model);
                    }
                }
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

        }

        /// <summary>
        /// GET: /Account/Register
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: /Account/Register
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                UserProfile = new UserProfile()
                {
                    Name = model.Name,
                }

            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                await userManager.AddToRoleAsync(user, _config.GetConfig("user.default.role"));

                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail",
                    "Account",
                    new { userId = user.Id, code = code },
                    hostConfiguration.Protocol,
                    hostConfiguration.Name);

                var additionalData = new Dictionary<string, string>()
                {
                    { "CallbackUrl", callbackUrl },
                    { "Name", model.Name },
                    { "Email", model.Email },
                    { "Password", "***" }
                };

                var subject = "You have been registered, please confirm your account";

                var email = _mailingHelper.CreateEmail(_config.GetConfig("smtp.from.email"),
                    model.Email,
                    subject,
                    "Emails/RegisterUser",
                    user,
                    additionalData,
                    null);

                var emailResult = _mailingHelper.SendEmail(email).Result;

                ViewData["EmailSent"] = emailResult.IsSent;

                return RedirectToLocal(returnUrl);
            }
            AddErrors(result);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            if (!User.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }

            var user = HttpContext.User.Identity.Name;
            var sub = HttpContext.User.Identity.GetSubjectId();

            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            await _persistedGrantService.RemoveAllGrantsAsync(sub, "client");

            await signInManager.SignOutAsync();



            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// POST: /Account/Logout
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {

            //return View("LoggedOut", vm);
            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    model.LogoutId = await interaction.CreateLogoutContextAsync();
                }

                string url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(idp, new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException)
                {
                }
            }

            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            //return View("LoggedOut", vm);
            return Redirect(logout?.PostLogoutRedirectUri);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            var user = HttpContext.User.Identity.Name;
            var sub = HttpContext.User.Identity.GetSubjectId();

            // delete authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            await _persistedGrantService.RemoveAllGrantsAsync(sub, "client");

            await signInManager.SignOutAsync();


            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// POST: /Account/ExternalLogin
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// GET: /Account/ExternalLoginCallback
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="remoteError"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {

                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        /// <summary>
        /// POST: /Account/ExternalLoginConfirmation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        /// <summary>
        /// GET: /Account/ConfirmEmail
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        /// <summary>
        /// GET: /Account/ForgotPassword
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// GET: /Account/ForgotPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null && await userManager.IsEmailConfirmedAsync(user))
            {
                var code = await userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword",
                    "Account",
                    new { code = code },
                    hostConfiguration.Protocol,
                    hostConfiguration.Name);

                var additionalData = new Dictionary<string, string>()
                {
                    { "CallbackUrl", callbackUrl },
                    { "Email", user.Email }
                };

                var subject = "Forgot Password e-servicebased.net";

                var email = _mailingHelper.CreateEmail(_config.GetConfig("smtp.from.email"),
                    user.Email,
                    subject,
                    "Emails/ForgotPassword",
                    user,
                    additionalData,
                    null);

                var emailResult = _mailingHelper.SendEmail(email).Result;

                ViewData["EmailSent"] = emailResult.IsSent;

                return View("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError("", "Email not registered or not confirmed.");

            return View(model);
        }

        /// <summary>
        /// GET: /Account/ForgotPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// GET: /Account/ResetPassword
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return View("Error");
            }
            return View();
        }

        /// <summary>
        /// POST: /Account/ResetPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                TempData["Message"] = "Email/User Not Found.";
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);

            if (result.Succeeded)
            {
                TempData["Message"] = "Your password has been reset.";
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }

            AddErrors(result);

            return View();
        }

        /// <summary>
        /// GET: /Account/ResetPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// GET: /Account/SendCode
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(dest => new SelectListItem { Text = dest, Value = dest }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            // Generate the token and send it
            var code = await userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;
            if (model.SelectedProvider == "Email")
            {
                await mailSender.SendEmailAsync(await userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await smsSender.SendSmsAsync(await userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {

                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Ok(true);

            if (!string.IsNullOrWhiteSpace(model.Id))
            {
                var userById = await userManager.FindByIdAsync(model.Id);
                if (userById != null && user != null && userById.Email == user.Email)
                    return Ok(true);
            }

            return Ok(false);
        }

        private void AddErrors(IdentityResult res)
        {
            foreach (IdentityError error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

    }
}
