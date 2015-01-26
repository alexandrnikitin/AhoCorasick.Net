using System;
using System.Collections.Generic;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeBasedOnArray
    {
        private readonly AhoCorasickTreeNode _rootNode;
        private int[][] _array;

        public AhoCorasickTreeBasedOnArray(string[] keywords)
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
            ToJaggedArray();
        }

        public void ToJaggedArray()
        {
            var maxId = SetIds();

            _array = new int[maxId][];

            var queue = new Queue<AhoCorasickTreeNode>();
            queue.Enqueue(_rootNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var node in currentNode.Nodes)
                {
                    queue.Enqueue(node);
                }

                var id = currentNode.Id;
                var nodeCount = currentNode._count;
                var num = 1 + 1 + 1 + nodeCount + (nodeCount * 3);
                _array[id] = new int[num];
                _array[id][0] = currentNode.IsFinished ? 1 : 0;
                _array[id][1] = nodeCount;
                _array[id][2] = currentNode.Failure.Id;

                for (var i = 0; i < nodeCount; i++)
                {
                    _array[id][3 + i] = currentNode._buckets[i];
                }

                for (var i = 0; i < nodeCount; i++)
                {
                    var entry = currentNode._entries[i];
                    _array[id][3 + nodeCount + (i * 3)] = entry.Key;
                    _array[id][3 + nodeCount + (i * 3) + 1] = entry.Next;
                    _array[id][3 + nodeCount + (i * 3) + 2] = entry.Value.Id;
                }

            }

        }

        private int SetIds()
        {
            var i = 0;

            var queue = new Queue<AhoCorasickTreeNode>();
            queue.Enqueue(_rootNode);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                foreach (var node in currentNode.Nodes)
                {
                    queue.Enqueue(node);
                }
                currentNode.Id = i++;
            }
            return i;
        }

        public bool Contains(string text)
        {
            var currentNode = 0;

            var length = text.Length;
            for (var i = 0; i < length; i++)
            {
                while (true)
                {
                    var node = -1;
                    var count = _array[currentNode][1];
                    if (count != 0)
                    {
                        var key = text[i];
                        var bucketIndex = key % count;
                        
                        for (var j = _array[currentNode][3 + bucketIndex]; j >= 0; j = _array[currentNode][3 + count + (3 * j) + 1])
                        {
                            if (_array[currentNode][3 + count + (3 * j)] == key)
                            {
                                node = _array[currentNode][3 + count + (3 * j) + 2];
                            }
                        }
                        
                    }

                    if (node == -1)
                    {
                        currentNode = _array[currentNode][2];
                        if (currentNode == 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (_array[node][0] == 1)
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
                while (failure.GetNode(key) != null && failure != _rootNode)
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
            public int Id;
            public readonly AhoCorasickTreeNode Parent;
            public AhoCorasickTreeNode Failure;
            public bool IsFinished;
            public readonly char Key;

            public int[] _buckets;
            public int _count;
            public Entry[] _entries;

            internal AhoCorasickTreeNode()
                : this(null, ' ')
            {
            }

            private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char key)
            {
                Key = key;
                Parent = parent;

                _buckets = new int[0];
                _entries = new Entry[0];
            }

            public AhoCorasickTreeNode[] Nodes
            {
                get { return _entries.Select(x => x.Value).ToArray(); }
            }

            public AhoCorasickTreeNode AddNode(char key)
            {
                var node = new AhoCorasickTreeNode(this, key);

                var newSize = _count + 1;
                Resize(newSize);

                var targetBucket = key % newSize;
                _entries[_count].Key = key;
                _entries[_count].Value = node;
                _entries[_count].Next = _buckets[targetBucket];
                _buckets[targetBucket] = _count;
                _count++;

                return node;
            }

            public AhoCorasickTreeNode GetNode(char key)
            {
                if (_count == 0) return null;

                var bucketIndex = key % _count;
                for (var i = _buckets[bucketIndex]; i >= 0; i = _entries[i].Next)
                {
                    if (_entries[i].Key == key)
                    {
                        return _entries[i].Value;
                    }
                }

                return null;
            }

            private void Resize(int newSize)
            {
                var newBuckets = new int[newSize];
                for (var i = 0; i < newSize; i++)
                {
                    newBuckets[i] = -1;
                }

                var newEntries = new Entry[newSize];
                Array.Copy(_entries, 0, newEntries, 0, _entries.Length);

                // rebalancing buckets for existing entries
                for (var i = 0; i < _entries.Length; i++)
                {
                    var bucket = newEntries[i].Key % newSize;
                    newEntries[i].Next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }

                _buckets = newBuckets;
                _entries = newEntries;
            }

            internal struct Entry
            {
                public char Key;
                public int Next;
                public AhoCorasickTreeNode Value;
            }
        }
    }
}