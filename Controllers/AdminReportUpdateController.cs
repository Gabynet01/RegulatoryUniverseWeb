using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegulatoryUniverse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Web;
using Microsoft.Extensions.Logging;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse.Controllers
{
    public class AdminReportUpdateController : Controller
    {
        private readonly RegulatoryUniverseContext _context;
        private readonly MailRequest _mailRequest;
        private readonly IEmailService _sendEmail;

        private readonly ILogger<AdminReportUpdateController> _logger;
        private readonly ISessionManagerService _sessionManagerService;
        private readonly IHelperServices _helperServices;

        public AdminReportUpdateController(RegulatoryUniverseContext context, IEmailService emailService, IOptions<MailRequest> mailRequest, ILogger<AdminReportUpdateController> logger, ISessionManagerService sessionManagerService, IHelperServices helperServices)
        {
            _context = context;
            _sendEmail = emailService;
            _mailRequest = mailRequest.Value;
            _logger = logger;
            _helperServices = helperServices;
            _sessionManagerService = sessionManagerService;
        }



        // GET: ReportUpdate
        public async Task<IActionResult> Index()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            //count of reports based on status
            var allReportsCount = (from dataUpdates in _context.ReportUpdates
                                   select dataUpdates).Count();

            var allPendingCount = (from dataUpdates in _context.ReportUpdates
                                   where dataUpdates.Status.Contains("pending")
                                   select dataUpdates).Count();

            var allSentCount = (from dataUpdates in _context.ReportUpdates
                                where dataUpdates.Status.Contains("sent")
                                select dataUpdates).Count();


            var allAcknowledged = (from dataUpdates in _context.ReportUpdates
                                   where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                   select dataUpdates).Count();


            var result = (from dataSchedules in _context.ReportSchedule
                          join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId

                          orderby dataUpdates.CreatedDate descending
                          select new MergeReportData
                          {
                              ReportName = dataSchedules.ReportName,
                              ReportId = dataUpdates.ReportId,
                              ApplicableLawRegulation = dataSchedules.ApplicableLawRegulation,
                              Description = dataSchedules.Description,
                              ReceivingInstitution = dataSchedules.ReceivingInstitution,
                              ScheduleFrequency = dataSchedules.ScheduleFrequency,
                              ScheduleFrequencyDate = dataSchedules.ScheduleFrequencyDate,
                              ScheduleFrequencyTime = dataSchedules.ScheduleFrequencyTime,
                              ResponsibleDepartment = dataSchedules.ResponsibleDepartment,
                              Status = dataUpdates.Status,
                              CreatedBy = dataUpdates.CreatedBy,
                              CreatedDate = dataUpdates.CreatedDate,
                              ReportSentDate = dataUpdates.ReportSentDate,
                              AcknowledgeReceipt = dataUpdates.AcknowledgeReceipt,
                              AcknowledgeDate = dataUpdates.AcknowledgeDate,
                              AcknowledgeBy = dataUpdates.AcknowledgeBy,
                              Id = dataUpdates.Id,

                          }).ToList();


            ViewData["AllReportsCount"] = allReportsCount;
            ViewData["AllPendingCount"] = allPendingCount;
            ViewData["AllSentCount"] = allSentCount;
            ViewData["AllAcknowledged"] = allAcknowledged;

            _logger.LogInformation("AdminReportUpdateController");
            _logger.LogInformation("Index page loaded");
            return View(result);
        }

        // GET: ReportUpdate/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id == null)
            {
                return NotFound();
            }

            var username = HttpContext.Session.GetString("username");

            var reportUpdate = (from dataSchedules in _context.ReportSchedule
                                join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId

                                select new MergeReportData
                                {
                                    ReportName = dataSchedules.ReportName,
                                    ReportId = dataUpdates.ReportId,
                                    ApplicableLawRegulation = dataSchedules.ApplicableLawRegulation,
                                    Description = dataSchedules.Description,
                                    ReceivingInstitution = dataSchedules.ReceivingInstitution,
                                    ScheduleFrequency = dataSchedules.ScheduleFrequency,
                                    ScheduleFrequencyDate = dataSchedules.ScheduleFrequencyDate,
                                    ScheduleFrequencyTime = dataSchedules.ScheduleFrequencyTime,
                                    ResponsibleDepartment = dataSchedules.ResponsibleDepartment,
                                    Status = dataUpdates.Status,
                                    CreatedBy = dataUpdates.CreatedBy,
                                    CreatedDate = dataUpdates.CreatedDate,
                                    ReportSentDate = dataUpdates.ReportSentDate,
                                    AcknowledgeDate = dataUpdates.AcknowledgeDate,
                                    AcknowledgeReceipt = dataUpdates.AcknowledgeReceipt,
                                    AcknowledgeBy = dataUpdates.AcknowledgeBy,
                                    Id = dataUpdates.Id

                                }).FirstOrDefault(m => m.Id == id);


            //var reportUpdate = await _context.ReportUpdates
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (reportUpdate == null)
            {
                return NotFound();
            }

            _logger.LogInformation("Details loaded");

            return View(reportUpdate);
        }

        // GET: ReportUpdate/Create
        public IActionResult Create()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            ViewData["username"] = HttpContext.Session.GetString("username");

            _logger.LogInformation("AdminReportUpdateController");
            _logger.LogInformation("Create loaded");
            return View();
        }

        // POST: ReportUpdate/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReportId,Status,CreatedBy,CreatedDate")] ReportUpdates reportUpdate)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            //check if input is empty
            if (_helperServices.IsAnyNullOrEmpty(reportUpdate) == true)
            {
                ViewBag.errorMsg = "All fields are required";
                return View(reportUpdate);
            }

            //check if special characters are found
            if (_helperServices.IsHtmlCharactersFound(reportUpdate) == true)
            {
                ViewBag.errorMsg = "Input field contains special characters, kindly remove them";
                return View(reportUpdate);
            }

            if (ModelState.IsValid)
            {
                _context.Add(reportUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("AdminReportUpdateController");
            _logger.LogInformation("create to db success");
            return View(reportUpdate);
        }

        // GET: ReportUpdate/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id == null)
            {
                return NotFound();
            }

            var reportUpdate = await _context.ReportUpdates.FindAsync(id);
            if (reportUpdate == null)
            {
                return NotFound();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");

            _logger.LogInformation("AdminReportUpdateController");
            _logger.LogInformation("Edit success");
            return View(reportUpdate);
        }

        // POST: ReportUpdate/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReportId,Status,CreatedBy,CreatedDate,ReportSentDate,AcknowledgeReceipt,AcknowledgeDate,AcknowledgeBy")] ReportUpdates reportUpdate)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            //check if input is empty
            if (_helperServices.IsAnyNullOrEmpty(reportUpdate) == true)
            {
                ViewBag.errorMsg = "All fields are required";
                return View(reportUpdate);
            }

            //check if special characters are found
            if (_helperServices.IsHtmlCharactersFound(reportUpdate) == true)
            {
                ViewBag.errorMsg = "Input field contains special characters, kindly remove them";
                return View(reportUpdate);
            }

            if (id != reportUpdate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reportUpdate);
                    await _context.SaveChangesAsync();


                    var username = HttpContext.Session.GetString("username");

                    //send email here to user who updated the status report to sent that it has been acknowledged
                    //get the report name from DB
                    var reportName = (from dataSchedules in _context.ReportSchedule
                                      where dataSchedules.Id == reportUpdate.ReportId
                                      select dataSchedules.ReportName).FirstOrDefault();

                    var link = HttpContext.Request.Host.ToString() + "/ReportUpdate";

                    _mailRequest.ToEmail = reportUpdate.CreatedBy + "@cbg.com.gh";
                    _mailRequest.Subject = "Acknowledged" + " - " + reportName;

                    var body = "<html><head></head><body>" +
                             "<div><p>Hello " + reportUpdate.CreatedBy + ",</p>" +
                             "<p>Please be informed that <b>" + username + "</b> just acknowledged that this report (<b>" + reportName + "</b>) has truly been sent by you. </p>" +
                             "<p>Kindly click the link below to view the details of the report.</p>" +
                             "<p><a href=" + link + "/Details/" + reportUpdate.Id + " >Cick here to view the report</a></p>" +
                             "<p>Thank you </p><p>Best Regards,</p><p>Compliance Team</p><p></p></div>" +
                             "</body></html>";

                    // Encode the string.
                    string myEncodedString = HttpUtility.HtmlEncode(body);
                    _mailRequest.Body = myEncodedString;

                    var sentResp = await _sendEmail.SendMail_esb(_mailRequest);

                    _logger.LogInformation("AdminReportUpdateController");
                    _logger.LogInformation("email was sent here with response {0}", sentResp);

                    _logger.LogInformation("email sender status {0} and reportname is {1}", sentResp, reportName);

                    if (sentResp.ToUpper() == "FALSE")
                    {
                        _logger.LogInformation("AdminReportUpdateController");
                        _logger.LogInformation("Resending this email now--> {0}", "gabriel.gyandoh@cbg.com.gh");
                        //resend the email again
                        await _sendEmail.SendMail_esb(_mailRequest);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportUpdateExists(reportUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reportUpdate);
        }

        // GET: ReportUpdate/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            if (id == null)
            {
                return NotFound();
            }

            var reportUpdate = await _context.ReportUpdates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reportUpdate == null)
            {
                return NotFound();
            }

            return View(reportUpdate);
        }

        // POST: ReportUpdate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            var reportUpdate = await _context.ReportUpdates.FindAsync(id);
            _context.ReportUpdates.Remove(reportUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportUpdateExists(int id)
        {
            return _context.ReportUpdates.Any(e => e.Id == id);
        }
    }
}
