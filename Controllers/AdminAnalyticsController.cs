using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegulatoryUniverse.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Web;
using System.Globalization;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;
using RegulatoryUniverse.Services;

namespace RegulatoryUniverse.Controllers
{
    public class AdminAnalyticsController : Controller
    {
        private readonly RegulatoryUniverseContext _context;
        private readonly ILogger<AdminAnalyticsController> _logger;
        private readonly ISessionManagerService _sessionManagerService;

        public AdminAnalyticsController(RegulatoryUniverseContext context, ILogger<AdminAnalyticsController> logger, ISessionManagerService sessionManagerService)
        {
            _context = context;
            _logger = logger;
            _sessionManagerService = sessionManagerService;
        }

        public IActionResult Index()
        {
            //start session manager
            var sessionActiveState = _sessionManagerService.CheckBrowserSession();
            var isAdmin = _sessionManagerService.CheckUserIsAdmin();
            if (sessionActiveState == false || isAdmin == false)
            {
                return RedirectToAction("Index", "Login");
            }
            //end session manager

            //lets get a list of data to plot the analytics
            var allReportScheduleCount = (from data in _context.ReportSchedule
                                          select data).Count();

            var allDailyCount = (from data in _context.ReportSchedule
                                 where data.ScheduleFrequency.Contains("daily")
                                 select data).Count();

            var allWeeklyCount = (from data in _context.ReportSchedule
                                  where data.ScheduleFrequency.Contains("weekly")
                                  select data).Count();

            var allQuarterlyCount = (from data in _context.ReportSchedule
                                     where data.ScheduleFrequency.Contains("quarterly")
                                     select data).Count();

            var allMonthlyCount = (from data in _context.ReportSchedule
                                   where data.ScheduleFrequency.Contains("monthly")
                                   select data).Count();

            var allHalfYearCount = (from data in _context.ReportSchedule
                                    where data.ScheduleFrequency.Contains("halfyear")
                                    select data).Count();

            var allAnnualCount = (from data in _context.ReportSchedule
                                  where data.ScheduleFrequency.Contains("annually")
                                  select data).Count();


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

            //this will fetch data by comparing the two table

            var reportDailyCount = (from dataSchedules in _context.ReportSchedule
                                    join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                    where dataSchedules.ScheduleFrequency.Contains("daily")
                                    where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                    select dataUpdates).Count();

            var reportWeeklyCount = (from dataSchedules in _context.ReportSchedule
                                     join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                     where dataSchedules.ScheduleFrequency.Contains("weekly")
                                     where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                     select dataUpdates).Count();

            var reportMonthlyCount = (from dataSchedules in _context.ReportSchedule
                                      join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                      where dataSchedules.ScheduleFrequency.Contains("monthly")
                                      where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                      select dataUpdates).Count();

            var reportQuarterlyCount = (from dataSchedules in _context.ReportSchedule
                                        join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                        where dataSchedules.ScheduleFrequency.Contains("quarterly")
                                        where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                        select dataUpdates).Count();

            var reportHalfYearCount = (from dataSchedules in _context.ReportSchedule
                                       join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                       where dataSchedules.ScheduleFrequency.Contains("halfyear")
                                       where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                       select dataUpdates).Count();

            var reportAnnualCount = (from dataSchedules in _context.ReportSchedule
                                     join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                     where dataSchedules.ScheduleFrequency.Contains("annually")
                                     where dataUpdates.AcknowledgeReceipt.Contains("yes")
                                     select dataUpdates).Count();


            ViewData["reportDailyCount"] = reportDailyCount;
            ViewData["reportWeeklyCount"] = reportWeeklyCount;
            ViewData["reportMonthlyCount"] = reportMonthlyCount;
            ViewData["reportQuarterlyCount"] = reportQuarterlyCount;
            ViewData["reportHalfYearCount"] = reportHalfYearCount;
            ViewData["reportAnnualCount"] = reportAnnualCount;

            ViewData["AllReportsCount"] = allReportsCount;
            ViewData["AllPendingCount"] = allPendingCount;
            ViewData["AllSentCount"] = allSentCount;
            ViewData["AllAcknowledged"] = allAcknowledged;
            ViewData["AllReportScheduleCount"] = allReportScheduleCount;

            ViewData["AllDailyCount"] = allDailyCount;
            ViewData["AllWeeklyCount"] = allWeeklyCount;
            ViewData["AllMonthlyCount"] = allMonthlyCount;
            ViewData["AllQuarterlyCount"] = allQuarterlyCount;
            ViewData["AllHalfYearCount"] = allHalfYearCount;
            ViewData["AllAnnualCount"] = allAnnualCount;

            _logger.LogInformation("AdminAnalyticsController");
            _logger.LogInformation("Admin analytics index loaded with data");

            return View();
        }

        //all the forms parameters are selected here
        [AcceptVerbs("Post")]
        public JsonResult ReportScheduleByStatus(string reportStatus)
        {
            var reportDailyCount = 0;
            var reportWeeklyCount = 0;
            var reportMonthlyCount = 0;
            var reportQuarterlyCount = 0;
            var reportHalfYearCount = 0;
            var reportAnnualCount = 0;



            reportDailyCount = (from dataSchedules in _context.ReportSchedule
                                join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                where dataSchedules.ScheduleFrequency.Contains("daily")
                                where dataUpdates.Status.Contains(reportStatus)
                                select dataUpdates).Count();

            reportWeeklyCount = (from dataSchedules in _context.ReportSchedule
                                 join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                 where dataSchedules.ScheduleFrequency.Contains("weekly")
                                 where dataUpdates.Status.Contains(reportStatus)
                                 select dataUpdates).Count();

            reportMonthlyCount = (from dataSchedules in _context.ReportSchedule
                                  join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                  where dataSchedules.ScheduleFrequency.Contains("monthly")
                                  where dataUpdates.Status.Contains(reportStatus)
                                  select dataUpdates).Count();

            reportQuarterlyCount = (from dataSchedules in _context.ReportSchedule
                                    join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                    where dataSchedules.ScheduleFrequency.Contains("quarterly")
                                    where dataUpdates.Status.Contains(reportStatus)
                                    select dataUpdates).Count();

            reportHalfYearCount = (from dataSchedules in _context.ReportSchedule
                                   join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                   where dataSchedules.ScheduleFrequency.Contains("halfyear")
                                   where dataUpdates.Status.Contains(reportStatus)
                                   select dataUpdates).Count();

            reportAnnualCount = (from dataSchedules in _context.ReportSchedule
                                 join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                 where dataSchedules.ScheduleFrequency.Contains("annually")
                                 where dataUpdates.Status.Contains(reportStatus)
                                 select dataUpdates).Count();



            ReportScheduleByStatusData allData = new ReportScheduleByStatusData
            {
                code = "200",
                reportDailyCount = reportDailyCount,
                reportAnnualCount = reportAnnualCount,
                reportHalfYearCount = reportHalfYearCount,
                reportMonthlyCount = reportMonthlyCount,
                reportQuarterlyCount = reportQuarterlyCount,
                reportWeeklyCount = reportWeeklyCount
            };

            _logger.LogInformation("AdminAnalyticsController");
            _logger.LogInformation("allData pulled for the JS call ReportScheduleByStatus-->{0}", allData.ToString());

            return Json(allData);

        }


        //all the forms parameters are selected here
        [AcceptVerbs("Post")]
        public JsonResult ReportAnalyticsBySearch(string selectScheduleFrequency, string selectReportStatus, string fromReportDateRange, string toReportDateRange)
        {
            //lets pick all the values here 

            //lets explode the array values
            string[] splitSchedules = selectScheduleFrequency.Split(",");
            string[] splitStatus = selectReportStatus.Split(",");

            var scheduleDynamicQuery = "";
            var statusDynamicQuery = "";

            //loop through the splitSchedules array
            for (var i = 0; i < splitSchedules.Count(); i++)
            {
                //only add || if it is not the last item
                if (i < (splitSchedules.Count() - 1))
                {

                    scheduleDynamicQuery += "ScheduleFrequency == \"" + splitSchedules[i] + "\" OR ";
                }
                else
                {
                    scheduleDynamicQuery += "ScheduleFrequency == \"" + splitSchedules[i] + "\"";
                }
            }

            //loop through the split status array
            //loop through the array
            for (var i = 0; i < splitStatus.Count(); i++)
            {
                //only add || if it is not the last item
                if (i < (splitStatus.Count() - 1))
                {

                    statusDynamicQuery += "status == \"" + splitStatus[i] + "\" OR ";
                }
                else
                {
                    statusDynamicQuery += "status == \"" + splitStatus[i] + "\"";
                }
            }

            _logger.LogInformation("AdminAnalyticsController");
            _logger.LogInformation("queries--> {0}", scheduleDynamicQuery);
            _logger.LogInformation("schedule {0}, reportStatus {1}, fromDate {2}, toDate {3}", selectScheduleFrequency, selectReportStatus, fromReportDateRange, toReportDateRange);

            var fromDate = fromReportDateRange;
            var toDate = toReportDateRange;

            DateTime newFromDate = DateTime.ParseExact(fromDate,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture);

            DateTime newToDate = DateTime.ParseExact(toDate,
                        "yyyy-MM-dd", CultureInfo.InvariantCulture);

            var getReportAnalyticsData = (from dataSchedules in _context.ReportSchedule
                                          join dataUpdates in _context.ReportUpdates on dataSchedules.Id equals dataUpdates.ReportId
                                          where dataUpdates.CreatedDate.Date >= newFromDate.Date && dataUpdates.CreatedDate.Date <= newToDate.Date
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

                                          }).Where(scheduleDynamicQuery).Where(statusDynamicQuery);


            ReportAnalyticsBySearchData allData = new ReportAnalyticsBySearchData
            {
                code = "200",
                reportCount = getReportAnalyticsData.Count(),
                reportData = getReportAnalyticsData.ToList(),
                message = "Success"

            };

            _logger.LogInformation("allData-->{0}", allData.ToString());
            return Json(allData);

        }

    }
}
