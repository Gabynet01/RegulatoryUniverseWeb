using System;
using System.Collections.Generic;

namespace RegulatoryUniverse.Models
{
    public partial class ReportSchedule
    {
        public ReportSchedule()
        {
            ReportUpdates = new HashSet<ReportUpdates>();
        }

        public int Id { get; set; }
        public string ReportName { get; set; }
        public string ReceivingInstitution { get; set; }
        public string ScheduleFrequency { get; set; }
        public string ScheduleFrequencyDate { get; set; }
        public string ScheduleFrequencyTime { get; set; }
        public string Description { get; set; }
        public string ApplicableLawRegulation { get; set; }
        public string ResponsibleDepartment { get; set; }
        public string AssignedEmails { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<ReportUpdates> ReportUpdates { get; set; }
    }
}
