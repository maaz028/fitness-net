using core.Models;
using core.Models.TrainerModels;
using infrastructure.AddionalClasses;
using infrastructure.Repositories.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Fitness_management_system.Controllers
{
    [Route("Trainer")]
    [Authorize(Roles = "Administration")]
    public class TrainerController : Controller
    {
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly IHostingEnvironment _iHosting;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITrainerRepository _trainerRepository;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public TrainerController(SignInManager<ApplicationUserModel> SignInManager, UserManager<ApplicationUserModel> UserManager, IHostingEnvironment IHosting, RoleManager<IdentityRole> RoleManager, ITrainerRepository TrainerRepository, AppDbContext Context, ILogger<TrainerController> logger)
        {
            _signInManager = SignInManager;
            _userManager = UserManager;
            _iHosting = IHosting;
            _roleManager = RoleManager;
            _trainerRepository = TrainerRepository;
            _context = Context;
            _logger = logger;
        }

        public void Boolean(bool StatusVal, bool NewEntryVal)
        {
            if (StatusVal) ViewBag.IsAccountStatusUpdated = true;
            else ViewBag.IsAccountStatusUpdated = false;

            if (NewEntryVal) ViewBag.isNewTrainerAdded = true;
            else ViewBag.isNewTrainerAdded = false;
        }
        private static int selectedEntries = 5;

        [Route("available-trainers")]
        public IActionResult AvailableTrainers(int? entries, bool isAccountStatusUpdated = false, int PageNumber = 1)
        {
            _logger.LogInformation($"Available trainers page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            int PageSize = (int)(entries == null ? TrainerController.selectedEntries : entries);
            TrainerController.selectedEntries = PageSize;

            Boolean(isAccountStatusUpdated, false);
            int recSkip = (PageNumber - 1) * PageSize;
            int totalRows = _context.Trainer.Count();
            Pagination GenratePages = new(PageSize, totalRows);
            int pages = GenratePages.GeneratePages();

            List<TrainerModel> model = _context.Trainer.OrderByDescending(x => x.CreatedDate).Skip(recSkip).Take(PageSize).ToList();

            ViewBag.TotalData = totalRows;
            ViewBag.SerialNo = recSkip + 1;
            ViewBag.PageNumber = PageNumber;
            ViewBag.PageSize = PageSize;
            ViewBag.Pages = pages;
            ViewBag.SelectedEntries = PageSize;
            ViewBag.Partials = recSkip + model.Count;

            _logger.LogInformation($"Available trainers page number: {PageNumber} return {model.Count} rows at {DateTime.Now.ToLongTimeString()}");

            return View(model);
        }

        [HttpGet]
        [Route("add-trainer")]
        public IActionResult AddTrainer(bool IsNewTrainerAdded = false)
        {
            _logger.LogInformation($"Add trainer page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            Boolean(false, IsNewTrainerAdded);

            return View();
        }

        [HttpPost]
        [Route("add-trainer")]
        public async Task<IActionResult> AddTrainer(TrainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                TrainerModel isTrainerAvailable = _context.Trainer.FirstOrDefault(x => x.Email == model.Email);

                if (isTrainerAvailable == null)
                {
                    TrainerModel trainer = new()
                    {
                        TrainerID = Guid.NewGuid().ToString(),
                        Name = model.Name,
                        Email = model.Email,
                        Age = (int)model.Age,
                        JoiningDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        Gender = model.Gender
                    };
                    await _trainerRepository.AddTrainerAsync(trainer);

                    _logger.LogInformation($"New trainer added by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                    return RedirectToAction("AvailableTrainers");
                }

                ModelState.AddModelError(String.Empty, "A Trainer with Email:" + model.Email + " already available.");

            }

            ViewBag.isNewTrainerAdded = false;

            return View(model);
        }

        [HttpGet]
        [Route("delete-trainer/{id}")]
        public async Task<ActionResult> DeleteTrainer(string id)
        {
            _ = await _trainerRepository.RemoveTrainerAsync(id) ?? throw new Exception("Id not found");

            _logger.LogInformation($"Trainer deleted having ID: {id} by {User.Identity.Name} visited at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("AvailableTrainers");
        }

        [HttpPost]
        [Route("change-trainer-account-status")]
        public async Task<IActionResult> ChangeTrainerAccountStatus(string trainerID1, AvailabilityStatusEnum accountStatus)
        {
            TrainerModel trainer = await _trainerRepository.GetSingleTrainerAsync(trainerID1);
            trainer.AvailabilityStatus = accountStatus;
            await _trainerRepository.UpdateTrainerAsync(trainer);

            _logger.LogInformation($"Trainer status updated having ID: {trainerID1} by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("AvailableTrainers", new { isAccountStatusUpdated = true });
        }

        [Route("trainer-details")]
        public async Task<IActionResult> TrainerDetails(string id)
        {
            TrainerModel trainer = await _trainerRepository.GetSingleTrainerAsync(id) ?? throw new Exception("Invalid trainer id");

            _logger.LogInformation($"Trainer details page having ID: {id} visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return View(trainer);
        }

        [HttpGet]
        [Route("edit-trainer")]
        public async Task<IActionResult> EditTrainerDetails(string id, bool IsTrainerDetailsUpdated = false)
        {
            _logger.LogInformation($"Trainer edit page having ID: {id} visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            if (IsTrainerDetailsUpdated)
            {
                ViewBag.IsTrainerDetailsUpdated = true;
            }
            else
            {
                ViewBag.IsTrainerDetailsUpdated = false;
            }

            TrainerModel trainer = await _trainerRepository.GetSingleTrainerAsync(id) ?? throw new Exception("Invalid trainer id");

            TrainerViewModel model = new()
            {
                ID = trainer.TrainerID,
                Name = trainer.Name,
                Email = trainer.Email,
                Gender = trainer.Gender,
                Age = trainer.Age,
                AvailabilityStatus = trainer.AvailabilityStatus,
                CreatedDate = trainer.CreatedDate,
                JoiningDate = trainer.JoiningDate,
                PhotoPath = trainer.PhotoPath,
            };

            return View(model);
        }

        [HttpPost]
        [Route("edit-trainer")]
        public async Task<IActionResult> EditTrainerDetails(TrainerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? uniqueFilename = null;

                if (model.Photo != null)
                {
                    string uploadFolder = Path.Combine(_iHosting.WebRootPath, "images/trainers");
                    uniqueFilename = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filepath = Path.Combine(uploadFolder, uniqueFilename);
                    model.Photo.CopyTo(new FileStream(filepath, FileMode.Create));
                }

                model.PhotoPath = !string.IsNullOrEmpty(uniqueFilename) ? uniqueFilename : model.PhotoPath;

                TrainerModel trainerModel = new()
                {
                    TrainerID = model.ID,
                    Name = model.Name,
                    Email = model.Email,
                    Gender = model.Gender,
                    Age = (int)model.Age,
                    AvailabilityStatus = model.AvailabilityStatus,
                    CreatedDate = model.CreatedDate,
                    JoiningDate = model.JoiningDate,
                    PhotoPath = model.PhotoPath,
                };

                _ = await _trainerRepository.UpdateTrainerAsync(trainerModel) ?? throw new Exception("Invalid trainer id");

                _logger.LogInformation($"Trainer details updated having ID: {model.ID} by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                return RedirectToAction("AvailableTrainers");
            }

            ViewBag.IsTrainerDetailsUpdated = false;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administration")]
        public IActionResult SearchTrainer(string trainerName)
        {
            IEnumerable<TrainerModel> trainers = _context.Trainer.Where(x => x.Name.Contains(trainerName)).ToList();

            return Json(trainers);
        }

    }

}
