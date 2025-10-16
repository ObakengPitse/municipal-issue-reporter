using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Utils;

namespace MunicipalIssueReporter.Services
{
    public static class IssueRepository
    {
        private static readonly string DataFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "issues.xml");
        public static List<Issue> Issues { get; private set; } = new List<Issue>();

        static IssueRepository()
        {
            Load();
        }

        public static void Add(Issue issue)
        {
            Issues.Add(issue);
            Save();
        }

        private static void Save()
        {
            try
            {
                using (var stream = File.Create(DataFile))
                {
                    var ser = new XmlSerializer(typeof(List<Issue>));
                    ser.Serialize(stream, Issues);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to save issues to the local file.");
            }
        }

        private static void Load()
        {
            try
            {
                if (!File.Exists(DataFile))
                {
                    Issues = new List<Issue>();
                    return;
                }

                using (var stream = File.OpenRead(DataFile))
                {
                    var ser = new XmlSerializer(typeof(List<Issue>));
                    Issues = (List<Issue>)ser.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to load issues. Starting with an empty list.");
                Issues = new List<Issue>();
            }
        }
    }
}
