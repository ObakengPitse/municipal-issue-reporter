using System;

namespace MunicipalIssueReporter.Models
{
    [Serializable]
    public class EventItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; } = 0; // higher = more important
        public string Location { get; set; } = string.Empty;

        public override string ToString() => $"{Title} ({Category}) - {StartDate:d}";
    }
}
