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
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TaskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // helper method to create notification 
        private void CreateNotification(string userId, string taskId, string message, string type)
        {
            var newNotification = new Notification
            {
                UserId = userId,
                TaskId = taskId,
                Message = message,
                Type = type,
                CreatedAt = DateTime.Now,
            };

            _context.Notifications.Add(newNotification);
        }

        // get all tasks
        public async Task<IActionResult> GetTasks(string ? p = null, string? priority = null, string? status = null, string? date= null, string ? searchQuery = null)
        {
            ViewData["ActiveParent"] = "MyTasks";

            (ViewData["ActiveLink"], ViewData["Title"]) = p switch
            {
                "Pending" => ("Pending", "Pending Tasks"),
                "In_Progress" => ("In_Progress", "In_Progress Tasks"),
                "Completed" => ("Completed", "Completed Tasks"),
                _ => ("AllTasks", "All Tasks")
            };

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("NotFound");
            }

            var query = _context.Tasks.Where(t => t.UserId == user.Id);

            if (p != null)
            {
                query = query.Where(t=>t.Status == p);
            }

            if(priority != null)
            {
                query = query.OrderByDescending(t => t.Priority == priority);
            }

            if(status != null)
            {
                query = query.OrderByDescending(t=>t.Status == status);
            }

            if (date != null)
            {
                if(date == "Newest")
                {
                    query = query.OrderByDescending(t=>t.CreatedOn);
                }
                else if(date == "Oldest")
                {
                    query = query.OrderBy(t => t.CreatedOn);
                }
            }

            // perform search matches
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.ToLower();

                query = query.Where(t =>
                    (t.Title != null && t.Title.ToLower().Contains(searchQuery)) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchQuery)) ||
                    (t.Status != null && t.Status.ToLower().Contains(searchQuery)) ||
                    (t.Priority != null && t.Priority.ToLower().Contains(searchQuery)) ||
                    (t.CreatedOn.ToString() != null && t.CreatedOn.ToString().ToLower().Contains(searchQuery)) ||
                    (t.DueDate.ToString() != null && t.DueDate.ToString().ToLower().Contains(searchQuery))
                );
            }

            var tasks = await query.ToListAsync();

            return View(tasks);
        }

        // add new task
        public IActionResult AddTask(string ? returnUrl)
        {
            var model = new AddTaskViewModel
            {
                ReturnUrl = returnUrl,
            };
            return PartialView("_AddTaskPartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(AddTaskViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddTaskPartialView", model);
            }

            var user = await _userManager.GetUserAsync(User);

            // add new task to database

            var newTask = new TaskItem
            {
                Title = model.Title,
                Description = model.Description,
                Priority = model.Priority.ToString(),
                CreatedOn = DateTime.Now,
                Status = "Pending",
                DueDate = model.DueDate,
                UserId = user.Id,
            };

            _context.Tasks.Add(newTask);
            
           
            var message = $"New Task \"{newTask.Title}\" has been added.";      // message for the notification
            var type = "Task Related";          // type of notification

            // create a new notification
            CreateNotification(user.Id, newTask.Id,message, type);

            await _context.SaveChangesAsync();
            
            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Json(new { success = true, redirectUrl = model.ReturnUrl });
            }

            return Json(new { success = true, redirectUrl = Url.Action("GetTasks") });
        }

        // edit task
        public async Task<IActionResult> EditTask(string taskId, string ? returnUrl)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(task => task.Id == taskId);

            if (task == null)
            {
                return View("NotFound");
            }

            var model = new EditTaskViewModel
            {
                TaskId = task.Id,
                ReturnUrl = returnUrl,
                Title = task.Title,
                Description = task.Description,
                Priority = Enum.Parse<TaskPriority>(task.Priority),
                Status = Enum.Parse<Status>(task.Status),
                DueDate = task.DueDate,
            };

            return PartialView("_EditTaskPartialView", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(EditTaskViewModel model)
        {
            var task = await _context.Tasks.Include(t=>t.User).FirstOrDefaultAsync(t => t.Id == model.TaskId);

            if (task == null)
            {
                return View("NotFound");
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_EditTaskPartialView", model);
            }

            task.Title = model.Title;
            task.Description = model.Description;
            task.DueDate = model.DueDate;
            task.Priority = model.Priority.ToString();
            task.Status = model.Status.ToString();

            var message = $"Task \"{task.Title}\" has been updated.";  // message for notification

            var type = "Task Related";                  // type of notification

            CreateNotification(task.User.Id, task.Id, message, type);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Json(new { success = true, redirectUrl = model.ReturnUrl });
            }

            return Json(new { success = true, redirectUrl = Url.Action("GetTasks") });
        }

        // delete task
        public async Task<IActionResult> DeleteTask(string taskId, string ? returnUrl)
        {
            var task = await _context.Tasks.FindAsync(taskId);

            if (task == null)
            {
                return View("NotFound");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return PartialView("_DeleteTaskPartialView", task);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(TaskItem model, string? ReturnUrl)
        {
            var task = await _context.Tasks.Include(t=>t.User).FirstOrDefaultAsync(t=>t.Id==model.Id);

            var taskTitle = task.Title;
           
            if (task == null)
            {
                return View("NotFound");
            }

            var message = $"Task\"{taskTitle}\" has been deleted.";
            var type = "Task Related";
            
            _context.Tasks.Remove(task);

            CreateNotification(task.User.Id, task.Id, message, type);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return Json(new { success = true, redirectUrl = ReturnUrl });

            }

            return Json(new { success = true, redirectUrl = Url.Action("GetTasks") });
        }

    }
}
