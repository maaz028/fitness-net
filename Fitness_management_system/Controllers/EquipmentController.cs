using core.Models;
using core.Models.EquipmentModels;
using infrastructure.AddionalClasses;
using infrastructure.Repositories.Trainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Fitness_management_system.Controllers
{
    [Route("Equipment")]
    [Authorize(Roles = "Administration, Member")]
    public class EquipmentController : Controller
    {
        private readonly SignInManager<ApplicationUserModel> _signInManager;
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly IHostingEnvironment _iHosting;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITrainerRepository _trainerRepository;
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public EquipmentController(SignInManager<ApplicationUserModel> signInManager, UserManager<ApplicationUserModel> userManager, IHostingEnvironment iHosting, RoleManager<IdentityRole> roleManager, ITrainerRepository trainerRepository, AppDbContext context, ILogger<EquipmentController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _iHosting = iHosting;
            _roleManager = roleManager;
            _trainerRepository = trainerRepository;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("add-equipment")]
        [Authorize(Roles = "Administration")]
        public IActionResult AddEquipment(bool IsNewEquipmentAdded = false)
        {
            _logger.LogInformation($"Add Equipment page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            ViewBag.IsNewEquipmentAdded = IsNewEquipmentAdded;

            return View();
        }

        [HttpPost]
        [Route("add-equipment")]
        [Authorize(Roles = "Administration")]
        public IActionResult AddEquipment(EquipmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Database.ExecuteSql($"spAddEquipment {Guid.NewGuid()},{model.Name.Trim()},{model.Description}");

                _logger.LogInformation($"New equipment having name:{model.Name} added by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                return RedirectToAction("AvailableEquipments", new { IsNewEquipmentAdded = true });
            }

            ViewBag.IsNewEquipmentAdded = false;

            return View(model);
        }

        [HttpGet]
        [Route("delete-equipment/{id}")]
        [Authorize(Roles = "Administration")]
        public async Task<ActionResult> DeleteEquipment(string id)
        {
            EquipmentModel eqp = _context.Equipment.Find(id);

            _context.Equipment.Remove(eqp);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Equipment having ID:{id} deleted by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            return RedirectToAction("AvailableEquipments");
        }

        private static int selectedEntries = 5;

        [HttpGet]
        [Route("available-equipments")]
        public IActionResult AvailableEquipments(int? entries, int pageNumber = 1)
        {
            _logger.LogInformation($"Available Equipment page visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            int pageSize = (int)(entries == null ? EquipmentController.selectedEntries : entries);
            EquipmentController.selectedEntries = pageSize;

            int recSkip = (pageNumber - 1) * pageSize;

            IEnumerable<EquipmentModel> model = _context.Equipment.FromSqlRaw("spGetEquipments").ToList();
            Pagination Pager = new(pageSize, model.Count());
            int Pages = Pager.GeneratePages();

            ViewBag.TotalData = model.Count();
            model = model.Skip(recSkip).Take(pageSize).ToList();

            ViewBag.Partials = recSkip + model.Count();
            ViewBag.PageNumber = pageNumber;
            ViewBag.Pages = Pages;
            ViewBag.SerialNo = recSkip + 1;
            ViewBag.SelectedEntries = pageSize;

            _logger.LogInformation($"Available equipments page number: {pageNumber} return {model.Count()} rows at {DateTime.Now.ToLongTimeString()}");


            return View(model);
        }

        [Route("equipment-details")]
        public ViewResult EquipmentDetails(string id)
        {
            _logger.LogInformation($"Equipment details page having ID:{id} visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            EquipmentModel model = _context.Equipment.Find(id) ?? throw new Exception($"Equipment with ID: {id} not available");

            return View(model);
        }

        [HttpGet]
        [Route("edit-equipment-details")]
        [Authorize(Roles = "Administration")]
        public IActionResult EditEquipmentDetails(string id)
        {
            _logger.LogInformation($"Edit Equipment details page having ID:{id} visited by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

            EquipmentModel eqp = _context.Equipment.Find(id);

            if (eqp == null)
                throw new Exception($"No Equipment Available with ID: {id}");

            EquipmentViewModel model = new EquipmentViewModel()
            {
                ID = eqp.ID,
                Name = eqp.Name,
                Description = eqp.Description,
                CreatedDate = eqp.CreatedDate,
                LastUpdatedDate = eqp.LastUpdatedDate,
                PhotoPath = eqp.PhotoPath
            };

            return View(model);
        }

        [HttpPost]
        [Route("edit-equipment-details")]
        [Authorize(Roles = "Administration")]
        public IActionResult EditEquipmentDetails(EquipmentViewModel model)
        {
            string uniqueFilename = string.Empty;

            if (ModelState.IsValid)
            {
                if (model.Photo != null)
                {
                    string folderName = Path.Combine(_iHosting.WebRootPath, "images/equipments");
                    uniqueFilename = Guid.NewGuid() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(folderName, uniqueFilename);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }

                EquipmentModel eqp = new EquipmentModel()
                {
                    ID = model.ID,
                    Name = model.Name,
                    Description = model.Description,
                    CreatedDate = model.CreatedDate,
                    PhotoPath = !string.IsNullOrEmpty(uniqueFilename) ? uniqueFilename : model.PhotoPath,
                };
                _context.Database.ExecuteSql($"spUpdateEquipmentDetails {eqp.ID}, {eqp.Name}, {eqp.Description}, {eqp.CreatedDate}, {eqp.PhotoPath}");

                _logger.LogInformation($"Equipment details having ID:{model.ID} updated by {User.Identity.Name} at {DateTime.Now.ToLongTimeString()}");

                return RedirectToAction("availableEquipments");
            }

            return View(model);
        }
    }

}

