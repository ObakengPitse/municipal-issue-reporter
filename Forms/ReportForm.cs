using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MunicipalIssueReporter.Forms
{
    public class ReportForm : Form
    {
        private TextBox txtLocation;
        private ComboBox cbCategory;
        private RichTextBox rtbDescription;
        private ListBox lbAttachments;
        private ProgressBar progressBar;
        private Label lblEngagement;

        private readonly IssueService _issueService = new IssueService();

        public ReportForm()
        {
            InitializeComponents();
            AttachEvents();
            UpdateEngagement();
        }

        private void InitializeComponents()
        {
            Text = "Report Issue";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 800;
            Height = 600;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                Padding = new Padding(10),
                AutoScroll = true
            };
            Controls.Add(layout);

            txtLocation = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Enter location..." };
            cbCategory = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            cbCategory.Items.AddRange(new string[] { "Sanitation", "Roads", "Utilities", "Parks", "Lighting", "Other" });
            rtbDescription = new RichTextBox { Dock = DockStyle.Fill, Height = 150 };
            lbAttachments = new ListBox { Dock = DockStyle.Fill, Height = 80 };
            progressBar = new ProgressBar { Dock = DockStyle.Fill, Minimum = 0, Maximum = 100, Height = 20 };
            lblEngagement = new Label { Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };

            var btnAttach = new Button { Text = "Attach images/documents...", Dock = DockStyle.Fill };
            btnAttach.Click += BtnAttach_Click;

            var btnSubmit = new Button { Text = "Submit", Dock = DockStyle.Fill };
            btnSubmit.Click += BtnSubmit_Click;

            var btnBack = new Button { Text = "Back to Main Menu", Dock = DockStyle.Fill };
            btnBack.Click += (s, e) => { new MainForm().Show(); Close(); };

            layout.Controls.Add(new Label { Text = "Location", Dock = DockStyle.Fill });
            layout.Controls.Add(txtLocation);
            layout.Controls.Add(new Label { Text = "Category", Dock = DockStyle.Fill });
            layout.Controls.Add(cbCategory);
            layout.Controls.Add(new Label { Text = "Description", Dock = DockStyle.Fill });
            layout.Controls.Add(rtbDescription);
            layout.Controls.Add(btnAttach);
            layout.Controls.Add(lbAttachments);
            layout.Controls.Add(progressBar);
            layout.Controls.Add(lblEngagement);
            layout.Controls.Add(btnSubmit);
            layout.Controls.Add(btnBack);
        }

        private void AttachEvents()
        {
            // Trigger progress updates when user interacts with inputs
            txtLocation.TextChanged += (s, e) => UpdateEngagement();
            cbCategory.SelectedIndexChanged += (s, e) => UpdateEngagement();
            rtbDescription.TextChanged += (s, e) => UpdateEngagement();
            lbAttachments.SelectedIndexChanged += (s, e) => UpdateEngagement();
        }

        private void BtnAttach_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Multiselect = true;
                    dlg.Filter = "All Files|*.*";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var f in dlg.FileNames)
                        {
                            if (!lbAttachments.Items.Contains(f))
                                lbAttachments.Items.Add(f);
                        }
                        UpdateEngagement();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to attach files: {ex.Message}", "Attachment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            var issue = new Issue
            {
                Location = txtLocation.Text.Trim(),
                Category = cbCategory.SelectedItem?.ToString() ?? "Other",
                Description = rtbDescription.Text.Trim(),
                Attachments = new List<string>(),
                ReportedAt = DateTime.Now
            };

            foreach (var item in lbAttachments.Items)
                issue.Attachments.Add(item.ToString());

            if (_issueService.SubmitIssue(issue))
            {
                MessageBox.Show("Issue submitted successfully. Thank you for reporting.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLocation.Clear();
                cbCategory.SelectedIndex = -1;
                rtbDescription.Clear();
                lbAttachments.Items.Clear();
                UpdateEngagement();
            }
        }

        private void UpdateEngagement()
        {
            int score = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) score += 35;
            if (cbCategory.SelectedIndex >= 0) score += 25;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) score += 30;
            if (lbAttachments.Items.Count > 0) score += 10;

            score = Math.Min(score, 100);
            progressBar.Value = score;

            lblEngagement.Text = score switch
            {
                0 => "Start by entering a location",
                < 40 => "Good — keep going!",
                < 70 => "Nice! Almost there",
                < 100 => "Great — you're ready to submit",
                _ => "Excellent — thank you for your report!"
            };
        }
    }
}
