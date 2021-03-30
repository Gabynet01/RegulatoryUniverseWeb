using System;
using System.Collections.Generic;

namespace RegulatoryUniverse.Models
{
    public partial class LoginManager
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime LoggedInDate { get; set; }
    }
}
