using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Services;
using MunicipalIssueReporter.Utils;

namespace MunicipalIssueReporter.Forms
{
    public class LocalEventsForm : Form
    {
        private EventService _eventService;

        private ComboBox cbCategoryFilter;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Button btnSearch;
        private DataGridView dgvEvents;
        private ListBox lbRecommendations;
        private Label lblCount;

        public LocalEventsForm()
        {
            _eventService = new EventService();
            InitializeComponents();
            LoadInitial();
        }

        private void InitializeComponents()
        {
            Text = "📅 Local Events & Announcements";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1000;
            Height = 700;

            // Main container
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(15),
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // Left = main grid
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Right = recommendations
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Search filters
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 85F)); // Event list
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Footer
            Controls.Add(mainLayout);

            // =========================
            // 🔍 Search Section (top row)
            // =========================
            var searchGroup = new GroupBox
            {
                Text = "Search Filters",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };

            var searchLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(10)
            };

            searchLayout.Controls.Add(new Label { Text = "Category:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
            cbCategoryFilter = new ComboBox { Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            searchLayout.Controls.Add(cbCategoryFilter);

            searchLayout.Controls.Add(new Label { Text = "From:", AutoSize = true, Margin = new Padding(12, 8, 3, 3) });
            dtFrom = new DateTimePicker { Width = 150, Format = DateTimePickerFormat.Short };
            searchLayout.Controls.Add(dtFrom);

            searchLayout.Controls.Add(new Label { Text = "To:", AutoSize = true, Margin = new Padding(12, 8, 3, 3) });
            dtTo = new DateTimePicker { Width = 150, Format = DateTimePickerFormat.Short };
            searchLayout.Controls.Add(dtTo);

            btnSearch = new Button
            {
                Text = "Search",
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold)
            };
            btnSearch.Click += BtnSearch_Click;
            searchLayout.Controls.Add(btnSearch);

            searchGroup.Controls.Add(searchLayout);
            mainLayout.Controls.Add(searchGroup, 0, 0);
            mainLayout.SetColumnSpan(searchGroup, 2);

            // =========================
            // 📋 Events List (left side)
            // =========================
            var eventsGroup = new GroupBox
            {
                Text = "Upcoming Events and Announcements",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };

            dgvEvents = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = System.Drawing.Color.WhiteSmoke
            };
            dgvEvents.CellDoubleClick += DgvEvents_CellDoubleClick;

            eventsGroup.Controls.Add(dgvEvents);
            mainLayout.Controls.Add(eventsGroup, 0, 1);

            // =========================
            // 💡 Recommendations (right)
            // =========================
            var recGroup = new GroupBox
            {
                Text = "Recommended for You",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold)
            };

            var recLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                Padding = new Padding(5)
            };
            recLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            recLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            recLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            lblCount = new Label
            {
                Text = "0 events found",
                Dock = DockStyle.Top,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic),
                Padding = new Padding(5)
            };
            recLayout.Controls.Add(lblCount, 0, 0);

            lbRecommendations = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
            lbRecommendations.DoubleClick += LbRecommendations_DoubleClick;
            recLayout.Controls.Add(lbRecommendations, 0, 1);

            var btnView = new Button { Text = "View Selected", Dock = DockStyle.Top };
            btnView.Click += BtnView_Click;
            var btnBack = new Button { Text = "← Back to Main Menu", Dock = DockStyle.Top };
            btnBack.Click += (s, e) => { new MainForm().Show(); Close(); };

            var actionsPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            actionsPanel.Controls.Add(btnView);
            actionsPanel.Controls.Add(btnBack);
            recLayout.Controls.Add(actionsPanel, 0, 2);

            recGroup.Controls.Add(recLayout);
            mainLayout.Controls.Add(recGroup, 1, 1);

            // =========================
            // ℹ️ Footer
            // =========================
            var footer = new Label
            {
                Text = "💡 Tip: Double-click an event to view its details and mark it as viewed.",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            mainLayout.Controls.Add(footer, 0, 2);
            mainLayout.SetColumnSpan(footer, 2);
        }

        private void LoadInitial()
        {
            try
            {
                cbCategoryFilter.Items.Clear();
                cbCategoryFilter.Items.Add("All");
                foreach (var c in _eventService.Categories.OrderBy(c => c))
                    cbCategoryFilter.Items.Add(c);
                cbCategoryFilter.SelectedIndex = 0;

                RefreshEventGrid(_eventService.Search(null, DateTime.Now.AddYears(-1), DateTime.Now.AddYears(2)));
                RefreshRecommendations();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to load events.");
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string category = cbCategoryFilter.SelectedItem?.ToString();
                if (string.Equals(category, "All", StringComparison.OrdinalIgnoreCase)) category = null;

                var from = dtFrom.Value.Date;
                var to = dtTo.Value.Date.AddDays(1).AddTicks(-1);

                var results = _eventService.Search(category, from, to);
                RefreshEventGrid(results);
                RefreshRecommendations();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Search failed.");
            }
        }

        private void RefreshEventGrid(IEnumerable<EventItem> items)
        {
            var list = items.OrderBy(e => e.StartDate).ToList();
            dgvEvents.DataSource = list.Select(e => new
            {
                e.Id,
                e.Title,
                e.Category,
                Start = e.StartDate.ToString("dd MMM yyyy"),
                End = e.EndDate.ToString("dd MMM yyyy"),
                e.Location,
                e.Priority
            }).ToList();

            lblCount.Text = $"{list.Count} event(s) found";
        }

        private void RefreshRecommendations()
        {
            try
            {
                lbRecommendations.Items.Clear();
                var recs = _eventService.GetRecommendations(6);
                foreach (var r in recs)
                    lbRecommendations.Items.Add($"{r.Title} — {r.StartDate:d} ({r.Category})");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to refresh recommendations.");
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            ViewSelectedEvent();
        }

        private void ViewSelectedEvent()
        {
            try
            {
                if (dgvEvents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Select an event first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var idObj = dgvEvents.SelectedRows[0].Cells["Id"].Value;
                if (idObj == null) return;

                var id = Guid.Parse(idObj.ToString());
                var ev = EventRepository.GetAll().FirstOrDefault(x => x.Id == id);
                if (ev == null) return;

                _eventService.MarkViewed(ev); // stack usage

                MessageBox.Show(
                    $"{ev.Title}\n\nCategory: {ev.Category}\nLocation: {ev.Location}\nStart: {ev.StartDate}\nEnd: {ev.EndDate}\n\n{ev.Description}",
                    "Event Details",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                RefreshRecommendations();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to view event.");
            }
        }

        private void DgvEvents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ViewSelectedEvent();
        }

        private void LbRecommendations_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (lbRecommendations.SelectedItem == null) return;
                var txt = lbRecommendations.SelectedItem.ToString();
                var title = txt.Split('—')[0].Trim();

                var ev = EventRepository.GetAll().FirstOrDefault(x =>
                    x.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

                if (ev != null)
                {
                    _eventService.MarkViewed(ev);
                    MessageBox.Show(
                        $"{ev.Title}\n\nCategory: {ev.Category}\nLocation: {ev.Location}\nStart: {ev.StartDate}\nEnd: {ev.EndDate}\n\n{ev.Description}",
                        "Event Details",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    RefreshRecommendations();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to open recommendation.");
            }
        }
    }
}
