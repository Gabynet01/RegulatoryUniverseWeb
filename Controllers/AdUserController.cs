using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegulatoryUniverse.Models;
using Newtonsoft.Json;
//using RegulatoryUniverse.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse.Controllers
{
    public class AdUserController : Controller
    {
        private readonly ILogger<AdUserController> _logger;
        private readonly RegulatoryUniverseContext _context;
        private readonly ISessionManagerService _sessionManagerService;

        public AdUserController(ILogger<AdUserController> logger, RegulatoryUniverseContext context, ISessionManagerService sessionManagerService)
        {
            _context = context;
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

            return View();
        }

        [AcceptVerbs("Post")]
        public JsonResult get_adusers(string search)
        {
            try
            {
                _logger.LogInformation("AdUserController");
                _logger.LogInformation("Lets get the email addresses from AD with keywords: " + search);
                List<email_list> users_email = new List<email_list>();
                PrincipalContext context = new PrincipalContext(ContextType.Domain);
                UserPrincipal principal = new UserPrincipal(context);
                principal.EmailAddress = $"*{search}*";
                principal.Enabled = true;
                PrincipalSearcher searcher = new PrincipalSearcher(principal);
                var principal_users = searcher.FindAll().AsQueryable().Cast<UserPrincipal>().OrderBy(x => x.Surname).ToList();
                //return principal_users;
                //return AdUser.(principal);
                foreach (var p_users in principal_users)
                {

                    email_list email_List = new email_list();
                    email_List.name = p_users.DisplayName;
                    email_List.email = p_users.EmailAddress;
                    users_email.Add(email_List);
                }

                return Json(users_email);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("AdUserController");
                _logger.LogInformation("Error retrieving AD User");

                throw new Exception("Error retrieving AD User", ex);
            }
            //ViewBag.users = principal_users;
            //return Json("users");
        }
        public JsonResult getadusers()
        {

            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            UserPrincipal principal = new UserPrincipal(context);
            principal.EmailAddress = "*@*";
            principal.Enabled = true;
            PrincipalSearcher searcher = new PrincipalSearcher(principal);
            //var users = searcher.FindAll().AsQueryable().Cast<UserPrincipal>().OrderBy(x => x.Surname).ToList();
            var users = searcher.FindAll().Take(5000).AsQueryable().Cast<UserPrincipal>().OrderBy(x => x.Surname).ToList();
            //return users;
            var email = users[0].EmailAddress;

            ViewBag.users = users;
            return Json("Ok");
        }
    }
}
