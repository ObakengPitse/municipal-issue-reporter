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
                if (string.IsNullOrWhiteSpace(issue.Location))
                    throw new ArgumentException("Location cannot be empty.");
                if (string.IsNullOrWhiteSpace(issue.Category))
                    throw new ArgumentException("Category must be selected.");

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
