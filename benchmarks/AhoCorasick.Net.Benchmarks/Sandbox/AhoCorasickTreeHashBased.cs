using System;
using System.Collections.Generic;
using System.Linq;

namespace AhoCorasick.Net.Benchmarks.Sandbox
{
    public class AhoCorasickTreeHashBased
    {
        private readonly AhoCorasickTreeNode _rootNode;

        public AhoCorasickTreeHashBased(string[] keywords)
        {
            if (keywords == null) throw new ArgumentNullException("keywords");
            if (keywords.Length == 0) throw new ArgumentException("should contain keywords");

            _rootNode = new AhoCorasickTreeNode();

            var length = keywords.Length;
            for (var i = 0; i < length; i++)
            {
                AddPatternToTree(keywords[i]);
            }

            SetFailures();
        }

        public bool Contains(string text)
        {
            var currentNode = _rootNode;

            var length = text.Length;
            for (var i = 0; i < length; i++)
            {
                while (true)
                {
                    var node = currentNode.GetNode(text[i]);
                    if (node == null)
                    {
                        currentNode = currentNode.Failure;
                        if (currentNode == _rootNode)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (node.IsFinished)
                        {
                            return true;
                        }

                        currentNode = node;
                        break;
                    }
                }
            }

            return false;
        }


        private void AddPatternToTree(string pattern)
        {
            var latestNode = _rootNode;
            var length = pattern.Length;
            for (var i = 0; i < length; i++)
            {
                latestNode = latestNode.GetNode(pattern[i])
                             ?? latestNode.AddNode(pattern[i]);
            }

            latestNode.IsFinished = true;
        }

        private void SetFailures()
        {
            _rootNode.Failure = _rootNode;
            var queue = new Queue<AhoCorasickTreeNode>();
            queue.Enqueue(_rootNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var node in currentNode.Nodes)
                {
                    queue.Enqueue(node);
                }

                if (currentNode == _rootNode)
                {
                    continue;
                }

                var failure = currentNode.Parent.Failure;
                var key = currentNode.Key;
                while (failure.GetNode(key) == null && failure != _rootNode)
                {
                    failure = failure.Failure;
                }

                failure = failure.GetNode(key);
                if (failure == null || failure == currentNode)
                {
                    failure = _rootNode;
                }

                currentNode.Failure = failure;
                if (!currentNode.IsFinished)
                {
                    currentNode.IsFinished = failure.IsFinished;
                }
            }
        }

        private class AhoCorasickTreeNode
        {
            public readonly AhoCorasickTreeNode Parent;
            public AhoCorasickTreeNode Failure;
            public bool IsFinished;
            public readonly ushort Key;

            internal AhoCorasickTreeNode()
                : this(null, ' ')
            {
            }

            private AhoCorasickTreeNode(AhoCorasickTreeNode parent, ushort key)
            {
                Key = key;
                Parent = parent;
                _size = 0;
                _entries2 = new Entry2[0];
            }

            public AhoCorasickTreeNode[] Nodes
            {
                get { return _entries2.Where(x => x.Key != 0).Select(x => x.Value).ToArray(); }
            }

            private ushort _size;
            private Entry2[] _entries2;
            private struct Entry2
            {
                public ushort Key;
                public AhoCorasickTreeNode Value;
            }


            public AhoCorasickTreeNode GetNode(ushort key)
            {
                if (_size == 0) return null;

                var ind = (ushort)(key & (_entries2.Length - 1));
                var keyThere = _entries2[ind].Key;
                var value = _entries2[ind].Value;
                if (keyThere != 0 && (keyThere == key))
                {
                    return value;
                }

                return null;
            }

            public AhoCorasickTreeNode AddNode(ushort key)
            {
                var node = new AhoCorasickTreeNode(this, key);

                if (_size == 0)
                {
                    Resize2();
                }

                while (true)
                {
                    var ind = (ushort)(key & (_size - 1));

                    if (_entries2[ind].Key != 0 && _entries2[ind].Key != key)
                    {
                        Resize2();
                        continue;
                    }

                    _entries2[ind].Key = key;
                    _entries2[ind].Value = node;
                    return node;
                }
            }

            private void Resize2()
            {
                _size = (ushort)(Math.Max(_size, (ushort)1) * 2);

                var newEntries = new Entry2[_size];
                for (var i = 0; i < _entries2.Length; i++)
                {
                    var key = _entries2[i].Key;
                    var value = _entries2[i].Value;
                    var ind = (ushort)(key & (_size - 1));

                    if (newEntries[ind].Key != 0 && newEntries[ind].Key != key)
                    {
                        Resize2();
                        return;
                    }
                    newEntries[ind].Key = key;
                    newEntries[ind].Value = value;
                }
                _entries2 = newEntries;
                
            }
        }
    }
}