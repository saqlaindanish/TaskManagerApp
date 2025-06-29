using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManager.Data;
using TaskManager.Models;
using TaskManager.Models.Entities;

namespace TaskManager.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public NotificationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // helper method to create notification when any task(s) meeting deadline
        private async Task CheckTasksAndAddNotificationsAsync(string userId)
        {
            var userTasks = await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();

            var now = DateTime.Now;
            var today = DateTime.Today;
            var twoHoursLater = now.AddHours(2);

            // Get tasks due today or due within next 2 hours
            var relevantTasks =  userTasks
                                        .Where(t =>
                                            t.DueDate == today ||                             // Due today
                                            (t.DueDate > now && t.DueDate <= twoHoursLater)   // Due within next 2 hours
                                        )
                                        .ToList();

            foreach (var task in relevantTasks)
            {
                string message;

                if (task.DueDate.Date == today && task.DueDate > now)
                {
                    message = $"Your task '{task.Title}' is due today.";
                }
                else if (task.DueDate > now && task.DueDate <= twoHoursLater)
                {
                    message = $"Only {Math.Round((task.DueDate - now).TotalMinutes)} minutes left for task '{task.Title}'!";
                }
                else
                {
                    continue; // No need to notify
                }

                // Check if notification for this task already exists (optional, to prevent duplicates)
                bool exists = await _context.Notifications.AnyAsync(n =>
                    n.TaskId == task.Id &&
                    n.UserId == task.UserId &&
                    n.Message == message &&
                    n.CreatedAt.Date == now.Date
                );

                if (!exists)
                {
                    var notification = new Notification
                    {
                        UserId = task.UserId,
                        TaskId = task.Id,
                        Message = message,
                        CreatedAt = now,
                        Type = "Task Related"
                    };

                    _context.Notifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
        }



        // get all notifications 
        public async Task<IActionResult> GetNotificationsData()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("NotFound");
            }

            // call for deadline tasks
            await CheckTasksAndAddNotificationsAsync(user.Id);

            var allNotifications = await _context.Notifications
                                                    .Include(n => n.Task)
                                                    .Where(n => n.UserId == user.Id)
                                                    .OrderByDescending(n => n.CreatedAt)
                                                    .Select(n => new
                                                    {
                                                        n.Id,
                                                        n.Message,
                                                        createdAt = n.CreatedAt.ToString("MMMM dd yyyy (dddd) hh:mm tt"),
                                                        n.IsSeen,
                                                        n.IsRead,
                                                        TaskTitle = n.Task!.Title,
                                                        TaskId = n.Task.Id,
                                                    })
                                                    .ToListAsync();

            var unReadNotifications = await _context.Notifications
                                                    .Where(n => n.UserId == user.Id && n.IsSeen == false)
                                                    .Select(n => new 
                                                    {
                                                        n.Id,
                                                        n.Message,
                                                    }).ToListAsync();
                                                   

            return Json(new
            {
                all = allNotifications.Take(6),
                unRead = unReadNotifications,
            });
        }


        // update the notification's IsSeen status to True
        [HttpPost]
        public async Task<IActionResult> UpdateIsSeenStatus([FromBody] List<Notification> notifications)
        {
            var user = await _userManager.GetUserAsync(User);

            foreach (var notification in notifications)
            {
                var n = await _context.Notifications.FindAsync(notification.Id);

                if (n != null && n.IsSeen == false)
                {
                    n.IsSeen = true;

                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        // get single notification
        public async Task<IActionResult> GetNotification(string id)
        {
            ViewData["ActiveParent"] = "Notifications";

            var notification = await _context.Notifications.Include(n=>n.Task).FirstOrDefaultAsync(n=>n.Id == id);

            if(notification !=null && notification.IsRead == false)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(notification);
        }


        // get all notifications for the user
        public async Task<IActionResult> AllNotifications()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null)
            {
                return View("NotFound");
            }

            ViewData["ActiveParent"] = "Notifications";
            ViewData["ActiveLink"] = "AllNotifications";

            var notifications = await _context.Notifications.Where(n => n.UserId == user.Id).OrderByDescending(n=>n.CreatedAt).ToListAsync();

            var unSeenNotifications = notifications.Where(n => n.IsSeen == false).ToList();
           
            foreach(var notification in unSeenNotifications)
            {
                notification.IsSeen = true;
            }
            
            await _context.SaveChangesAsync();

            return View(notifications);
        }

    }
}
