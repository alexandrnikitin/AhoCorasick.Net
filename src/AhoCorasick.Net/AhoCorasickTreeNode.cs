using System;
using System.Linq;

namespace AhoCorasick.Net
{
    public class AhoCorasickTreeNode
    {
        public AhoCorasickTreeNode Failure;
        public bool IsFinished;
        public char Value;

        public readonly AhoCorasickTreeNode Parent;

        private int[] _buckets;
        private int _count;
        private Transition[] _transitions;

        public AhoCorasickTreeNode()
            : this(null, ' ')
        {
        }

        private AhoCorasickTreeNode(AhoCorasickTreeNode parent, char value)
        {
            Value = value;
            Parent = parent;

            _buckets = new int[0];
            _transitions = new Transition[0];
        }

        public AhoCorasickTreeNode[] Transitions
        {
            get { return _transitions.Select(x => x.Value).ToArray(); }
        }

        public AhoCorasickTreeNode AddTransition(char key)
        {
            Resize(_count + 1);

            var value = new AhoCorasickTreeNode(this, key);
            var targetBucket = key % _count;

            for (var i = _buckets[targetBucket]; i >= 0; i = _transitions[i].Next)
            {
                if (_transitions[i].Key == key)
                {
                    _transitions[i].Value = value;
                    return value;
                }
            }

            var index = _count - 1;

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
            if (_count == 0)
            {
                return null;
            }
            var bucketIndex = key % _count;
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
            for (var i = 0; i < newSize; i++)
            {
                newBuckets[i] = -1;
            }

            var newTransitions = new Transition[newSize];
            Array.Copy(_transitions, 0, newTransitions, 0, _count);

            // rebalancing buckets
            for (var i = 0; i < _count; i++)
            {
                var bucket = newTransitions[i].Key % newSize;
                newTransitions[i].Next = newBuckets[bucket];
                newBuckets[bucket] = i;
            }

            _buckets = newBuckets;
            _transitions = newTransitions;
            _count = _buckets.Length;
        }
    }
}