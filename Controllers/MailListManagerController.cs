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
    public class MailListManagerController : Controller
    {
        private readonly RegulatoryUniverseContext _context;
        private readonly ISessionManagerService _sessionManagerService;
        private readonly IHelperServices _helperServices;

        public MailListManagerController(RegulatoryUniverseContext context, ISessionManagerService sessionManagerService, IHelperServices helperServices)
        {
            _context = context;
            _sessionManagerService = sessionManagerService;
            _helperServices = helperServices;
        }


        // GET: MailListManager
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

            var allReports = await (from data in _context.MailListManager
                                    orderby data.CreatedDate descending
                                    select data).ToListAsync();


            return View(allReports);
        }

        // GET: MailListManager/Details/5
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

            var mailListManager = await _context.MailListManager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mailListManager == null)
            {
                return NotFound();
            }

            return View(mailListManager);
        }

        // GET: MailListManager/Create
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

        // POST: mailListManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Email, Class, CreatedBy, CreatedDate")] MailListManager mailListManager)
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
            if (_helperServices.IsAnyNullOrEmpty(mailListManager) == true)
            {
                ViewBag.errorMsg = "All fields are required";
                return View(mailListManager);
            }

            //check if special characters are found
            if (_helperServices.IsHtmlCharactersFound(mailListManager) == true)
            {
                ViewBag.errorMsg = "Input field contains special characters, kindly remove them";
                return View(mailListManager);
            }

            if (ModelState.IsValid)
            {
                _context.Add(mailListManager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mailListManager);
        }

        // GET: MailListManager/Edit/5
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

            var mailListManager = await _context.MailListManager.FindAsync(id);
            if (mailListManager == null)
            {
                return NotFound();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View(mailListManager);
        }

        // POST: MailListManager/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Email, Class, CreatedBy, CreatedDate")] MailListManager mailListManager)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id != mailListManager.Id)
            {
                return NotFound();
            }

            //check if input is empty
            if (_helperServices.IsAnyNullOrEmpty(mailListManager) == true)
            {
                ViewBag.errorMsg = "All fields are required";
                return View(mailListManager);
            }

            //check if special characters are found
            if (_helperServices.IsHtmlCharactersFound(mailListManager) == true)
            {
                ViewBag.errorMsg = "Input field contains special characters, kindly remove them";
                return View(mailListManager);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mailListManager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!mailListManagerExists(mailListManager.Id))
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
            return View(mailListManager);
        }

        // GET: MailListManager/Delete/5
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

            var mailListManager = await _context.MailListManager
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mailListManager == null)
            {
                return NotFound();
            }

            return View(mailListManager);
        }

        // POST: mailListManagers/Delete/5
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

            var mailListManager = await _context.MailListManager.FindAsync(id);
            _context.MailListManager.Remove(mailListManager);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool mailListManagerExists(int id)
        {
            return _context.MailListManager.Any(e => e.Id == id);
        }
    }
}
