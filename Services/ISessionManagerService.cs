using RegulatoryUniverse.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace RegulatoryUniverse.Services
{
    //interface to SessionManagerService in services
    public interface ISessionManagerService
    {
        bool CheckBrowserSession();
        bool CheckUserIsAdmin();
    }
}
