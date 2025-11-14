using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalIssueReporter.Utils
{
    public class Graph<T>
    {
        public class Edge { public int To; public double Weight; public Edge(int to, double w) { To = to; Weight = w; } }
        private readonly Dictionary<int, List<Edge>> _adj = new Dictionary<int, List<Edge>>();
        private readonly Dictionary<int, T> _nodes = new Dictionary<int, T>();

        public void AddNode(int id, T value) { _nodes[id] = value; if (!_adj.ContainsKey(id)) _adj[id] = new List<Edge>(); }
        public void AddEdge(int a, int b, double weight = 1, bool undirected = true)
        {
            if (!_adj.ContainsKey(a)) _adj[a] = new List<Edge>();
            if (!_adj.ContainsKey(b)) _adj[b] = new List<Edge>();
            _adj[a].Add(new Edge(b, weight));
            if (undirected) _adj[b].Add(new Edge(a, weight));
        }

        public IEnumerable<T> BFS(int start)
        {
            var visited = new HashSet<int>();
            var q = new Queue<int>();
            var outList = new List<T>();
            if (!_adj.ContainsKey(start)) return outList;
            q.Enqueue(start); visited.Add(start);
            while (q.Count > 0)
            {
                var u = q.Dequeue();
                outList.Add(_nodes[u]);
                foreach (var e in _adj[u])
                    if (!visited.Contains(e.To)) { visited.Add(e.To); q.Enqueue(e.To); }
            }
            return outList;
        }

        public IEnumerable<T> DFS(int start)
        {
            var visited = new HashSet<int>();
            var outList = new List<T>();
            if (!_adj.ContainsKey(start)) return outList;
            DFSUtil(start, visited, outList);
            return outList;
        }
        private void DFSUtil(int u, HashSet<int> visited, List<T> outList)
        {
            visited.Add(u);
            outList.Add(_nodes[u]);
            foreach (var e in _adj[u])
                if (!visited.Contains(e.To)) DFSUtil(e.To, visited, outList);
        }

        // Prim's MST returning list of edges (u,v,weight)
        public List<(int u, int v, double w)> PrimMST()
        {
            var res = new List<(int, int, double)>();
            if (_adj.Count == 0) return res;

            var inMST = new HashSet<int>();

            // Use the named tuple type in both places
            var pq = new SortedSet<(double w, int from, int to)>(
                Comparer<(double w, int from, int to)>.Create((a, b) =>
                {
                    int c = a.w.CompareTo(b.w);
                    if (c != 0) return c;

                    c = a.from.CompareTo(b.from);
                    if (c != 0) return c;

                    return a.to.CompareTo(b.to);
                })
            );

            int start = _adj.Keys.First();
            inMST.Add(start);

            foreach (var e in _adj[start])
                pq.Add((e.Weight, start, e.To));

            while (pq.Count > 0)
            {
                var min = pq.Min;
                pq.Remove(min);

                if (inMST.Contains(min.to))
                    continue;

                // Add MST edge
                res.Add((min.from, min.to, min.w));
                inMST.Add(min.to);

                foreach (var e in _adj[min.to])
                {
                    if (!inMST.Contains(e.To))
                        pq.Add((e.Weight, min.to, e.To));
                }
            }

            return res;
        }


        public Dictionary<int, List<Edge>> Adjacency => _adj;
        public Dictionary<int, T> Nodes => _nodes;
    }
}
