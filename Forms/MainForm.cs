using System;
using System.Windows.Forms;

namespace MunicipalIssueReporter.Forms
{
    public class MainForm : Form
    {
        private Label lblTitle;
        private Button btnReportIssues;
        private Button btnLocalEvents;
        private Button btnServiceStatus;
        private Button btnExit;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Municipal Services - Main Menu";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 600;
            Height = 400;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // Use a TableLayoutPanel for clean, responsive layout
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 1,
                Padding = new Padding(20)
            };

            // Evenly space rows
            for (int i = 0; i < 5; i++)
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            Controls.Add(layout);

            // --- Title Label ---
            lblTitle = new Label
            {
                Text = "Welcome — Municipal Services Portal",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold)
            };
            layout.Controls.Add(lblTitle, 0, 0);

            // --- Report Issues Button ---
            btnReportIssues = new Button
            {
                Text = "📝 Report Issues",
                Dock = DockStyle.Fill,
                Height = 50,
                Font = new System.Drawing.Font("Segoe UI", 12F)
            };
            btnReportIssues.Click += BtnReportIssues_Click;
            layout.Controls.Add(btnReportIssues, 0, 1);

            // --- Local Events Button ---
            btnLocalEvents = new Button
            {
                Text = "📅 Local Events and Announcements",
                Dock = DockStyle.Fill,
                Height = 50,
                Font = new System.Drawing.Font("Segoe UI", 12F)
            };
            btnLocalEvents.Click += BtnLocalEvents_Click;
            layout.Controls.Add(btnLocalEvents, 0, 2);

            // --- Service Request Status Button ---
            btnServiceStatus = new Button
            {
                Text = "🔍 Service Request Status (Coming Soon)",
                Dock = DockStyle.Fill,
                Height = 50,
                Font = new System.Drawing.Font("Segoe UI", 12F),
                Enabled = false
            };
            layout.Controls.Add(btnServiceStatus, 0, 3);

            // --- Exit Button ---
            btnExit = new Button
            {
                Text = "🚪 Exit Application",
                Dock = DockStyle.Fill,
                Height = 50,
                Font = new System.Drawing.Font("Segoe UI", 12F)
            };
            btnExit.Click += (s, e) => Application.Exit();
            layout.Controls.Add(btnExit, 0, 4);
        }

        // --- Event Handlers ---

        private void BtnReportIssues_Click(object sender, EventArgs e)
        {
            try
            {
                var reportForm = new ReportForm();
                reportForm.FormClosed += (s, args) => this.Show();
                this.Hide();
                reportForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Report Form.\n{ex.Message}",
                    "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLocalEvents_Click(object sender, EventArgs e)
        {
            try
            {
                var eventsForm = new LocalEventsForm();
                eventsForm.FormClosed += (s, args) => this.Show();
                this.Hide();
                eventsForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open Local Events page.\n{ex.Message}",
                    "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
