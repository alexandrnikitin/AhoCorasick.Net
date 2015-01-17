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
            }
        }
    }
}