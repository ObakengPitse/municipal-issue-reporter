using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Utils;

namespace MunicipalIssueReporter.Services
{
    public static class EventRepository
    {
        private static readonly string DataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "events.xml");
        // Store events keyed by start date for ordered traversal
        public static SortedDictionary<DateTime, List<EventItem>> EventsByDate { get; private set; } = new SortedDictionary<DateTime, List<EventItem>>();

        static EventRepository()
        {
            Load();
            if (EventsByDate.Count == 0)
                SeedSampleData();
        }

        public static void Add(EventItem ev)
        {
            try
            {
                if (!EventsByDate.ContainsKey(ev.StartDate.Date))
                    EventsByDate[ev.StartDate.Date] = new List<EventItem>();

                EventsByDate[ev.StartDate.Date].Add(ev);
                Save();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to add event.");
            }
        }

        public static IEnumerable<EventItem> GetAll()
        {
            foreach (var kv in EventsByDate)
                foreach (var ev in kv.Value)
                    yield return ev;
        }

        public static void Save()
        {
            try
            {
                // flatten to list for serialization
                var all = new List<EventItem>();
                foreach (var kv in EventsByDate)
                    all.AddRange(kv.Value);

                using (var stream = File.Create(DataFile))
                {
                    var ser = new XmlSerializer(typeof(List<EventItem>));
                    ser.Serialize(stream, all);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to save events.");
            }
        }

        public static void Load()
        {
            try
            {
                if (!File.Exists(DataFile))
                {
                    EventsByDate = new SortedDictionary<DateTime, List<EventItem>>();
                    return;
                }

                using (var stream = File.OpenRead(DataFile))
                {
                    var ser = new XmlSerializer(typeof(List<EventItem>));
                    var list = (List<EventItem>)ser.Deserialize(stream) ?? new List<EventItem>();
                    EventsByDate = new SortedDictionary<DateTime, List<EventItem>>();
                    foreach (var ev in list)
                    {
                        var key = ev.StartDate.Date;
                        if (!EventsByDate.ContainsKey(key)) EventsByDate[key] = new List<EventItem>();
                        EventsByDate[key].Add(ev);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to load events. Starting with empty list.");
                EventsByDate = new SortedDictionary<DateTime, List<EventItem>>();
            }
        }

        private static void SeedSampleData()
        {
            Add(new EventItem
            {
                Title = "Community Clean-Up",
                Category = "Sanitation",
                Description = "Join neighbours to clean the park and public areas. Gloves and bags will be provided.",
                StartDate = DateTime.Today.AddDays(3).AddHours(9),
                EndDate = DateTime.Today.AddDays(3).AddHours(12),
                Priority = 5,
                Location = "Central Park"
            });

            Add(new EventItem
            {
                Title = "Roadworks Information Session",
                Category = "Roads",
                Description = "Public meeting to discuss road closures, detours, and upcoming infrastructure upgrades.",
                StartDate = DateTime.Today.AddDays(7).AddHours(18),
                EndDate = DateTime.Today.AddDays(7).AddHours(20),
                Priority = 7,
                Location = "Town Hall"
            });

            Add(new EventItem
            {
                Title = "Parks Volunteer Day",
                Category = "Parks",
                Description = "Planting trees, cleaning riversides, and general park maintenance.",
                StartDate = DateTime.Today.AddDays(14).AddHours(8),
                EndDate = DateTime.Today.AddDays(14).AddHours(14),
                Priority = 4,
                Location = "Riverside Park"
            });

            Add(new EventItem
            {
                Title = "Community Health Fair",
                Category = "Health",
                Description = "Free blood pressure, diabetes, and wellness screenings with local clinics.",
                StartDate = DateTime.Today.AddDays(10).AddHours(9),
                EndDate = DateTime.Today.AddDays(10).AddHours(15),
                Priority = 8,
                Location = "Community Centre"
            });

            Add(new EventItem
            {
                Title = "Neighbourhood Watch Meeting",
                Category = "Safety",
                Description = "Discussion on safety tips, local crime updates, and security patrol scheduling.",
                StartDate = DateTime.Today.AddDays(5).AddHours(17),
                EndDate = DateTime.Today.AddDays(5).AddHours(19),
                Priority = 6,
                Location = "Police Station Hall"
            });

            Add(new EventItem
            {
                Title = "Local Farmers Market",
                Category = "Agriculture",
                Description = "Fresh produce, organic foods, and homemade goods from local farmers.",
                StartDate = DateTime.Today.AddDays(2).AddHours(8),
                EndDate = DateTime.Today.AddDays(2).AddHours(13),
                Priority = 5,
                Location = "Market Square"
            });

            Add(new EventItem
            {
                Title = "Coding for Beginners Workshop",
                Category = "Education",
                Description = "Free workshop teaching basic programming skills. Bring your own laptop.",
                StartDate = DateTime.Today.AddDays(12).AddHours(10),
                EndDate = DateTime.Today.AddDays(12).AddHours(15),
                Priority = 7,
                Location = "Library Auditorium"
            });

            Add(new EventItem
            {
                Title = "Fire Safety Awareness Day",
                Category = "Safety",
                Description = "Fire department demonstrations, evacuation drills, and free smoke alarm checks.",
                StartDate = DateTime.Today.AddDays(20).AddHours(9),
                EndDate = DateTime.Today.AddDays(20).AddHours(13),
                Priority = 6,
                Location = "Main Fire Station"
            });

            Add(new EventItem
            {
                Title = "Youth Sports Tournament",
                Category = "Sports",
                Description = "Local schools compete in football, basketball, and athletics. Open to all spectators.",
                StartDate = DateTime.Today.AddDays(9).AddHours(8),
                EndDate = DateTime.Today.AddDays(9).AddHours(17),
                Priority = 5,
                Location = "City Stadium"
            });

            Add(new EventItem
            {
                Title = "Public Transport Feedback Forum",
                Category = "Transport",
                Description = "City transport officials gather feedback to improve bus and taxi services.",
                StartDate = DateTime.Today.AddDays(15).AddHours(17),
                EndDate = DateTime.Today.AddDays(15).AddHours(19),
                Priority = 6,
                Location = "Civic Offices"
            });

            Add(new EventItem
            {
                Title = "Blood Donation Drive",
                Category = "Health",
                Description = "Donate blood to save lives. Walk-ins are welcome, ID required.",
                StartDate = DateTime.Today.AddDays(6).AddHours(9),
                EndDate = DateTime.Today.AddDays(6).AddHours(14),
                Priority = 8,
                Location = "Community Health Centre"
            });

            Add(new EventItem
            {
                Title = "Cultural Heritage Festival",
                Category = "Community",
                Description = "A celebration of local traditions, food, music, and art. All are welcome!",
                StartDate = DateTime.Today.AddDays(18).AddHours(10),
                EndDate = DateTime.Today.AddDays(18).AddHours(20),
                Priority = 9,
                Location = "Heritage Grounds"
            });
        }

    }
}
