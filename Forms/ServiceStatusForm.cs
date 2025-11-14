using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Services;
using MunicipalIssueReporter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalIssueReporter.Forms
{
    public class ServiceStatusForm : Form
    {
        private DataGridView dgvRequests;
        private TextBox txtSearchId;
        private Button btnSearch;
        private Button btnBack;
        private Button btnRebuildTrees;
        private Button btnShowPriority;
        private Button btnGraphTraversal;
        private Button btnComputeMST;

        // Data structures
        private BinarySearchTree<Issue> _bstByTimestamp;
        private AvlTree<Issue> _avlByTimestamp;
        private MinHeap<Issue> _minHeapByPriority;
        private Graph<Issue> _graph;

        public ServiceStatusForm()
        {
            InitializeComponents();
            BuildDataStructures(); // initial build from IssueRepository
            LoadGrid();
        }

        private void InitializeComponents()
        {
            Text = "Service Request Status";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Padding = new Padding(8) };
            for (int i = 0; i < 6; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, i == 0 ? 8f : 18f));

            // Search panel
            var searchPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            txtSearchId = new TextBox { Width = 300, PlaceholderText = "Enter tracking id (e.g. ABC123) or GUID fragment..." };
            btnSearch = new Button { Text = "Track by ID" };
            btnSearch.Click += BtnSearch_Click;
            btnRebuildTrees = new Button { Text = "Rebuild data structures" };
            btnRebuildTrees.Click += (s, e) => { BuildDataStructures(); LoadGrid(); };
            searchPanel.Controls.Add(txtSearchId);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnRebuildTrees);

            dgvRequests = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, AllowUserToAddRows = false };
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tracking ID", DataPropertyName = "TrackingId", Width = 120 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Category", DataPropertyName = "Category", Width = 120 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Location", DataPropertyName = "Location", Width = 150 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Priority", DataPropertyName = "Priority", Width = 70 });
            dgvRequests.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reported At", DataPropertyName = "ReportedAt", Width = 160 });

            // Bottom controls
            var bottomPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            btnShowPriority = new Button { Text = "Show Priority Queue (MinHeap)" };
            btnShowPriority.Click += BtnShowPriority_Click;
            btnGraphTraversal = new Button { Text = "Show Graph Traversal (BFS)" };
            btnGraphTraversal.Click += BtnGraphTraversal_Click;
            btnComputeMST = new Button { Text = "Compute MST (Prim)" };
            btnComputeMST.Click += BtnComputeMST_Click;
            btnBack = new Button { Text = "Back to Main Menu" };
            btnBack.Click += (s, e) => { new MainForm().Show(); Close(); };

            bottomPanel.Controls.Add(btnShowPriority);
            bottomPanel.Controls.Add(btnGraphTraversal);
            bottomPanel.Controls.Add(btnComputeMST);
            bottomPanel.Controls.Add(btnBack);

            layout.Controls.Add(searchPanel, 0, 0);
            layout.Controls.Add(dgvRequests, 0, 1);
            layout.Controls.Add(bottomPanel, 0, 5);

            Controls.Add(layout);
        }

        private void BuildDataStructures()
        {
            // Key by ReportedAt.Ticks for BST/AVL to demonstrate tree behaviour
            _bstByTimestamp = new BinarySearchTree<Issue>(i => i.ReportedAt.Ticks);
            _avlByTimestamp = new AvlTree<Issue>();
            _minHeapByPriority = new MinHeap<Issue>(i => -i.Priority); // min-heap of -priority -> highest first
            _graph = new Graph<Issue>();

            var issues = Services.IssueRepository.Issues;
            // Build nodes with integer ids (index)
            for (int i = 0; i < issues.Count; i++)
            {
                var iss = issues[i];
                _bstByTimestamp.Insert(iss);
                _avlByTimestamp.Insert(iss, iss.ReportedAt.Ticks);
                _minHeapByPriority.Add(iss);
                _graph.AddNode(i, iss);
            }

            // Build simple edges: connect issues in same category or same location, weight by time difference (minutes)
            for (int i = 0; i < issues.Count; i++)
            {
                for (int j = i + 1; j < issues.Count; j++)
                {
                    double w = Math.Abs((issues[i].ReportedAt - issues[j].ReportedAt).TotalMinutes) + 1;

                    // Strong connection: Same category or location
                    if (issues[i].Category == issues[j].Category || issues[i].Location == issues[j].Location)
                    {
                        _graph.AddEdge(i, j, Math.Max(1, w / 10.0), undirected: true);
                    }
                    else
                    {
                        // Weak fallback connection (within same day)
                        if (w < 1440)
                            _graph.AddEdge(i, j, w / 2.0, undirected: true);
                    }
                    // GUARANTEE GRAPH CONNECTIVITY FOR MST
                    // Adds a generic fallback edge so MST always possible

                    _graph.AddEdge(i, j, w, undirected: true);
                }
            }

        }

        private void LoadGrid(IEnumerable<Issue> list = null)
        {
            var toBind = (list ?? Services.IssueRepository.Issues).Select(i => new
            {
                i.TrackingId,
                i.Category,
                i.Location,
                i.Status,
                i.Priority,
                ReportedAt = i.ReportedAt.ToString("g")
            }).ToList();

            dgvRequests.DataSource = toBind;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var q = txtSearchId.Text.Trim();
            if (string.IsNullOrEmpty(q))
            {
                LoadGrid();
                return;
            }

            // Try find by tracking fragment or full guid
            var found = Services.IssueRepository.Issues
                .Where(i => i.TrackingId.Equals(q, StringComparison.OrdinalIgnoreCase)
                         || i.Id.ToString().IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            if (found.Count == 0)
            {
                MessageBox.Show("No service request found with that ID.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Show first found in detail
            var display = found.Select(i => new
            {
                i.TrackingId,
                i.Category,
                i.Location,
                i.Status,
                i.Priority,
                ReportedAt = i.ReportedAt.ToString("g"),
                Description = i.Description
            }).ToList();

            dgvRequests.DataSource = display;
        }

        private void BtnShowPriority_Click(object sender, EventArgs e)
        {
            var sorted = _minHeapByPriority.ToSortedList().Reverse().ToList(); // reverse to show highest first
            var display = sorted.Select(i => new
            {
                i.TrackingId,
                i.Priority,
                i.Status,
                i.Category,
                ReportedAt = i.ReportedAt.ToString("g")
            }).ToList();
            dgvRequests.DataSource = display;
        }

        private void BtnGraphTraversal_Click(object sender, EventArgs e)
        {
            if (Services.IssueRepository.Issues.Count == 0)
            {
                MessageBox.Show("No requests to traverse.", "Empty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // choose the first node as start and BFS
            var traversal = _graph.BFS(_graph.Nodes.Keys.First()).Select(i => new
            {
                ((Issue)(object)i).TrackingId,
                ((Issue)(object)i).Category,
                ((Issue)(object)i).Location,
                ((Issue)(object)i).Status
            }).ToList();

            // Note: Graph<T>.BFS returns T, but above we stored Issue as T, so it's direct
            dgvRequests.DataSource = traversal;
            MessageBox.Show("Displayed BFS traversal starting from the first request node.", "Graph BFS", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnComputeMST_Click(object sender, EventArgs e)
        {
            var edges = _graph.PrimMST();
            if (edges.Count == 0)
            {
                MessageBox.Show("No MST available (graph may be disconnected or empty).", "MST", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var display = edges.Select(e =>
            new
            {
                From = _graph.Nodes[e.u].TrackingId,
                To = _graph.Nodes[e.v].TrackingId,
                Weight = Math.Round(e.w, 2)
            }).ToList();

            dgvRequests.DataSource = display;
            MessageBox.Show("Minimum Spanning Tree edges displayed (Prim's algorithm).", "MST", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
