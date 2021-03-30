using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RegulatoryUniverse.Models;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RegulatoryUniverseContext _context;
        private readonly ISessionManagerService _sessionManagerService;
        private readonly IHelperServices _helperServices;

        public HomeController(ILogger<HomeController> logger, RegulatoryUniverseContext context, ISessionManagerService sessionManagerService, IHelperServices helperServices)
        {
            _logger = logger;
            _context = context;
            _sessionManagerService = sessionManagerService;
            _helperServices = helperServices;
        }

        public async Task<IActionResult> Index()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            if (sessionActiveState == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            var username = HttpContext.Session.GetString("username");

            var allData = (from data in _context.ReportSchedule
                           where data.AssignedEmails.Contains(username)
                           orderby data.CreatedDate descending
                           select data
                      ).ToList();

            var allReportScheduleCount = (from data in _context.ReportSchedule
                                          where data.AssignedEmails.Contains(username)
                                          select data).Count();

            var allDailyCount = (from data in _context.ReportSchedule
                                 where data.ScheduleFrequency.Contains("daily")
                                 where data.AssignedEmails.Contains(username)
                                 select data).Count();

            var allWeeklyCount = (from data in _context.ReportSchedule
                                  where data.ScheduleFrequency.Contains("weekly")
                                  where data.AssignedEmails.Contains(username)
                                  select data).Count();

            var allQuarterlyCount = (from data in _context.ReportSchedule
                                     where data.ScheduleFrequency.Contains("quarterly")
                                     where data.AssignedEmails.Contains(username)
                                     select data).Count();

            var allMonthlyCount = (from data in _context.ReportSchedule
                                   where data.ScheduleFrequency.Contains("monthly")
                                   where data.AssignedEmails.Contains(username)
                                   select data).Count();

            var allHalfYearCount = (from data in _context.ReportSchedule
                                    where data.ScheduleFrequency.Contains("halfyear")
                                    where data.AssignedEmails.Contains(username)
                                    select data).Count();

            var allAnnualCount = (from data in _context.ReportSchedule
                                  where data.ScheduleFrequency.Contains("annually")
                                  where data.AssignedEmails.Contains(username)
                                  select data).Count();

            ViewData["AllReportScheduleCount"] = allReportScheduleCount;

            ViewData["AllDailyCount"] = allDailyCount;
            ViewData["AllWeeklyCount"] = allWeeklyCount;
            ViewData["AllMonthlyCount"] = allMonthlyCount;
            ViewData["AllQuarterlyCount"] = allQuarterlyCount;
            ViewData["AllHalfYearCount"] = allHalfYearCount;
            ViewData["AllAnnualCount"] = allAnnualCount;

            return View(allData);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            if (sessionActiveState == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
