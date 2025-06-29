using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models.Entities;
using TaskManager.Models.ViewModels;

namespace TaskManager.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["ActiveLink"] = "Dashboard";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return View("NotFound");
            }

            var userTasks = await _context.Tasks.Where(t=>t.UserId == user.Id).ToListAsync();

            var newTasks = userTasks.Where(t => t.CreatedOn.Date == DateTime.Today).ToList();
            var dueToday = userTasks.Where(t => t.DueDate == DateTime.Today && t.Status!="Completed").ToList();
            var pending = userTasks.Where(t => t.Status == "Pending").ToList();
            var inProgress = userTasks.Where(t => t.Status == "In_Progress").ToList();
            var completed = userTasks.Where(t => t.Status == "Completed").ToList();

            var viewModel = new DashboardViewModel
            {
                NewTasks = newTasks,
                TasksDueToday = dueToday,
                PendingTasks = pending,
                In_ProgressTasks = inProgress,
                CompetedTasks = completed,
            };

            return View(viewModel);
        }

        // get tasks / all, pending, inProgress, completed
        public async Task<IActionResult> GetTasksData()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return Json(new { success = false, data = "User not found" });
            }

            var userTasks = _context.Tasks.Where(t => t.UserId == user.Id);

            var all = userTasks.Count();
            var pending = userTasks.Count(t => t.Status == "Pending");
            var inProgress = userTasks.Count(t => t.Status == "In_Progress");
            var completed = userTasks.Count(t => t.Status == "Completed");

            return Json(new
            {
                all, 
                pending,
                completed,
                inProgress,
            });
        }
    }
}