using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegulatoryUniverse.Models;
using Microsoft.AspNetCore.Http;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse.Controllers
{
    public class ReportSchedulesController : Controller
    {
        private readonly RegulatoryUniverseContext _context;
        private readonly ISessionManagerService _sessionManagerService;
        private readonly IHelperServices _helperServices;

        public ReportSchedulesController(RegulatoryUniverseContext context, ISessionManagerService sessionManagerService, IHelperServices helperServices)
        {
            _context = context;
            _sessionManagerService = sessionManagerService;
            _helperServices = helperServices;
        }


        // GET: ReportSchedules
        public async Task<IActionResult> Index()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            var allReports = await (from data in _context.ReportSchedule
                                    orderby data.CreatedDate descending
                                    select data).ToListAsync();

            var allReportScheduleCount = (from data in _context.ReportSchedule
                                          select data).Count();

            var allDailyCount = (from data in _context.ReportSchedule
                                 where data.ScheduleFrequency.Contains("daily")
                                 select data).Count();

            var allWeeklyCount = (from data in _context.ReportSchedule
                                  where data.ScheduleFrequency.Contains("weekly")
                                  select data).Count();

            var allQuarterlyCount = (from data in _context.ReportSchedule
                                     where data.ScheduleFrequency.Contains("quarterly")
                                     select data).Count();

            var allMonthlyCount = (from data in _context.ReportSchedule
                                   where data.ScheduleFrequency.Contains("monthly")
                                   select data).Count();

            var allHalfYearCount = (from data in _context.ReportSchedule
                                    where data.ScheduleFrequency.Contains("halfyear")
                                    select data).Count();

            var allAnnualCount = (from data in _context.ReportSchedule
                                  where data.ScheduleFrequency.Contains("annually")
                                  select data).Count();

            ViewData["AllReportScheduleCount"] = allReportScheduleCount;

            ViewData["AllDailyCount"] = allDailyCount;
            ViewData["AllWeeklyCount"] = allWeeklyCount;
            ViewData["AllMonthlyCount"] = allMonthlyCount;
            ViewData["AllQuarterlyCount"] = allQuarterlyCount;
            ViewData["AllHalfYearCount"] = allHalfYearCount;
            ViewData["AllAnnualCount"] = allAnnualCount;

            return View(allReports);
        }

        // GET: ReportSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id == null)
            {
                return NotFound();
            }

            var reportSchedule = await _context.ReportSchedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportSchedule == null)
            {
                return NotFound();
            }

            return View(reportSchedule);
        }

        // GET: ReportSchedules/Create
        public IActionResult Create()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            ViewData["username"] = HttpContext.Session.GetString("username");
            return View();
        }

        // POST: ReportSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReportName,ReceivingInstitution,ScheduleFrequency,ScheduleFrequencyDate,ScheduleFrequencyTime,Description,ApplicableLawRegulation,ResponsibleDepartment,AssignedEmails,CreatedDate,CreatedBy")] ReportSchedule reportSchedule)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            //check if input is empty
            if (_helperServices.IsAnyNullOrEmpty(reportSchedule) == true)
            {
                ViewBag.errorMsg = "All fields are required";
                return View(reportSchedule);
            }

            //check if special characters are found
            if (_helperServices.IsHtmlCharactersFound(reportSchedule) == true)
            {
                ViewBag.errorMsg = "Input field contains special characters, kindly remove them";
                return View(reportSchedule);
            }

            if (ModelState.IsValid)
            {
                _context.Add(reportSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reportSchedule);
        }

        // GET: ReportSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id == null)
            {
                return NotFound();
            }

            var reportSchedule = await _context.ReportSchedule.FindAsync(id);
            if (reportSchedule == null)
            {
                return NotFound();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(reportSchedule);
        }

        // POST: ReportSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReportName,ReceivingInstitution,ScheduleFrequency,ScheduleFrequencyDate,ScheduleFrequencyTime,Description,ApplicableLawRegulation,ResponsibleDepartment,AssignedEmails,CreatedDate,CreatedBy")] ReportSchedule reportSchedule)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id != reportSchedule.Id)
            {
                return NotFound();
            }

            //check if input is empty
            if (_helperServices.IsAnyNullOrEmpty(reportSchedule) == true)
            {
                ViewBag.errorMsg = "All fields are required";
                return View(reportSchedule);
            }

            //check if special characters are found
            if (_helperServices.IsHtmlCharactersFound(reportSchedule) == true)
            {
                ViewBag.errorMsg = "Input field contains special characters, kindly remove them";
                return View(reportSchedule);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportScheduleExists(reportSchedule.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reportSchedule);
        }

        // GET: ReportSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id == null)
            {
                return NotFound();
            }

            var reportSchedule = await _context.ReportSchedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportSchedule == null)
            {
                return NotFound();
            }

            return View(reportSchedule);
        }

        // POST: ReportSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            var reportSchedule = await _context.ReportSchedule.FindAsync(id);
            _context.ReportSchedule.Remove(reportSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportScheduleExists(int id)
        {
            return _context.ReportSchedule.Any(e => e.Id == id);
        }
    }
}
