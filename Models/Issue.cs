using System;
using System.Collections.Generic;

namespace MunicipalIssueReporter.Models
{
    [Serializable]
    public class Issue
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TrackingId => Id.ToString().Split('-')[0].ToUpper(); // short human-friendly id
        public string Location { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Attachments { get; set; } = new List<string>();
        public DateTime ReportedAt { get; set; } = DateTime.Now;

        // New fields for status/prioritisation and simple lifecycle
        public string Status { get; set; } = "Submitted"; // Submitted, In Progress, Completed
        public int Priority { get; set; } = 0; // higher = more urgent

        public override string ToString()
        {
            return $"{TrackingId} | {Category} | {Location} | {Status} | {ReportedAt:g}";
        }
    }
}
