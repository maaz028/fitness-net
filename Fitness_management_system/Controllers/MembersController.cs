using core.Models;
using core.Models.AccountModels;
using core.Models.MemberModels;
using infrastructure.AddionalClasses;
using infrastructure.Repositories.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Fitness_management_system.Controllers
{
    [Route("Members")]
    public class MembersController : Controller
    {
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly IHostingEnvironment _iHosting;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITrainerRepository _trainerRepository;
        private readonly AppDbContext _context;

        private readonly ILogger _logger;

        public MembersController(SignInManager<ApplicationUserModel> signInManager, UserManager<ApplicationUserModel> userManager, IHostingEnvironment iHosting, RoleManager<IdentityRole> roleManager, ITrainerRepository trainerRepository, AppDbContext context,
            ILogger<MembersController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _iHosting = iHosting;
            _roleManager = roleManager;
            _trainerRepository = trainerRepository;
            _context = context;
            _logger = logger;
        }

        private static int selectedEntries = 5;

        [Route("available-members")]
        [Authorize(Roles = "Administration")]
        public IActionResult AvailableMembers(int? entries, bool isAccountStatusUpdated = false, bool isTrainerAssigned = false, int pageNumber = 1)
        {
            _logger.LogInformation($"Available members page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            int pageSize = (int)(entries == null ? MembersController.selectedEntries : entries);
            MembersController.selectedEntries = pageSize;

            int recSkip = (pageNumber - 1) * pageSize;

            IEnumerable<ApplicationUserModel> model = _userManager.Users.OrderByDescending(x => x.CreatedDate).Skip(recSkip).Take(pageSize).ToList();
            int totalCount = _userManager.Users.Count();
            Pagination Pager = new Pagination(pageSize, totalCount);
            int pages = Pager.GeneratePages();

            ViewBag.Pages = pages;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalItems = totalCount;
            ViewBag.IsAccountStatusUpdated = isAccountStatusUpdated;
            ViewBag.IsTrainerAssigned = isTrainerAssigned;
            ViewBag.SelectedEntries = pageSize;
            ViewBag.Partials = recSkip + model.Count();

            _logger.LogInformation($"Available members page number: {pageNumber} return {model.Count()} rows at {DateTime.Now.ToLongTimeString()}");


            return View(model);
        }

        [Route("member-details")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> MemberViewDetails(string id)
        {
            _logger.LogInformation($"member details page having ID: {id} visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            ApplicationUserModel user = await _userManager.FindByIdAsync(id) ?? throw new Exception($"No Member available having ID: {id}");

            MemberViewModel model = new()
            {
                MemberID = user.Id,
                MemberName = user.Name,
                MemberEmail = user.Email,
                MemberGender = user.Gender,
                AvailabilityStatus = user.AvailabilityStatus,
                ApplyDate = user.CreatedDate,
                JoiningDate = user.JoiningDate,
                LastUpdatedDate = user.LastUpdatedDate,
                PhotoPath = user.PhotoPath,

            };

            foreach (IdentityRole role in _roleManager.Roles.ToList())
            {
                bool isUserInRole = await _userManager.IsInRoleAsync(user, role.Name);

                if (isUserInRole)
                {
                    MemberRolesViewModel userRole = new()
                    {
                        ID = role.Id,
                        RoleName = role.Name,
                    };
                    model.Roles.Add(userRole);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("manage-member-roles")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> ManageMemberRoles(string MemberID)
        {
            _logger.LogInformation($"member roles page having ID: {MemberID} visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            ApplicationUserModel user = await _userManager.FindByIdAsync(MemberID) ?? throw new Exception($"No User found having id: {MemberID}");
            List<ManageMemberRolesViewModel> rolesViewModels = new();

            foreach (IdentityRole role in _roleManager.Roles.ToList())
            {
                bool result = await _userManager.IsInRoleAsync(user, role.Name);
                ManageMemberRolesViewModel assignedRoles = new();

                if (result)
                {
                    assignedRoles.MemberId = MemberID;
                    assignedRoles.RoleName = role.Name;
                    assignedRoles.RoleID = role.Id;
                    assignedRoles.IsSelected = true;
                }
                else
                {
                    assignedRoles.MemberId = MemberID;
                    assignedRoles.RoleName = role.Name;
                    assignedRoles.RoleID = role.Id;
                    assignedRoles.IsSelected = false;
                }

                rolesViewModels.Add(assignedRoles);
            }

            return View(rolesViewModels);
        }

        [HttpPost]
        [Route("manage-member-roles")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> ManageMemberRoles(IEnumerable<ManageMemberRolesViewModel> model)
        {
            ApplicationUserModel user = await _userManager.FindByIdAsync(model.ToList()[0].MemberId) ?? throw new Exception("No User found having id" + model.ToList()[0].MemberId);
            foreach (ManageMemberRolesViewModel data in model)
            {
                IdentityRole role = await _roleManager.FindByIdAsync(data.RoleID) ?? throw new Exception("No Role found having id" + data.RoleID);

                if ((bool)data.IsSelected) await _userManager.AddToRoleAsync(user, role.Name);
                else await _userManager.RemoveFromRoleAsync(user, role.Name);
            }

            _logger.LogInformation($"member roles having ID: {model.ToList()[0].MemberId} updated by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("MemberViewDetails", new { id = model.ToList()[0].MemberId });
        }

        [HttpPost]
        [Route("change-account-status")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> ChangeAccountStatus(string memberID1, AvailabilityStatusEnum accountStatus)
        {
            ApplicationUserModel user = await _userManager.FindByIdAsync(memberID1);

            if (user != null)
            {
                user.AvailabilityStatus = accountStatus;
                user.JoiningDate = accountStatus == AvailabilityStatusEnum.Active ? DateTime.Now : null;
                await _userManager.UpdateAsync(user);
            }

            _logger.LogInformation($"member having ID: {memberID1} status updated by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("AvailableMembers", new { isAccountStatusUpdated = true });
        }

        [HttpPost]
        [Route("assign-trainer")]
        [Authorize(Roles = "Administration")]
        public async Task<IActionResult> AssignTrainerToMembers(string memberID2, string TrainerID)
        {
            ViewBag.IsAccountStatusUpdated = false;
            ApplicationUserModel member = await _userManager.FindByIdAsync(memberID2) ?? throw new Exception("Member with ID:" + memberID2 + " not found");

            member.AssignedTrainer = TrainerID;
            await _userManager.UpdateAsync(member);

            _logger.LogInformation($"Trainer assign to member having ID: {memberID2} by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("AvailableMembers", new { isTrainerAssigned = true });

        }

        [HttpPost]
        [Authorize(Roles = "Administration")]
        [Route("Members/SearchMember")]
        public IActionResult SearchMember(string memberName)
        {
            IEnumerable<ApplicationUserModel> user = _userManager.Users.Where(x => x.Name.Contains(memberName)).ToList();

            return Json(user);
        }
    }
}
