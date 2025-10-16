using System;
using System.Collections.Generic;

namespace MunicipalIssueReporter.Models
{
    [Serializable]
    public class Issue
    {
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Attachments { get; set; } = new List<string>();
        public DateTime ReportedAt { get; set; }
    }
}
