using System;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeNode
    {
        public readonly AhoCorasickTreeNode Parent;
        public AhoCorasickTreeNode Failure;
        public bool IsFinished;
        public readonly char Value;

        private int[] _buckets;
        private int _count;
        private Entry[] _entries;

        public AhoCorasickTreeNode()
            : this(null, ' ')
        {
        }

        private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char value)
        {
            Value = value;
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
            Resize(_count + 1);

            var value = new AhoCorasickTreeNode(this, key);
            var targetBucket = key % _count;

            for (var i = _buckets[targetBucket]; i >= 0; i = _entries[i].Next)
            {
                if (_entries[i].Key == key)
                {
                    _entries[i].Value = value;
                    return value;
                }
            }

            var index = _count - 1;

            _entries[index].Next = _buckets[targetBucket];
            _entries[index].Key = key;
            _entries[index].Value = value;
            _buckets[targetBucket] = index;

            return value;
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

            var newTransitions = new Entry[newSize];
            Array.Copy(_entries, 0, newTransitions, 0, _count);

            // rebalancing buckets
            for (var i = 0; i < _count; i++)
            {
                var bucket = newTransitions[i].Key % newSize;
                newTransitions[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }

            _buckets = newBuckets;
            _entries = newTransitions;
            _count = _buckets.Length;
        }
    }
}