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
using Microsoft.Extensions.Logging;

namespace RegulatoryUniverse.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserProfileServiceDbContext _context;
        private readonly RegulatoryUniverseContext _contextMain;
        private readonly ILogger<LoginController> _logger;

        public LoginController(UserProfileServiceDbContext context, RegulatoryUniverseContext contextMain, ILogger<LoginController> logger)
        {
            _context = context;
            _contextMain = contextMain;
            _logger = logger;
        }

        // GET: Login
        public IActionResult Index()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        public JsonResult ProcessLoginData(string formData)
        {
            _logger.LogInformation("LoginController");
            _logger.LogInformation("Logginnnnnn dddaaaaattttaaaa");

            //since the data coming from AJAX is a string, lets deserialize
            var myDetails = JsonConvert.DeserializeObject<Users>(formData);
            var formUsername = myDetails.Username;
            var formPassword = myDetails.Password;

            if (formUsername.Contains("@"))
            {

                _logger.LogInformation("LoginController");
                _logger.LogInformation("User is loggin in with email address");

                string[] username_explode = formUsername.Split("@");
                //Console.WriteLine(jsonList);
                formUsername = username_explode[0];

            }


            var user = (from data in _context.Users
                        where data.Username == formUsername
                        where data.IsActive
                        select data
                       ).FirstOrDefault();


            if (user == null)
            {
                StoreAppData unauthorisedData = new StoreAppData
                {
                    code = "204",
                    message = "OOPS! You are not Authorised to use this app",
                    data = formUsername
                };

                _logger.LogInformation("LoginController");
                _logger.LogInformation("Unauthorised to use this app");

                return Json(unauthorisedData);

            }

            else
            {
                //get the user IDs and full names
                var userId = user.UserId;
                var userFullName = user.FullName;

                //get only the user role ID
                var userRole = (from dataUserRole in _context.UserRoles
                                where dataUserRole.UserId == userId
                                where dataUserRole.RoleId == 2019 || dataUserRole.RoleId == 2020 || dataUserRole.RoleId == 2
                                select dataUserRole).FirstOrDefault();


                //check if it is null or not null
                if (userRole == null)
                {
                    StoreAppData noRoleAssignedData = new StoreAppData
                    {
                        code = "204",
                        message = "OOPS! Kindly see admin to assign a role to this account and try again",
                        data = formUsername
                    };

                    _logger.LogInformation("LoginController");
                    _logger.LogInformation("User has not been added to database");
                    return Json(noRoleAssignedData);
                }

                else
                {
                    //assign the value of the Role ID to a variable
                    var derivedRoleId = userRole.RoleId;

                    //lets get the text equivalent of the roleID
                    var Roles = (from dataRole in _context.Roles
                                 where dataRole.RoleId == derivedRoleId
                                 select dataRole).FirstOrDefault();

                    var userRoleName = Roles.Name;

                    using (var adcontext = new PrincipalContext(ContextType.Domain, "cbg", "DC=cbg,DC=com"))
                    {
                        if (adcontext.ValidateCredentials(formUsername, formPassword))
                        {
                            //check if user already exists in login manager db
                            //Lets check if user session exists
                            var checkIfUserIsLoggedIn = (from checkLoginSessionData in _contextMain.LoginManager
                                                         where checkLoginSessionData.Username == formUsername
                                                         select checkLoginSessionData
                                                         ).FirstOrDefault();

                            _logger.LogInformation("checkIfUserIsLoggedIn {0}", checkIfUserIsLoggedIn);

                            var tokenKey = RandomString();

                            _logger.LogInformation("generated Token {0}", tokenKey);


                            var createSessionAction = 0;
                            var loginMessage = "";

                            //save these into the Login Manager DB
                            try
                            {
                                //check if user session has been created before
                                if (checkIfUserIsLoggedIn == null)
                                {
                                    LoginManager createLoginSessionData = new LoginManager();

                                    //create the object here
                                    createLoginSessionData.Username = formUsername;
                                    createLoginSessionData.Token = tokenKey;
                                    createLoginSessionData.LoggedInDate = System.DateTime.Now;

                                    //create it 
                                    _contextMain.LoginManager.Add(createLoginSessionData);
                                    createSessionAction = _contextMain.SaveChanges();

                                    loginMessage = "Login Successful";

                                    _logger.LogInformation("I am creating it {0}", createSessionAction);
                                }
                                else
                                {
                                    //since user exists, lets update the Token
                                    var findLoggedInUser = _contextMain.LoginManager.FindAsync(checkIfUserIsLoggedIn.Id).Result;
                                    findLoggedInUser.Token = tokenKey;
                                    //update it
                                    _contextMain.LoginManager.Update(findLoggedInUser);
                                    createSessionAction = _contextMain.SaveChanges();

                                    loginMessage = "New login detected, Old session cleared";

                                    _logger.LogInformation("I am updating login manager {0}", createSessionAction);

                                }

                                if (createSessionAction == 1)
                                {
                                    StoreAppData successData = new StoreAppData
                                    {
                                        code = "200",
                                        message = loginMessage,
                                        data = formUsername
                                    };
                                    HttpContext.Session.SetString("isLoggedIn", "YES");
                                    HttpContext.Session.SetString("loginToken", tokenKey);
                                    HttpContext.Session.SetString("username", formUsername);
                                    HttpContext.Session.SetString("userRole", userRoleName);
                                    HttpContext.Session.SetString("fullname", userFullName);

                                    _logger.LogInformation("LoginController");
                                    _logger.LogInformation("Successful login");

                                    return Json(successData);

                                }

                                //login session data could not be created
                                else
                                {
                                    StoreAppData noLoginSessionCreated = new StoreAppData
                                    {
                                        code = "204",
                                        message = "OOPS! Login session could not be created, please try again",
                                        data = formUsername
                                    };

                                    return Json(noLoginSessionCreated);

                                }

                            }
                            catch (Exception ex)
                            {

                                StoreAppData noLoginSessionCreated = new StoreAppData
                                {
                                    code = "204",
                                    message = "OOPS! An error occured trying to create login session data, please try again",
                                    data = formUsername
                                };

                                return Json(noLoginSessionCreated);

                            }

                        }

                        if (adcontext.ValidateCredentials(formUsername, formPassword) == false)
                        {

                            StoreAppData incorrectLoginData = new StoreAppData
                            {
                                code = "204",
                                message = "OOPS! Incorrect username and password",
                                data = formUsername
                            };

                            _logger.LogInformation("LoginController");
                            _logger.LogInformation("Incorrect username and password");

                            return Json(incorrectLoginData);
                        }
                    }
                }

            }


            StoreAppData allData = new StoreAppData
            {
                code = "204",
                message = "OOPS! You are not Authorised to use this app",
                data = formUsername
            };

            _logger.LogInformation("LoginController");
            _logger.LogInformation("Unauthorised to use this app");

            return Json(allData);
        }

        public static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var list = Enumerable.Repeat(0, 8).Select(x => chars[random.Next(chars.Length)]);
            return string.Join("", list);
        }

        //Logout user
        public IActionResult LogoutUser()
        {

            _logger.LogInformation("LoginController");
            _logger.LogInformation("Loggin out user from this application and login manager session cleared");

            var usernameData = HttpContext.Session.GetString("username");

            var checkIfUserIsLoggedIn = (from checkLoginSessionData in _contextMain.LoginManager
                                         where checkLoginSessionData.Username == usernameData
                                         select checkLoginSessionData
                                         ).FirstOrDefault();

            if (checkIfUserIsLoggedIn != null)
            {
                //let us delete this user from LoginManager DB
                _contextMain.LoginManager.Remove(checkIfUserIsLoggedIn);
                _contextMain.SaveChanges();
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}
