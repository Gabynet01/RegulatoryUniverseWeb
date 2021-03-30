using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegulatoryUniverse.Models;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse.Controllers
{
    public class AdminAppUserController : Controller
    {
        private readonly UserProfileServiceDbContext _contextUser;
        private readonly RegulatoryUniverseContext _context;
        private readonly ILogger<AdminAppUserController> _logger;
        private readonly ISessionManagerService _sessionManagerService;

        public AdminAppUserController(RegulatoryUniverseContext context, UserProfileServiceDbContext contextUser, ILogger<AdminAppUserController> logger, ISessionManagerService sessionManagerService)
        {
            _context = context;
            _contextUser = contextUser;
            _logger = logger;
            _sessionManagerService = sessionManagerService;
        }
        public IActionResult Index()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            var allUserData = (from dataUsers in _contextUser.Users
                               join dataUserRoles in _contextUser.UserRoles on dataUsers.UserId equals dataUserRoles.UserId
                               join dataRolesDefinition in _contextUser.Roles on dataUserRoles.RoleId equals dataRolesDefinition.RoleId
                               where dataUserRoles.RoleId == 2019 || dataUserRoles.RoleId == 2020
                               select new ManageAppUserData
                               {
                                   UserId = dataUsers.UserId,
                                   FullName = dataUsers.FullName,
                                   Email = dataUsers.Email,
                                   Username = dataUsers.Username,
                                   Role = dataRolesDefinition.Name

                               }).ToList();

            _logger.LogInformation("AdminAppUserController");
            _logger.LogInformation("Index page loaded");
            return View(allUserData);
        }


        // GET: Users/Details/5
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

            var users = await _contextUser.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
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

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,FullName,Email,Password,PasswordSalt,ChangePassword,IsActiveDirectoryUser,IsLocked,LogInAttemptCount,LastFailedLogInAttempt,ClientId,BranchId,IsActive,EntryBy,EntryDate,ModifiedBy,ModifiedDate,T24userName")] Users users)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (ModelState.IsValid)
            {
                _contextUser.Add(users);
                await _contextUser.SaveChangesAsync();

                _logger.LogInformation("AdminAppUserController");
                _logger.LogInformation("Create action performed");
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }
        [HttpPost]
        public IActionResult Add()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            var username = Convert.ToString(HttpContext.Request.Form["username"]);
            var role = Convert.ToString(HttpContext.Request.Form["UserRole"]);

            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal principal = new UserPrincipal(context);
            principal.EmailAddress = $"*{username}*";
            principal.Enabled = true;
            PrincipalSearcher searcher = new PrincipalSearcher(principal);
            var principal_users = searcher.FindAll().AsQueryable().Cast<UserPrincipal>().OrderBy(x => x.Surname).ToList();
            //return users;
            var email = principal_users[0].EmailAddress;
            var fullname = principal_users[0].Name;
            if (principal_users.Count() != 0)
            {
                int compliance_admin_role_id = 2019;
                int compliance_urole_id = 2020;

                string[] username_explode = username.Split("@");
                //Console.WriteLine(jsonList);
                username = username_explode[0];
                var user = (from data in _contextUser.Users
                            where data.Username == username
                            select data
                           ).FirstOrDefault();

                //var userId = user.UserId;
                _logger.LogInformation("AdminAppUserController");
                _logger.LogInformation("User exists in UserprofileDb");
                if (user != null)
                {

                    var userId = user.UserId;

                    var result1 = (from datauserRole in _contextUser.UserRoles
                                   where datauserRole.UserId == userId
                                   where datauserRole.RoleId == compliance_admin_role_id || datauserRole.RoleId == compliance_urole_id
                                   join dataRole in _contextUser.Roles on datauserRole.RoleId equals dataRole.RoleId
                                   select new UserRoleData
                                   {
                                       UserId = userId,
                                       Username = user.Username,
                                       UserRole = datauserRole.RoleId,
                                       UserRoleName = dataRole.Name,

                                   }).ToList();


                    if (result1.Count() == 0)
                    {
                        _logger.LogInformation("AdminAppUserController");
                        _logger.LogInformation("no roles for the user");

                        UserRoles addrole = new UserRoles();
                        addrole.RoleId = Convert.ToInt32(role);
                        addrole.UserId = userId;

                        _contextUser.Add(addrole);
                        var adduserrole = _contextUser.SaveChanges();

                        ViewBag.message_status = "success";
                        ViewBag.message = "Role assigned successfully";

                    }
                    else
                    {
                        ViewBag.message_status = "error";
                        ViewBag.message = "OOPS!, this user has the same role";

                        _logger.LogInformation("AdminAppUserController");
                        _logger.LogInformation("User has the same role");

                    }

                }
                else
                {

                    Users newuser = new Users();
                    DateTime currentDateTime = DateTime.Now;
                    newuser.Username = username;
                    newuser.FullName = fullname;
                    newuser.Email = email;
                    newuser.Password = Convert.ToString(0);
                    newuser.PasswordSalt = Convert.ToString(0);
                    newuser.ChangePassword = Convert.ToBoolean("False");
                    newuser.IsActiveDirectoryUser = Convert.ToBoolean("True");
                    newuser.IsActive = Convert.ToBoolean("True");
                    newuser.IsLocked = Convert.ToBoolean("False");
                    newuser.LogInAttemptCount = 0;
                    newuser.LastFailedLogInAttempt = currentDateTime;
                    newuser.ClientId = null;
                    newuser.EntryBy = null;
                    newuser.EntryDate = currentDateTime;
                    newuser.ModifiedBy = null;
                    newuser.ModifiedDate = currentDateTime;
                    newuser.T24userName = null;

                    try
                    {
                        _contextUser.Add(newuser);
                        var adduserq = _contextUser.SaveChanges();

                        if (adduserq == 1)
                        {

                            UserRoles addrole1 = new UserRoles();
                            addrole1.RoleId = Convert.ToInt32(role);
                            addrole1.UserId = newuser.UserId;

                            _contextUser.Add(addrole1);
                            var adduserrole = _contextUser.SaveChanges();

                            _logger.LogInformation("AdminAppUserController");
                            _logger.LogInformation("user role added");
                        }


                        ViewBag.message_status = "success";
                        ViewBag.message = "User and role created successfully";
                    }
                    catch (Exception ex)
                    {
                        throw;
                        ViewBag.message_status = "error";
                        ViewBag.message = ex;

                        _logger.LogInformation("AdminAppUserController");
                        _logger.LogInformation("Error occured when creating user");
                    }

                }

            }
            else
            {
                ViewBag.message_status = "error";
                ViewBag.message = "User not found in the AD";

                _logger.LogInformation("AdminAppUserController");
                _logger.LogInformation("User not found in AD");

            }

            //return Content(username + " " + role);
            return View();
        }

        // GET: Users/Edit/5
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

            var users = await _contextUser.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,FullName,Email,Password,PasswordSalt,ChangePassword,IsActiveDirectoryUser,IsLocked,LogInAttemptCount,LastFailedLogInAttempt,ClientId,BranchId,IsActive,EntryBy,EntryDate,ModifiedBy,ModifiedDate,T24userName")] Users users)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id != users.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _contextUser.Update(users);
                    await _contextUser.SaveChangesAsync();

                    _logger.LogInformation("AdminAppUserController");
                    _logger.LogInformation("Update user successful");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.UserId))
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
            return View(users);
        }

        // GET: Users/Delete/5
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

            var users = await _contextUser.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (users == null)
            {
                return NotFound();
            }

            _logger.LogInformation("AdminAppUserController");
            _logger.LogInformation("User delete page loaded");
            return View(users);
        }

        // POST: Users/Delete/5
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

            var users = await _contextUser.Users.FindAsync(id);
            _contextUser.Users.Remove(users);
            await _contextUser.SaveChangesAsync();

            _logger.LogInformation("AdminAppUserController");
            _logger.LogInformation("User deleted successfully");
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            _logger.LogInformation("AdminAppUserController");
            _logger.LogInformation("User exists successful");

            return _contextUser.Users.Any(e => e.UserId == id);
        }
    }
}