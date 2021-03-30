using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using RegulatoryUniverse.Models;
using RestSharp;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.ServiceModel;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse
{
    public interface IEmailService
    {
        void Send(string from, string to, string subject, string html);
        Task<string> SendMail_esb(MailRequest request);
    }

    public class EmailService : IEmailService
    {
        //private readonly MailSettings _appSettings;
        private readonly MailRequest _mailRequest;
        private readonly ILogger<EmailService> _logger;
        private readonly IHelperServices _helperServices;
        private String APIKEY = "1f1520a7-916b-46d7-8eda-5f5daa2ef365";
        //private String APIKEY = "CBG";

        public EmailService(IOptions<MailSettings> appSettings, IOptions<MailRequest> mailRequest, ILogger<EmailService> logger, IHelperServices helperServices)
        {
            _logger = logger;
            //_appSettings = appSettings.Value;
            _mailRequest = mailRequest.Value;
            _helperServices = helperServices;
        }

        public void Send(string from, string to, string subject, string html)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            //smtp.Connect(_appSettings.SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.StartTls);
            //smtp.Authenticate(_appSettings.SmtpUser, _appSettings.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public async Task<string> SendMail_esb(MailRequest request)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain,
                    System.Net.Security.SslPolicyErrors errors)
            {
                if (errors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    return true;
                }
                return true;
            };
            //var client = new RestClient("http://172.30.200.41:8055/BridgeSOAPService.asmx?wsdl");     

            var client = new RestClient("https://cbgbridge.apps.cbg.com.gh/BridgeSOAPService.asmx");
            client.Timeout = -1;
            var email_request = new RestRequest(Method.POST);
            email_request.AddHeader("Content-Type", "text/xml");
            email_request.AddParameter("text/xml", "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:tem=\"http://tempuri.org/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <tem:SendEMail>\r\n         <!--Optional:-->\r\n         <tem:to>" + request.ToEmail + "</tem:to>\r\n         <!--Optional:-->\r\n         <tem:cc>?</tem:cc>\r\n         <!--Optional:-->\r\n         <tem:bc>?</tem:bc>\r\n         <!--Optional:-->\r\n         <tem:subject>" + request.Subject + "</tem:subject>\r\n         <!--Optional:-->\r\n         <tem:message>" + request.Body + "</tem:message>\r\n         <!--Optional:-->\r\n         <tem:attached>?</tem:attached>\r\n         <!--Optional:-->\r\n         <tem:APIKey>"+APIKEY+"</tem:APIKey>\r\n      </tem:SendEMail>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>", ParameterType.RequestBody);
            IRestResponse response = client.Execute(email_request);
            Console.WriteLine(response.Content);

            string xmlresponse = Convert.ToString(response.Content);

            XmlDocument doc1 = new XmlDocument();
            doc1.LoadXml(response.Content);
            string json = JsonConvert.SerializeXmlNode(doc1);
            object json1 = JObject.Parse(json);
            string json2string = json.ToString();
            Console.WriteLine(json2string);


            string[] jsonList = json.Split(",");
            //Console.WriteLine(jsonList);
            //foreach (string responses in jsonList)
            //{
            //    Console.WriteLine(responses);
            //}
            //Console.WriteLine(jsonList[6]);

            string[] jsonList_last = jsonList[6].Split("\"");
            //Console.WriteLine(firstStringPosition_ft);
            //Console.WriteLine(secondStringPosition_ft);
            var request_status = jsonList_last[3];
            Console.WriteLine(request_status);
            return request_status;
        }


        //public async Task<string> SendMail_esb(MailRequest request)
        //{

        //    _logger.LogInformation("EmailServiceController");

        //    //lets call the Soap Service and 
        //    CBGBridgeSOAPService.BridgeSOAPServiceSoapClient serviceSoapClient = new CBGBridgeSOAPService.
        //        BridgeSOAPServiceSoapClient(new BasicHttpsBinding(BasicHttpsSecurityMode.Transport),
        //        new EndpointAddress("https://cbgbridge.apps.cbg.com.gh/BridgeSOAPService.asmx"));

        //    _logger.LogInformation("CBG Bridge Service initiated");

        //    //set default response status 
        //    var responseStatus = "";

        //    _logger.LogInformation("Sending email with ffg request email_addresses:->{0}", request.ToEmail);
        //    var sendEmailResp = await serviceSoapClient.SendEMailAsync(request.ToEmail, "", "", request.Subject, request.Body, "", APIKEY);

        //    //get the response body which is always in XML format
        //    var responseBody = sendEmailResp.Body.SendEMailResult;

        //    _logger.LogInformation("response body of the email server is ---->> {0}", responseBody.ToString());

        //    responseStatus = responseBody.ToString();

        //    return responseStatus;
        //}
    }
}