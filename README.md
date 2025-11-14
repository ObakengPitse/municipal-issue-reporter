# Municipal Services Application — Service Request Status Module

## 📘 Overview
The **Municipal Issue Reporter System** is a C# Windows Forms application designed to help community members:
- Report municipal issues (e.g., road damage, sanitation concerns).
- View **Local Events & Announcements**.
- Check **Service Request Status**.

It demonstrates the use of **data structures** such as stacks, queues, priority queues, hash tables, sorted dictionaries, and sets to efficiently manage event data and provide user recommendations.

---

## ⚙️ System Requirements
Before running the application, ensure that the following software is installed:

- **Windows 10 or later**
- **Visual Studio 2022 (Community, Professional, or Enterprise)**  
- **.NET Framework 6.0 or later** *(Windows Forms support required)*
- Optional: **Azure SQL Server** (for future database connectivity)

---

## Project Structure
```
MunicipalIssueReporter/
│
├── Forms/
│   ├── MainForm.cs
│   ├── ReportForm.cs
│   ├── LocalEventsForm.cs
│   └── ServiceStatusForm.cs
│
├── Models/
│   ├── Issue.cs
│   └── EventItem.cs
│
├── Services/
│   ├── EventService.cs
│   ├── EventRepository.cs
│   ├── IssueService.cs
│   ├── IssueRepository.cs
│   
│
├── Utils/
│   ├── Trees.cs
│   ├── MinHeap.cs
│   ├── Graph.cs
│   └── ErrorHandler.cs
│
├── issues.xml
├── Program.cs
└── README.md
```

## 🧱 Compilation Instructions

### Option 1: Using Visual Studio (Recommended)
1. Open **Visual Studio 2022**.
2. Click **File → Open → Project/Solution**.
3. Select the file **`MunicipalIssueReporter.csproj`**.
4. Wait for dependencies to load.
5. In the toolbar, set **Configuration** to `Debug` and **Platform** to `Any CPU`.
6. Click **Start (▶)** or press **F5** to compile and run the application.

### Option 2: Using .NET CLI
1. Open **Command Prompt** or **PowerShell** in the project root directory.
2. Run the following commands:
   ```bash
   dotnet build
   dotnet run
   ```
3. The application window will launch automatically.

---

## 🖥️ Running the Application

### On Launch:
You will be greeted with the **Main Menu**, which includes:
- **Report Issues** – Submit new municipal issues.  
- **Local Events & Announcements** – View and search upcoming community events.  
- **Service Request Status** – Track the progress of reported issues.

### Navigation:
- Use the main menu buttons to navigate between pages.
- Each form includes a **Back to Main Menu** button for easy return.

### Report Issues
Fill in fields, attach files, submit. Issues saved to `issues.xml`.

### Local Events & Announcements
- View upcoming events sorted by date and priority  
- Search events by **category** and **date range**  
- View detailed event descriptions by double-clicking  
- See **Recommended Events** based on viewed history  
- Uses **SortedDictionary**, **HashSet**, and **Stack** for efficient event storage

### Service Request Status
- Track by ID
- Show Priority Queue (**MinHeap**)
- Graph Traversal (**BFS**)
- Minimum Spanning Tree (**Prim**)
- Rebuild Data Structures

## Troubleshooting
- If MST unavailable → graph disconnected or empty.
- Delete issues.xml if load errors occur.

## Future Enhancements
- Visual graph rendering
- Dijkstra routing
- Red-Black Tree
- Unit tests


# Data Structures — Service Request Status
This part of the document explains **each implemented data structure** used in the *Service Request Status* feature, describing its **role**, **time/space complexity**, **concrete examples**, and **how it improves efficiency** for the application.

---

## 1. Binary Tree (BinaryNode<T>)
**Role**
- Fundamental node-based structure used as the building block for tree-based data structures (BST, AVL).
- Stores a value and references to left/right child nodes.

**Time / Space Complexity**
- Storage: **O(n)** nodes.
- Basic traversal (pre/in/post-order): **O(n)** time.

**Concrete example**
- A `BinaryNode<Issue>` stores an `Issue` object and links to left/right nodes. Used internally by BST/AVL implementations.

**Contribution to efficiency**
- Provides a simple, explicit representation of hierarchical relations and allows recursive tree algorithms (insert/traverse) to be implemented clearly and efficiently.

---

## 2. Binary Search Tree (BST) — `BinarySearchTree<T>`
**Role**
- Stores issues ordered by a key (e.g., `ReportedAt.Ticks`) to allow ordered traversal and efficient searches by the key.
- Supports in-order traversal to return issues in chronological order.

**Time / Space Complexity**
- Average case: Insert/Search/Delete — **O(log n)**.
- Worst case (unbalanced): **O(n)**.
- Traversal: **O(n)**.
- Space: **O(n)** for nodes.

**Concrete example**
- Insert issues as they are reported keyed by timestamp. An in-order traversal returns oldest → newest without sorting the whole dataset.

**Contribution to efficiency**
- Avoids repeated sorts when presenting chronological lists.
- If data arrives nearly-random, average performance is good. However, for worst-case inputs (sorted insertions) performance can degrade; that's why a balanced tree (AVL) is also provided.

---

## 3. AVL Tree (Self-balancing BST)
**Role**
- A balanced BST storing issues keyed by timestamp or another numeric key.
- Guarantees balanced heights by performing rotations on insert/delete.

**Time / Space Complexity**
- Insert/Search/Delete: **O(log n)** guaranteed.
- Traversal: **O(n)**.
- Space: **O(n)**.

**Concrete example**
- When many issues are submitted in time-order (e.g., bursts during an event), AVL maintains balance, ensuring searches and insertions remain logarithmic.

**Contribution to efficiency**
- Predictable performance ensures the UI remains responsive when building or querying the tree (e.g., range queries on timestamps).
- Particularly useful when the dataset size grows large — it prevents degeneration that would occur with a plain BST.

---

## 4. (Optional) Red‑Black Tree — explanation
**Role**
- Alternative self-balancing BST (not strictly required if AVL is present).
- Offers O(log n) operations with different balancing heuristics (fewer rotations on average).

**When to use**
- In large-scale systems or library code where insertion/delete frequency versus read frequency suggests Red-Black's trade-offs are beneficial.

**Contribution**
- Provides practically efficient balancing for dynamic workloads and matches many standard library map/set implementations.

---

## 5. Min-Heap / Priority Queue
**Role**
- Organises issues by `Priority` to quickly retrieve the highest-priority tasks for triage and assignment.
- Implemented as a binary heap backed by an array or `List<T>`.

**Time / Space Complexity**
- Insert: **O(log n)**
- Peek (top priority): **O(1)**
- Pop (remove top): **O(log n)**
- Build (from n items): **O(n)** for bottom-up heapify, or **O(n log n)** for repeated insertions.
- Space: **O(n)**.

**Concrete example**
- If the team needs the top 10 urgent requests, popping 10 times returns them in **O(k log n)** rather than sorting all requests (**O(n log n)**).

**Contribution to efficiency**
- Enables incremental/ongoing prioritisation: you can push new issues and immediately access the most urgent ones without re-sorting the entire list.
- Ideal for real-time dashboards and task assignment screens.

---

## 6. Graph (Adjacency List)
**Role**
- Models relationships between issues (nodes): same category, same location, temporal proximity, or any custom relation.
- Edges have weights that represent cost/distance/time (used by MST and traversal algorithms).

**Time / Space Complexity**
- Representation: **O(n + m)** where n = nodes, m = edges.
- Building by pairwise comparisons (naive): **O(n²)** time.
- BFS/DFS: **O(n + m)** time.
- Prim’s MST (with suitable priority queue): **O(m log n)**.

**Concrete example**
- Connect issues from the same suburb or the same road to form a cluster. Edge weight might be the time difference or geographic distance.
- A graph with 50 issues that cluster naturally into a few groups lets staff identify which ones can be handled in a single route.

**Contribution to efficiency**
- Allows grouping and cluster analysis: maintenance crews can handle connected issues in batches, reducing travel time and redundancy.
- Supports graph algorithms (BFS/DFS) to explore related items and MST to compute minimal connection routes.

**Implementation notes**
- For small to moderate `n`, pairwise building is acceptable. For larger datasets, use indexing (hash by category/location) or spatial structures to avoid O(n²).

---

## 7. Breadth-First Search (BFS)
**Role**
- Traversal algorithm that visits nodes in layers (closest first).
- Useful to find all issues reachable within k hops or to reveal clusters level-by-level.

**Time / Space Complexity**
- Time: **O(n + m)**
- Space: **O(n)** for the visited queue.

**Concrete example**
- Start BFS from a reported burst of streetlight faults to discover all other faults in that neighborhood (first layer = directly connected; next layer = neighbors of neighbors).

**Contribution to efficiency**
- Efficiently finds groups of related requests and helps in batch-assignment decisions. BFS is optimal for finding shortest unweighted paths in number of edges.

---

## 8. Depth-First Search (DFS)
**Role**
- Traversal algorithm that explores as far as possible along each branch before backtracking.
- Useful to identify connected components and deep dependency chains.

**Time / Space Complexity**
- Time: **O(n + m)**
- Space: **O(n)** for recursion/stack in worst case.

**Concrete example**
- Trace a chain of dependent fixes (e.g., a power outage report connected to multiple downstream issues) to uncover deep, related problems.

**Contribution to efficiency**
- Helps identify isolated components and provides a quick way to label connected components for component-wise processing (e.g., compute MST per component).

---

## 9. Prim’s Minimum Spanning Tree (MST)
**Role**
- Computes a set of edges connecting all nodes in a connected component with minimum total weight.
- Used to suggest minimal inspection/travel paths that connect related issues.

**Time / Space Complexity**
- With a binary heap: **O(m log n)**
- Space: **O(n + m)**

**Concrete example**
- Given several pothole reports in a district, Prim’s MST yields the minimal combined path (by chosen weight metric) connecting all report locations. This helps plan an inspector route with minimal travel.

**Contribution to efficiency**
- Reduces operational cost by finding minimal interconnections among related requests, ideal for scheduling field crews.
- When the graph is disconnected, compute MST per connected component to produce per-cluster minimal routes.

**Important note**
- MST requires a connected component. If the whole graph is disconnected, you can:
  - compute MST for each connected component, or
  - add fallback edges (less realistic) to force connectivity.

---

## 10. Putting them together — workflow & examples

**Typical workflow in the Service Status feature**
1. **Insert** new `Issue` → saved in repository.
2. **Rebuild data structures**:
   - BST/AVL keyed by timestamp for chronological queries.
   - MinHeap keyed by priority for urgent queues.
   - Graph built by relations (category/location/time) for clustering.
3. **UI actions**:
   - Show priority queue → pop top k from heap (`O(k log n)`).
   - Track by ID → search repository or tree (`O(log n)` if using tree).
   - Graph traversal → BFS/DFS to display related cluster (`O(n + m)`).
   - Compute MST → Prim’s algorithm per component (`O(m log n)`).

**Concrete combined example**
- A user submits 200 issues over a week.
- To show the current top 20 urgent items you use the MinHeap: **fast** retrieval without sorting all 200.
- To display a chronological activity log you do an AVL in-order walk (guaranteed `O(n)` traversal after `O(log n)` inserts).
- To plan field crew routes, you group issues by location using the Graph and compute MST per connected component to minimize travel.

---

## 11. Implementation tips & trade-offs

- **BST vs AVL**: Use AVL when inserts are often ordered or data grows large to avoid worst-case `O(n)` behaviors.
- **Graph building**: Avoid naive `O(n²)` pairwise checks for large datasets; instead, bucket by category/location or use spatial indices.
- **Heap building**: Use bottom-up heapify if constructing from array for `O(n)` build time when you already have all elements.
- **MST on disconnected graphs**: Compute per-component MSTs using BFS/DFS to find components first.

---
