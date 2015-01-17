using System;
using System.Collections.Generic;

namespace AhoCorasick.Net
{
    public class AhoCorasickTree
    {
        private readonly AhoCorasickTreeNode _rootNode;

        public AhoCorasickTree(string[] keywords)
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
                    var transition = currentNode.GetTransition(text[i]);
                    if (transition == null)
                    {
                        currentNode = currentNode.Failure;
                        if (currentNode == _rootNode)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (transition.IsFinished)
                        {
                            return true;
                        }

                        currentNode = transition;
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
                latestNode = latestNode.GetTransition(pattern[i])
                             ?? latestNode.AddTransition(pattern[i]);
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
                foreach (var node in currentNode.Transitions)
                {
                    queue.Enqueue(node);
                }

                if (currentNode == _rootNode)
                {
                    continue;
                }

                var failure = currentNode.Parent.Failure;
                var value = currentNode.Value;
                while (failure.GetTransition(value) != null && failure != _rootNode)
                {
                    failure = failure.Failure;
                }

                failure = failure.GetTransition(value);
                if (failure == null || failure == currentNode)
                {
                    failure = _rootNode;
                }

                currentNode.Failure = failure;
            }
        }
    }
}