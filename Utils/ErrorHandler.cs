using System;
using System.Windows.Forms;

namespace MunicipalIssueReporter.Utils
{
    public static class ErrorHandler
    {
        public static void Handle(Exception ex, string userMessage = "An unexpected error occurred.")
        {
            MessageBox.Show($"{userMessage}\n\nDetails: {ex.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
