using System;
using System.Collections.Generic;

namespace RegulatoryUniverse.Models
{
    public partial class ReportUpdates
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ReportSentDate { get; set; }
        public string AcknowledgeReceipt { get; set; }
        public string AcknowledgeDate { get; set; }
        public string AcknowledgeBy { get; set; }

        public virtual ReportSchedule Report { get; set; }
    }

    public class MergeReportData
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string AcknowledgeReceipt { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AcknowledgeDate { get; set; }
        public string AcknowledgeBy { get; set; }
        public DateTime ReportSentDate { get; set; }
        public string ReportName { get; set; }
        public string ReceivingInstitution { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ScheduleFrequencyDate { get; set; }
        public string ScheduleFrequencyTime { get; set; }
        public string Description { get; set; }
        public string ApplicableLawRegulation { get; set; }
        public string ResponsibleDepartment { get; set; }
        public string AssignedEmails { get; set; }
        //public int AllReportsCount{ get; set; }
        //public int AllPendingCount { get; set; }
        //public int AllSentCount { get; set; }

    }

    public class ReportScheduleByStatusData
    {
        public string code { get; set; }
        public int reportDailyCount { get; set; }
        public int reportWeeklyCount { get; set; }
        public int reportMonthlyCount { get; set; }
        public int reportQuarterlyCount { get; set; }
        public int reportHalfYearCount { get; set; }
        public int reportAnnualCount { get; set; }
        public string message { get; set; }
    }

    public class ReportAnalyticsBySearchData
    {
        public string code { get; set; }
        public int reportCount { get; set; }
        public List<MergeReportData> reportData { get; set; }
        public string message { get; set; }
    }
}
