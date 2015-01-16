using System;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeNode
    {
        public AhoCorasickTreeNode Failure;

        public bool IsFinished;

        private readonly AhoCorasickTreeNode _parent;

        private int[] _buckets;
        private int _bucketsLength;
        private Transition[] _transitions;

        public AhoCorasickTreeNode()
            : this(null, ' ')
        {
        }

        private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char value)
        {
            Value = value;
            _parent = parent;

            _buckets = new int[0];
            _transitions = new Transition[0];
        }

        public AhoCorasickTreeNode ParentFailure
        {
            get { return _parent == null ? null : _parent.Failure; }
        }

        public AhoCorasickTreeNode[] Transitions
        {
            get { return _transitions.Select(x => x.Value).ToArray(); }
        }

        public char Value { get; private set; }

        public AhoCorasickTreeNode AddTransition(char key)
        {
            Resize(_transitions.Length + 1);

            var value = new AhoCorasickTreeNode(this, key);
            var targetBucket = key % _buckets.Length;

            for (var i = _buckets[targetBucket]; i >= 0; i = _transitions[i].Next)
            {
                if (_transitions[i].Key == key)
                {
                    _transitions[i].Value = value;
                    return value;
                }
            }

            var index = _transitions.Length - 1;

            _transitions[index].Next = _buckets[targetBucket];
            _transitions[index].Key = key;
            _transitions[index].Value = value;
            _buckets[targetBucket] = index;

            return value;
        }

        public bool ContainsTransition(char c)
        {
            return GetTransition(c) != null;
        }

        public AhoCorasickTreeNode GetTransition(char key)
        {
            if (_bucketsLength == 0)
            {
                return null;
            }
            var bucketIndex = key % _bucketsLength;
            for (var i = _buckets[bucketIndex]; i >= 0; i = _transitions[i].Next)
            {
                if (_transitions[i].Key == key)
                {
                    return _transitions[i].Value;
                }
            }

            return null;
        }

        private void Resize(int newSize)
        {
            var newBuckets = new int[newSize];
            for (var i = 0; i < newBuckets.Length; i++)
            {
                newBuckets[i] = -1;
            }

            var newEntries = new Transition[newSize];
            Array.Copy(_transitions, 0, newEntries, 0, _transitions.Length);
            for (var i = 0; i < _transitions.Length; i++)
            {
                var bucket = newEntries[i].Key % newSize;
                newEntries[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
            _buckets = newBuckets;
            _transitions = newEntries;

            _bucketsLength = _buckets.Length;
        }
    }
}