using System;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeNode
    {
        private const int MaxPrimeArrayLength = 0x7FEFFFFD;

        private static readonly int[] primes =
            {
                3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919, 
                1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 
                17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437, 
                187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263, 
                1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
            };

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
            get { return entries.Select(x => x.value).ToArray(); }
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

            for (var i = buckets[targetBucket]; i >= 0; i = entries[i].next)
            {
                if (entries[i].key == key)
                {
                    entries[i].value = value;
                    return value;
                }
            }
            int index;
            if (freeCount > 0)
            {
                index = freeList;
                freeList = entries[index].next;
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

            entries[index].next = buckets[targetBucket];
            entries[index].key = key;
            entries[index].value = value;
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
            for (var i = buckets[bucketIndex]; i >= 0; i = entries[i].next)
            {
                if (entries[i].key == key)
                {
                    return entries[i].value;
                }
            }

            return null;
        }

        private static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }

        private static int GetPrime(int min)
        {
            for (var i = 0; i < primes.Length; i++)
            {
                var prime = primes[i];
                if (prime >= min) return prime;
            }

            return min;
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
                var bucket = newEntries[i].key % newSize;
                newEntries[i].next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
            buckets = newBuckets;
            entries = newEntries;

            bucketsLength = buckets.Length;
        }

        private struct Entry
        {
            public char key;
            public int next;
            public AhoCorasickTreeNode value;
        }
    }
}