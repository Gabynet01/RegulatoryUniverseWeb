using System;
using System.Collections.Generic;
using System.Text;

namespace RegulatoryUniverse.Models
{
    public class MailSettings
    {
        public string Secret { get; set; }

        // refresh token time to live (in days), inactive tokens are
        // automatically deleted from the database after this time
        public int RefreshTokenTTL { get; set; }

        public string EmailFrom { get; set; }
        public string SmtpHost = "smtp.office365.com";
        public int SmtpPort = 587;
        public string SmtpUser = "svc.alerts@cbg.com.gh";
        public string SmtpPass = "?LHK{8\\hh(Be";
    }

    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    public class response_model
    {
        public string SendEMailResult { get; set; }
    }
}
