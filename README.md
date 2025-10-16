# 🏙️ Municipal Issue Reporter System

## 📘 Overview
The **Municipal Issue Reporter System** is a C# Windows Forms application designed to help community members:
- Report municipal issues (e.g., road damage, sanitation concerns).
- View **Local Events & Announcements**.
- Check **Service Request Status** (Coming soon).

It demonstrates the use of **data structures** such as stacks, queues, priority queues, hash tables, sorted dictionaries, and sets to efficiently manage event data and provide user recommendations.

---

## ⚙️ System Requirements
Before running the application, ensure that the following software is installed:

- **Windows 10 or later**
- **Visual Studio 2022 (Community, Professional, or Enterprise)**  
- **.NET Framework 6.0 or later** *(Windows Forms support required)*
- Optional: **Azure SQL Server** (for future database connectivity)

---

## 🧩 Project Structure
```
MunicipalIssueReporter/
│
├── Forms/
│   ├── MainForm.cs
│   ├── ReportIssueForm.cs
│   └── LocalEventsForm.cs
│
├── Models/
│   └── EventItem.cs
│
├── Services/
│   └── EventService.cs
│
├── Utils/
│   └── ErrorHandler.cs
│
├── Program.cs
├── MunicipalIssueReporter.csproj
└── README.md
```

---

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
- **Service Request Status** – Track the progress of reported issues(Coming Soon!).

### Navigation:
- Use the main menu buttons to navigate between pages.
- Each form includes a **Back to Main Menu** button for easy return.

---

### 📅 Report Issues Page
This section allows users to capture and submit issues.

### Features:
✅ Report issues 
✅ Attach images and documents  
✅ Dynamic progress bar 


## 📅 Local Events & Announcements 
This section allows users to view, search, and explore community events.

### Features:
✅ View upcoming events sorted by date and priority  
✅ Search events by **category** and **date range**  
✅ View detailed event descriptions by double-clicking  
✅ See **Recommended Events** based on viewed history  
✅ Uses **SortedDictionary**, **HashSet**, and **Stack** for efficient event storage

### How It Works:
1. **Event Data** is preloaded through `EventRepository.SeedSampleData()`.
2. Events are stored and sorted using a **SortedDictionary<DateTime, EventItem>**.
3. Each viewed event is recorded in a **Stack** for tracking user activity.
4. Recommendations are generated based on past search and viewing patterns.

---

## 🔧 Technical Implementation Details

| Component | Description |
|------------|--------------|
| **Stacks / Queues** | Manage recently viewed events and upcoming event processing |
| **SortedDictionary** | Store and retrieve events sorted by date |
| **HashSet** | Store unique event categories |
| **Recommendation Engine** | Suggests similar events based on category and date proximity |
| **ErrorHandler Utility** | Gracefully handles user and system exceptions |

---

## 🧠 How to Use (Report Issues)

1. **Open the Application.**
2. From the **Main Menu**, select **Report Issues**.
3. **Capture** the location of the issue in the textbox.
4. **Category Selection:** Choose the issue category from the dropdown (e.g., Sanitation, Roads, Utilities).
5. **Description Box:** Provide detailed information about the issue in the RichTextBox.
6. **Media Attachment:** Click the button to attach images or documents via OpenFileDialog.
7. **Submit Button:** Click to submit the issue.
8. **Engagement Feature:** Observe the dynamic feedback using a ProgressBar or encouraging messages.
9. **Navigation Buttons:** Use the "Back to Main Menu" button to return to the main menu.

## 🧠 Local Events & Announcements

1. From the **Main Menu**, select **Local Events & Announcements**.
2. Use the **Category dropdown** and **Date pickers** to filter results.
3. Click **Search** to refresh the event list.
4. **Double-click** an event to see details and mark it as viewed.
5. View suggested items in the **Recommendations** panel.
6. Use **Back to Main Menu** to return and explore other options.

---

## 🧰 Troubleshooting

| Issue | Possible Cause | Solution |
|--------|----------------|-----------|
| App won’t start | Missing .NET Framework | Install .NET 6 SDK or higher |
| Build fails | Corrupt dependencies | Rebuild project or run `dotnet restore` |
| No events showing | Seed data not loaded | Check `SeedSampleData()` in `EventRepository.cs` |
| UI not scaling | Display scaling too high | Set display scale to 100–125% in Windows settings |

---

## 👩🏽‍💻 Developer Notes
- Built with **C# (.NET 6)** and **Windows Forms**.
- Designed for **educational purposes** to demonstrate software architecture and data structures.
- Easily extendable for **database integration (Azure SQL)** or **API communication** in future versions.

---

## 📄 License
This project is released for **academic and demonstration purposes only.**  
All rights reserved © 2025 Municipal Issue Reporter Team.
