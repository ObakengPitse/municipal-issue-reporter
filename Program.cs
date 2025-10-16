using System;
using System.Windows.Forms;
using MunicipalIssueReporter.Forms;
using MunicipalIssueReporter.Utils;

namespace MunicipalIssueReporter
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Run app from a separate startup method for OOP clarity
                AppStartup.Run();
            }
            catch (Exception ex)
            {
                // Catch any unhandled startup-level exceptions
                ErrorHandler.Handle(ex, "The application failed to start correctly.");
            }
        }
    }

    /// <summary>
    /// Responsible for initializing and launching the main application form.
    /// Keeps Program.Main() clean and readable.
    /// </summary>
    public static class AppStartup
    {
        public static void Run()
        {
            try
            {
                // You can later add login or splash screen logic here if needed
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "An error occurred while running the application.");
            }
        }
    }
}
