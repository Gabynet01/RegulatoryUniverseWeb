using RegulatoryUniverse.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RegulatoryUniverse.Services
{
    //interface to HelperServices in services
    public interface IHelperServices
    {
        string RandomString();
        JToken ParseXmlResponseToJson(string responseBody);
        bool IsAnyNullOrEmpty(object myObject);
        bool IsHtmlCharactersFound(object myObject);
        string GetMailNotificationList();

    }
}
