using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Areas.Administrator.ViewModel;
using SchoolManagmentSystem.Data;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class AdmController : Controller
    {

        private readonly ILogger<AdmController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public AdmController(ILogger<AdmController> logger, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ManageUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult RolesList()
        {
            ViewBag.roles = _roleManager.Roles.ToList();
            return View();

        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                if (!String.IsNullOrEmpty(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            return RedirectToAction(nameof(RolesList));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (!String.IsNullOrEmpty(roleName))
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    await _roleManager.DeleteAsync(role);
                }
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (ApplicationUser user in users)
            {
                var thisViewModel = new UserRolesViewModel();
                thisViewModel.UserId = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.FirstName = user.FirstName;
                thisViewModel.LastName = user.LastName;
                thisViewModel.Roles = await GetUserRoles(user);
                userRolesViewModel.Add(thisViewModel);
            }
            return View(userRolesViewModel);
        }
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }


        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("ListUsers");
        }


    }
}

