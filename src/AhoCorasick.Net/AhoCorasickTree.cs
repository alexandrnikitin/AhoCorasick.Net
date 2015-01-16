using System;
using System.Collections.Generic;

namespace AhoCorasick.Net
{
    public class AhoCorasickTree
    {
        private readonly AhoCorasickTreeNode _root;

        public AhoCorasickTree(string[] keywords)
        {
            if (keywords == null) throw new ArgumentNullException("keywords");
            if (keywords.Length == 0) throw new ArgumentException("should contain keywords");

            _root = new AhoCorasickTreeNode();

            foreach (var keyword in keywords)
            {
                AddPatternToTree(keyword);
            }

            SetFailureNodes();
        }

        public bool Contains(string text)
        {
            var pointer = _root;

            var length = text.Length;
            for (var i = 0; i < length; i++)
            {
                while (true)
                {
                    var transition = pointer.GetTransition(text[i]);
                    if (transition == null)
                    {
                        pointer = pointer.Failure;
                        if (pointer == _root)
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

                        pointer = transition;
                        break;
                    }
                }
            }

            return false;
        }

        private void SetFailureNodes()
        {
            var nodes = FailToRootNode();
            FailUsingBFS(nodes);
            _root.Failure = _root;
        }

        private void AddPatternToTree(string pattern)
        {
            var node = _root;
            foreach (var c in pattern)
            {
                node = node.GetTransition(c)
                       ?? node.AddTransition(c);
            }
            node.IsFinished = true;
        }

        private List<AhoCorasickTreeNode> FailToRootNode()
        {
            var nodes = new List<AhoCorasickTreeNode>();
            foreach (var node in _root.Transitions)
            {
                node.Failure = _root;
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
                        node.Failure = _root;
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
    }
}