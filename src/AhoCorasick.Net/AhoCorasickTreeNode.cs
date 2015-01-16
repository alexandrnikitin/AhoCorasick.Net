using System;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeNode
    {
        public AhoCorasickTreeNode Failure;

        public bool IsFinished;

        private readonly AhoCorasickTreeNode _parent;

        private int[] buckets;
        private int bucketsLength;
        private int count;
        private Entry[] entries;
        private int freeCount;
        private int freeList;

        public AhoCorasickTreeNode()
            : this(null, ' ')
        {
        }

        private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char value)
        {
            Value = value;
            _parent = parent;

            buckets = new int[0];
            entries = new Entry[0];
            freeList = -1;
        }

        public AhoCorasickTreeNode ParentFailure
        {
            get { return _parent == null ? null : _parent.Failure; }
        }

        public AhoCorasickTreeNode[] Transitions
        {
            get { return entries.Select(x => x.Value).ToArray(); }
        }

        public char Value { get; private set; }

        public AhoCorasickTreeNode AddTransition(char key)
        {
            if (count == entries.Length)
            {
                Resize();
            }

            var value = new AhoCorasickTreeNode(this, key);
            var targetBucket = key % buckets.Length;

            for (var i = buckets[targetBucket]; i >= 0; i = entries[i].Next)
            {
                if (entries[i].Key == key)
                {
                    entries[i].Value = value;
                    return value;
                }
            }
            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].Next;
                freeCount--;
            }
            else
            {
                if (count == entries.Length)
                {
                    Resize();
                    targetBucket = key % buckets.Length;
                }
                index = count;
                count++;
            }

            entries[index].Next = buckets[targetBucket];
            entries[index].Key = key;
            entries[index].Value = value;
            buckets[targetBucket] = index;

            return value;
        }

        public bool ContainsTransition(char c)
        {
            return GetTransition(c) != null;
        }

        public AhoCorasickTreeNode GetTransition(char key)
        {
            if (bucketsLength == 0)
            {
                return null;
            }
            var bucketIndex = key % bucketsLength;
            for (var i = buckets[bucketIndex]; i >= 0; i = entries[i].Next)
            {
                if (entries[i].Key == key)
                {
                    return entries[i].Value;
                }
            }

            return null;
        }

        private void Resize()
        {
            Resize(count + 1);
        }

        private void Resize(int newSize)
        {
            var newBuckets = new int[newSize];
            for (var i = 0; i < newBuckets.Length; i++)
            {
                newBuckets[i] = -1;
            }
            var newEntries = new Entry[newSize];
            Array.Copy(entries, 0, newEntries, 0, count);
            for (var i = 0; i < count; i++)
            {
                var bucket = newEntries[i].Key % newSize;
                newEntries[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
            buckets = newBuckets;
            entries = newEntries;

            bucketsLength = buckets.Length;
        }
    }
}