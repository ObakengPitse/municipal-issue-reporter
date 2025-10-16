using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalIssueReporter.Models;
using MunicipalIssueReporter.Utils;

namespace MunicipalIssueReporter.Services
{
    public class EventService
    {
        // track recent searches for recommendation (Queue acts as time-ordered log)
        private readonly Queue<string> _recentSearches = new Queue<string>(capacity: 10);

        // track recently viewed events for "undo" or history (Stack)
        private readonly Stack<Guid> _recentlyViewed = new Stack<Guid>();

        // set of categories (HashSet)
        public HashSet<string> Categories { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public EventService()
        {
            LoadCategoriesFromRepo();
        }

        private void LoadCategoriesFromRepo()
        {
            foreach (var ev in EventRepository.GetAll())
                Categories.Add(ev.Category);
        }

        // Search by optional category and date range
        public List<EventItem> Search(string category = null, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                from ??= DateTime.MinValue;
                to ??= DateTime.MaxValue;

                var results = new List<EventItem>();
                foreach (var ev in EventRepository.GetAll())
                {
                    if (!string.IsNullOrWhiteSpace(category) && !ev.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (ev.StartDate < from || ev.StartDate > to)
                        continue;
                    results.Add(ev);
                }

                // log search for recommendations
                if (!string.IsNullOrWhiteSpace(category))
                {
                    EnqueueSearch(category);
                }

                // sort results by date then by priority descending
                return results.OrderBy(e => e.StartDate).ThenByDescending(e => e.Priority).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Search failed.");
                return new List<EventItem>();
            }
        }

        private void EnqueueSearch(string category)
        {
            try
            {
                if (_recentSearches.Count == 10) _recentSearches.Dequeue();
                _recentSearches.Enqueue(category);
            }
            catch { /* swallow; not critical */ }
        }

        // Call when a user views an event so we can use in recommendation and "recent" features
        public void MarkViewed(EventItem ev)
        {
            try
            {
                if (ev == null) return;
                _recentlyViewed.Push(ev.Id);
            }
            catch { }
        }

        // Get recent viewed IDs as list
        public List<Guid> GetRecentlyViewed() => _recentlyViewed.ToList();

        // Recommendation engine:
        // - compute frequency of categories in recent searches
        // - build PriorityQueue of candidate events (by priority)
        // - return top N events not already viewed in recent history
        public List<EventItem> GetRecommendations(int top = 5)
        {
            try
            {
                // frequency counting using a dictionary (hash table)
                var freq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var cat in _recentSearches)
                {
                    if (!freq.ContainsKey(cat)) freq[cat] = 0;
                    freq[cat]++;
                }

                // If no searches yet, fall back to upcoming high-priority events
                IEnumerable<EventItem> candidates;
                if (freq.Count == 0)
                {
                    candidates = EventRepository.GetAll().Where(e => e.StartDate >= DateTime.Now);
                }
                else
                {
                    // select events that match the top categories
                    var topCats = freq.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).Take(3).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    candidates = EventRepository.GetAll().Where(e => topCats.Contains(e.Category) || e.StartDate >= DateTime.Now);
                }

                // Use PriorityQueue to rank by priority then by soonest date
#if NET6_0_OR_GREATER
                var pq = new PriorityQueue<EventItem, (int, DateTime)>(Comparer<(int, DateTime)>.Create((a, b) =>
                {
                    // Higher priority should come first => invert
                    var pr = b.Item1.CompareTo(a.Item1);
                    if (pr != 0) return pr;
                    return a.Item2.CompareTo(b.Item2); // earlier date first
                }));
                foreach (var ev in candidates)
                {
                    // priority key: (Priority, StartDate) but PriorityQueue only supports one priority; we use tuple
                    pq.Enqueue(ev, (ev.Priority, ev.StartDate));
                }

                var recommendations = new List<EventItem>();
                var viewed = new HashSet<Guid>(_recentlyViewed);
                while (pq.Count > 0 && recommendations.Count < top)
                {
                    var ev = pq.Dequeue();
                    if (viewed.Contains(ev.Id)) continue;
                    recommendations.Add(ev);
                }
                return recommendations;
#else
                // Fallback when PriorityQueue isn't present: order using LINQ
                return candidates
                    .Where(e => !_recentlyViewed.Contains(e.Id))
                    .OrderByDescending(e => e.Priority)
                    .ThenBy(e => e.StartDate)
                    .Take(top)
                    .ToList();
#endif
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to build recommendations.");
                return new List<EventItem>();
            }
        }
    }
}
