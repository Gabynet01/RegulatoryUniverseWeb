using RegulatoryUniverse.Models;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.ServiceModel;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace RegulatoryUniverse.Services
{
    public class HelperServices : IHelperServices
    {
        private readonly UserProfileServiceDbContext _context;
        private readonly RegulatoryUniverseContext _mainContext;
        private readonly ILogger<HelperServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HelperServices(UserProfileServiceDbContext context, RegulatoryUniverseContext mainContext, IHttpContextAccessor httpContextAccessor, ILogger<HelperServices> logger)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mainContext = mainContext;
        }

        //fetch email user lists
        public string GetMailNotificationList()
        {
            _logger.LogInformation("fetch email manager lists");

            var getAllEmails = (from data in _mainContext.MailListManager
                                where data.Class == "Admin Notifications"
                                select data.Email
                               ).ToList();

            if (getAllEmails.Count == 0)
            {
                _logger.LogDebug("No User Found");
                return "No Email Found";
            }
            
            string combindedEmails = string.Join(";", getAllEmails);
            _logger.LogDebug("Email list is ready");
            _logger.LogInformation("final Email array list is --->>> {0}", combindedEmails);

            return combindedEmails;
        }

        //Generate random string 
        public string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var list = Enumerable.Repeat(0, 8).Select(x => chars[random.Next(chars.Length)]);
            return string.Join("", list);
        }

        //parse XML Response to JSON
        public JToken ParseXmlResponseToJson(string responseBody)
        {
            //parse the XML to JSON 
            _logger.LogInformation("Parsing XML Body to JSON");
            XmlDocument doc1 = new XmlDocument();
            doc1.LoadXml(responseBody);
            string json = JsonConvert.SerializeXmlNode(doc1);
            var finalFormattedJsonObject = JObject.Parse(json);

            _logger.LogInformation("Parsing the XML Body response to --> {0}", finalFormattedJsonObject.ToString());

            var responseResult = finalFormattedJsonObject["result"];

            _logger.LogInformation("Response Result String--->>>>{0}", responseResult);
            _logger.LogInformation("Record Status String--->>>>{0}", responseResult["recordStatus"]);
            _logger.LogInformation("Response data String--->>>>{0}", responseResult["responseData"]);

            return responseResult;
        }

        //function to check if an object is empty
        public bool IsAnyNullOrEmpty(object myObject)
        {
            _logger.LogInformation("Checking if an object has an empty or Null value");
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsHtmlCharactersFound(object myObject)
        {
            _logger.LogInformation("Checking if an object has any special characters");
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (!string.IsNullOrEmpty(value) && IsHtmlEncoded(value))
                    {
                        return true;
                    }
                    //lets check for special characters
                    var encodedText = HttpUtility.JavaScriptStringEncode(value);
                    _logger.LogInformation("Encoded text -->{0} and Boolean --> {1}", encodedText, IsHtmlEncoded(value));

                }
            }
            return false;
        }

        private bool IsHtmlEncoded(string text)
        {
            return (HttpUtility.JavaScriptStringEncode(text) != text);
        }

    }
}
