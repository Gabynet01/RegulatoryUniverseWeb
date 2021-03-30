using System;
using System.Collections.Generic;

namespace RegulatoryUniverse.Models
{
    public partial class MailListManager
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Class { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
