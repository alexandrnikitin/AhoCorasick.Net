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
            private char _size;
            private Entry[] _entries;
            public readonly AhoCorasickTreeNode Parent;
            public AhoCorasickTreeNode Failure;
            public bool IsFinished;
            public readonly char Key;

            internal AhoCorasickTreeNode()
                : this(null, ' ')
            {
            }

            private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char key)
            {
                Key = key;
                Parent = parent;
                _size = (char)0;
                _entries = new Entry[0];
            }

            public AhoCorasickTreeNode[] Nodes
            {
                get { return _entries.Where(x => x.Key != 0).Select(x => x.Value).ToArray(); }
            }

            private struct Entry
            {
                public char Key;
                public AhoCorasickTreeNode Value;
            }


            public AhoCorasickTreeNode GetNode(char key)
            {
                if (_size == 0) return null;

                var ind = (char)(key & (_size - 1));
                var keyThere = _entries[ind].Key;
                var value = _entries[ind].Value;
                if (keyThere != 0 && (keyThere == key))
                {
                    return value;
                }

                return null;
            }

            public AhoCorasickTreeNode AddNode(char key)
            {
                var node = new AhoCorasickTreeNode(this, key);

                if (_size == 0)  Resize();

                while (true)
                {
                    var ind = (char)(key & (_size - 1));

                    if (_entries[ind].Key != 0 && _entries[ind].Key != key)
                    {
                        Resize();
                        continue;
                    }

                    _entries[ind].Key = key;
                    _entries[ind].Value = node;
                    return node;
                }
            }

            private void Resize()
            {
                _size = (char)(Math.Max(_size, (char)1) * 2);

                var newEntries = new Entry[_size];
                for (var i = 0; i < _entries.Length; i++)
                {
                    var key = _entries[i].Key;
                    var value = _entries[i].Value;
                    var ind = (char)(key & (_size - 1));

                    if (newEntries[ind].Key != 0 && newEntries[ind].Key != key)
                    {
                        Resize();
                        return;
                    }
                    newEntries[ind].Key = key;
                    newEntries[ind].Value = value;
                }
                _entries = newEntries;
                
            }
        }
    }
}