
using core.Models;
using core.Models.AccountModels;
using core.Models.AccountModels.ViewModels;
using core.Models.AdminAccountModels;
using infrastructure.AddionalClasses;
using infrastructure.Repositories;
using infrastructure.Repositories.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Fitness_management_system.Controllers
{
    [Route("Account")]
    [Authorize(Roles = "Administration, Member")]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly IHostingEnvironment _iHosting;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITrainerRepository _trainerRepository;
        private readonly AppDbContext _context;
        private readonly IEmailService _service;
        private readonly ILogger _logger;

        public AccountController(SignInManager<ApplicationUserModel> signInManager, UserManager<ApplicationUserModel> userManager, IHostingEnvironment iHosting, RoleManager<IdentityRole> roleManager, ITrainerRepository trainerRepository, AppDbContext context, IEmailService service, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _iHosting = iHosting;
            _roleManager = roleManager;
            _trainerRepository = trainerRepository;
            _context = context;
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        [Route("Login")]
        [AllowAnonymous]
        public IActionResult Login(string? ReturnUrl)
        {
            _logger.LogInformation($"login page visited at {DateTime.Now.ToLongTimeString()}");

            if (!string.IsNullOrEmpty(ReturnUrl)) ViewBag.ReturnUrl = ReturnUrl;

            return View();
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User having username:{model.Username} logged in at {DateTime.Now.ToLongTimeString()}");

                    if (!string.IsNullOrEmpty(model.ReturnUrl)) return LocalRedirect(model.ReturnUrl);
                    else return RedirectToAction("dashboard", "dashboard");
                }
                else
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            return View(model); ;
        }

        [HttpPost]
        [Route("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation($"User logged out at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("Login", "Account");


        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Register")]
        public IActionResult Register()
        {
            _logger.LogInformation($"Register page visited at {DateTime.Now.ToLongTimeString()}");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserModel user = new() { UserName = model.Email, Email = model.Email, Name = model.Name, Gender = model.Gender, CreatedDate = DateTime.Now, LastUpdatedDate = DateTime.Now, AvailabilityStatus = AvailabilityStatusEnum.InActive };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Member");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    _logger.LogInformation($"User registered with username: {model.Email} at {DateTime.Now.ToLongTimeString()}");

                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        [Route("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword(bool isEmailSent = false)
        {
            _logger.LogCritical($"Forgot password page visited at {DateTime.Now.ToLongTimeString()}");

            ViewData["isEmailSent"] = isEmailSent;

            return View();
        }

        [HttpPost]
        [Route("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewData["isEmailSent"] = false;

            if (ModelState.IsValid)
            {
                ApplicationUserModel user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    UserEmailOptions emailOptions = new UserEmailOptions()
                    {
                        Email = new List<string>() { model.Email },
                        KeyValuePairs = new List<KeyValuePair<string, string>>()
                    {
                        new ("{{Username}}",model.Email )
                    }
                    };
                    await _service.SendTestEmail(emailOptions);

                    _logger.LogCritical($"Email sent to username:{model.Email} for resetting password at {DateTime.Now.ToLongTimeString()}");

                }

                return RedirectToAction("ForgotPassword", new { isEmailSent = true });
            }

            return View(model);
        }

        [AllowAnonymous]
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IEnumerable<IdentityRole> GetAllRoles()
        {
            return _roleManager.Roles.ToList();
        }

        [Route("manage-roles")]
        [Authorize(Roles = "Administration")]
        public IActionResult ManageRoles()
        {
            _logger.LogInformation($"Manage Roles page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            IEnumerable<IdentityRole> roles = GetAllRoles();
            RoleViewModel model = new()
            {
                Roles = roles
            };

            return View(model);
        }

        [HttpGet]
        [Route("edit-role")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> EditRole(string roleId)
        {
            _logger.LogInformation($"Edit Role page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            IdentityRole role = await _roleManager.FindByIdAsync(roleId) ?? throw new Exception($"No Role found having ID: {roleId}");

            RoleViewModel model = new()
            {
                ID = role.Id,
                RoleName = role.Name
            };

            return View(model);
        }

        [HttpPost]
        [Route("edit-role")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(model.ID) ?? throw new Exception($"No Role found having ID: {model.ID}");

            role.Name = model.RoleName;
            await _roleManager.UpdateAsync(role);

            _logger.LogInformation($"Role having ID:{model.ID} edited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("ManageRoles");
        }

        [HttpPost]
        [Route("add-role")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new() { Name = model.RoleName.Trim() };
                IdentityResult result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                else
                {
                    _logger.LogInformation($"New Role added having name: {model.RoleName} by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");
                }
            }

            model.Roles = GetAllRoles();

            return View("./ManageRoles", model);
        }

        [HttpGet]
        [Route("delete-role")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId) ?? throw new Exception("Role not found");

            IdentityResult success = await _roleManager.DeleteAsync(role);

            if (success.Succeeded)
            {
                _logger.LogInformation($"Role deleted having ID: {roleId} by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                return RedirectToAction("ManageRoles");
            }
            throw new Exception("An Error occured; Unable to delete Role");
        }

        [HttpGet]
        [Route("settings")]
        public async Task<IActionResult> Settings()
        {
            _logger.LogInformation($"Setting page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            ApplicationUserModel user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                SettingsViewModel model = new()
                {
                    ID = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Gender = user.Gender,
                    AccountStatus = user.AvailabilityStatus,
                    ApplyDate = user.CreatedDate,
                    LastUpdatedDate = user.LastUpdatedDate,
                    JoiningDate = user.JoiningDate,
                    PhotoPath = user.PhotoPath,
                };

                return View(model);
            }

            return View();
        }

        [HttpPost]
        [Route("settings")]
        public async Task<IActionResult> UpdateAccountSettings(SettingsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string? uniqueFilename = null;

                    if (model.Photo != null)
                    {
                        string uploadFolder = Path.Combine(_iHosting.WebRootPath, "images/members");
                        uniqueFilename = $"{Guid.NewGuid()}_{model.Photo.FileName}";
                        string filepath = Path.Combine(uploadFolder, uniqueFilename);
                        model.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
                    }

                    ApplicationUserModel user = await _userManager.GetUserAsync(User);

                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Gender = model.Gender;
                    user.PhotoPath = !string.IsNullOrEmpty(uniqueFilename) ? uniqueFilename : model.PhotoPath;
                    user.LastUpdatedDate = DateTime.Now;

                    IdentityResult success = await _userManager.UpdateAsync(user);

                    if (success.Succeeded)
                    {
                        _logger.LogInformation($"User profile setting updated by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                        await _signInManager.SignOutAsync();

                        return RedirectToAction("Settings");
                    }
                    else throw new Exception("error occured");
                }

                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        [Route("Change-Password")]
        public IActionResult ChangePassword(string id)
        {
            _logger.LogInformation($"Change-Password page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return View();
        }

        [HttpPost]
        [Route("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUserModel user = await _userManager.GetUserAsync(User) ?? throw new Exception("You are not Logged In");

                if (model.CurrentPassword == model.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "New Password can not be the Current Password");
                    return View();
                }

                IdentityResult result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    _logger.LogCritical($"User passwprd updated by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                    await _signInManager.SignOutAsync();

                    return RedirectToAction("settings");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View();
        }

    }
}
