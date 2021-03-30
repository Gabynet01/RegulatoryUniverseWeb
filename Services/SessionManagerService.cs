using RegulatoryUniverse.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace RegulatoryUniverse.Services
{
    public class SessionManagerService : ISessionManagerService
    {

        private readonly RegulatoryUniverseContext _context;
        private readonly ILogger<SessionManagerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManagerService(RegulatoryUniverseContext context, IHttpContextAccessor httpContextAccessor, ILogger<SessionManagerService> logger)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public bool CheckBrowserSession()
        {
            _logger.LogInformation("Checking Browser session if it is active or not");
            var isLoggedIn = _httpContextAccessor.HttpContext.Session.GetString("isLoggedIn");
            var isSessionActive = true;
       
            if (isLoggedIn == null) {
                _logger.LogInformation("Session has expired... redirecting to login page");
                //session has expired
                isSessionActive = false;
            } 
            else {
                _logger.LogInformation("Browser session is still active, lets check if session token has changed");
                //session is still active but check if token is the same as what is in the DB
                var sessionUser = _httpContextAccessor.HttpContext.Session.GetString("username");
                //Lets check if user session exists
                var getLoggedInUserData = (from userSessionData in _context.LoginManager
                                           where userSessionData.Username == sessionUser
                                           select userSessionData
                ).FirstOrDefault();

                var sessionToken = _httpContextAccessor.HttpContext.Session.GetString("loginToken");
                var dbToken = getLoggedInUserData.Token;
                //lets compare both
                if (sessionToken.ToUpper() != dbToken.ToUpper())
                {
                    _logger.LogInformation("session Token does not match with DB Token.... redirecting to login page");

                    //therefore session is no longer active on on the first logged in browser
                    isSessionActive = false;

                }
               
            }
            return isSessionActive;
        }

        //check if user is an admin
        public bool CheckUserIsAdmin()
        {
            _logger.LogInformation("Checking user is an admin or not");
            var adminRole = _httpContextAccessor.HttpContext.Session.GetString("userRole");
            var isAdmin = true;

            if (adminRole == null)
            {
                _logger.LogInformation("role not specified... redirecting to login page");
                //session has expired
                isAdmin = false;
            }
            else
            {
                _logger.LogInformation("Role specified, lets check if role is an Admin");
                _logger.LogInformation("Role session is --->>> {0}", adminRole.ToUpper());
                
                
                //lets compare both
                if (adminRole.ToUpper() != "ADMINISTRATOR" && adminRole.ToUpper() != "COMPLIANCEADMIN")
                {
                    _logger.LogInformation("Admin role is not an administrator nor compliance admin");

                    //Unauthorised access
                    isAdmin = false;

                }

            }
            return isAdmin;
        }

    }
}
