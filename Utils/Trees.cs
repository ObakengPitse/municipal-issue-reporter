using System;
using System.Collections.Generic;

namespace MunicipalIssueReporter.Utils
{
    // Simple binary tree node
    public class BinaryNode<T>
    {
        public T Value;
        public BinaryNode<T> Left;
        public BinaryNode<T> Right;
        public BinaryNode(T value) { Value = value; }
    }

    // Binary Search Tree (keyed by long key extractor)
    public class BinarySearchTree<T>
    {
        private BinaryNode<T> _root;
        private readonly Func<T, long> _keySelector;

        public BinarySearchTree(Func<T, long> keySelector) => _keySelector = keySelector;

        public void Insert(T value)
        {
            _root = Insert(_root, value);
        }

        private BinaryNode<T> Insert(BinaryNode<T> node, T value)
        {
            if (node == null) return new BinaryNode<T>(value);
            var k = _keySelector(value);
            var nk = _keySelector(node.Value);
            if (k < nk) node.Left = Insert(node.Left, value);
            else node.Right = Insert(node.Right, value);
            return node;
        }

        public IEnumerable<T> InOrder()
        {
            var list = new List<T>();
            InOrder(_root, list);
            return list;
        }

        private void InOrder(BinaryNode<T> node, List<T> list)
        {
            if (node == null) return;
            InOrder(node.Left, list);
            list.Add(node.Value);
            InOrder(node.Right, list);
        }

        // Simple search by key
        public T Find(long key)
        {
            var node = _root;
            while (node != null)
            {
                var nk = _keySelector(node.Value);
                if (key == nk) return node.Value;
                node = key < nk ? node.Left : node.Right;
            }
            return default;
        }
    }

    // AVL tree (balanced BST) — storing T with long key selector
    public class AvlTree<T>
    {
        private class Node
        {
            public T Value;
            public long Key;
            public Node Left, Right;
            public int Height;
            public Node(T val, long key) { Value = val; Key = key; Height = 1; }
        }

        private Node _root;

        private int Height(Node n) => n?.Height ?? 0;
        private int BalanceFactor(Node n) => n == null ? 0 : Height(n.Left) - Height(n.Right);
        private void FixHeight(Node n) => n.Height = Math.Max(Height(n.Left), Height(n.Right)) + 1;

        private Node RotateRight(Node y)
        {
            var x = y.Left;
            var T2 = x.Right;
            x.Right = y;
            y.Left = T2;
            FixHeight(y);
            FixHeight(x);
            return x;
        }

        private Node RotateLeft(Node x)
        {
            var y = x.Right;
            var T2 = y.Left;
            y.Left = x;
            x.Right = T2;
            FixHeight(x);
            FixHeight(y);
            return y;
        }

        public void Insert(T value, long key) => _root = Insert(_root, value, key);

        private Node Insert(Node node, T val, long key)
        {
            if (node == null) return new Node(val, key);
            if (key < node.Key) node.Left = Insert(node.Left, val, key);
            else node.Right = Insert(node.Right, val, key);

            FixHeight(node);
            var balance = BalanceFactor(node);

            if (balance > 1 && key < node.Left.Key) return RotateRight(node);
            if (balance < -1 && key > node.Right.Key) return RotateLeft(node);
            if (balance > 1 && key > node.Left.Key)
            {
                node.Left = RotateLeft(node.Left);
                return RotateRight(node);
            }
            if (balance < -1 && key < node.Right.Key)
            {
                node.Right = RotateRight(node.Right);
                return RotateLeft(node);
            }
            return node;
        }

        public IEnumerable<T> InOrder()
        {
            var list = new List<T>();
            InOrder(_root, list);
            return list;
        }
        private void InOrder(Node n, List<T> outList)
        {
            if (n == null) return;
            InOrder(n.Left, outList);
            outList.Add(n.Value);
            InOrder(n.Right, outList);
        }
    }
}
