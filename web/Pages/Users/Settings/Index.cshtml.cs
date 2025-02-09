using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Atlas_Web.Models;
using Atlas_Web.Helpers;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Pages.Users.Settings
{
    public class IndexModel : PageModel
    {
        private readonly Atlas_WebContext _context;
        private readonly IMemoryCache _cache;
        public UserSetting EnableShareNotifications { get; set; }

        public IndexModel(Atlas_WebContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ActionResult> OnGetAsync()
        {
            EnableShareNotifications = await _context.UserSettings.SingleOrDefaultAsync(
                x => x.Name == "share_notification" && x.UserId == User.GetUserId()
            );

            return Page();
        }

        public async Task<ActionResult> OnGetEnableShareNotification(string value)
        {
            var shareNotification = await _context.UserSettings.SingleOrDefaultAsync(
                x => x.Name == "share_notification" && x.UserId == User.GetUserId()
            );

            if (shareNotification != null)
            {
                shareNotification.Value = value;
                await _context.SaveChangesAsync();
            }
            else
            {
                await _context.AddAsync(
                    new UserSetting
                    {
                        UserId = User.GetUserId(),
                        Name = "share_notification",
                        Value = value
                    }
                );
                await _context.SaveChangesAsync();
            }

            return Content("ok");
        }
    }
}
