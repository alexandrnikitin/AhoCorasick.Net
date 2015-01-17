using System;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeNode
    {
        public readonly AhoCorasickTreeNode Parent;
        public AhoCorasickTreeNode Failure;
        public bool IsFinished;
        public readonly char Key;

        private int[] _buckets;
        private int _count;
        private Entry[] _entries;

        public AhoCorasickTreeNode()
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
            _buckets[targetBucket] = _count;
            _count++;
            
            return node;
        }

        public AhoCorasickTreeNode GetNode(char key)
        {
            if (_count == 0)
            {
                return null;
            }

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

            // rebalancing buckets
            for (var i = 0; i < newSize; i++)
            {
                var bucket = newEntries[i].Key % newSize;
                newEntries[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }

            _buckets = newBuckets;
            _entries = newEntries;
        }
    }
}