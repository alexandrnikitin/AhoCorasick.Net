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

            SetFailureNodes();
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

        private List<AhoCorasickTreeNode> FailToRootNode()
        {
            var nodes = new List<AhoCorasickTreeNode>();
            foreach (var node in _rootNode.Transitions)
            {
                node.Failure = _rootNode;
                nodes.AddRange(node.Transitions);
            }
            return nodes;
        }

        private void FailUsingBFS(List<AhoCorasickTreeNode> nodes)
        {
            while (nodes.Count != 0)
            {
                var newNodes = new List<AhoCorasickTreeNode>();
                foreach (var node in nodes)
                {
                    var failure = node.ParentFailure;
                    var value = node.Value;

                    while (failure != null && !failure.ContainsTransition(value))
                    {
                        failure = failure.Failure;
                    }

                    if (failure == null)
                    {
                        node.Failure = _rootNode;
                    }
                    else
                    {
                        node.Failure = failure.GetTransition(value);
                        if (!node.IsFinished)
                        {
                            node.IsFinished = node.Failure.IsFinished;
                        }
                    }

                    newNodes.AddRange(node.Transitions);
                }
                nodes = newNodes;
            }
        }

        private void SetFailureNodes()
        {
            var nodes = FailToRootNode();
            FailUsingBFS(nodes);
            _rootNode.Failure = _rootNode;
        }
    }
}