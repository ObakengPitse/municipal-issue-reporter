using System;
using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Utils;

namespace MunicipalIssueReporter.Services
{
    public class IssueService
    {
        public bool SubmitIssue(Issue issue)
        {
            try
            {
                // -------------------- VALIDATION --------------------
                if (string.IsNullOrWhiteSpace(issue.Location))
                    throw new ArgumentException("Location cannot be empty.");

                if (string.IsNullOrWhiteSpace(issue.Category))
                    throw new ArgumentException("Category must be selected.");

                // -------------------- PRIORITY CALCULATION --------------------
                int priority = 0;

                // Category selected → +10
                if (!string.IsNullOrWhiteSpace(issue.Category))
                    priority += 10;

                // Attachments → +5 each
                priority += (issue.Attachments?.Count ?? 0) * 5;

                // Description provided → +20
                if (!string.IsNullOrWhiteSpace(issue.Description))
                    priority += 20;

                // Clamp priority between 0 and 100
                priority = Math.Max(0, Math.Min(priority, 100));

                issue.Priority = priority;
                // --------------------------------------------------------------

                // Save to repository
                IssueRepository.Add(issue);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to submit issue.");
                return false;
            }
        }
    }
}
