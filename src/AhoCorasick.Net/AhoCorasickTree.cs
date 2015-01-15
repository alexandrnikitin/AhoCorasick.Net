using System.Collections.Generic;

namespace AhoCorasick.Net
{
    public class AhoCorasickTree
    {
        public AhoCorasickTreeNode Root;

        public AhoCorasickTree(IEnumerable<string> keywords)
        {
            Root = new AhoCorasickTreeNode();

            if (keywords != null)
            {
                foreach (var p in keywords)
                {
                    AddPatternToTree(p);
                }

                SetFailureNodes();
            }
        }

        public bool Contains(string text)
        {
            var pointer = Root;

            var length = text.Length;
            for (var i = 0; i < length; i++)
            {
                while (true)
                {
                    var transition = pointer.GetTransition(text[i]);
                    if (transition == null)
                    {
                        pointer = pointer.Failure;
                        if (pointer == Root)
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
            Root.Failure = Root;
        }

        private void AddPatternToTree(string pattern)
        {
            var node = Root;
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
            foreach (var node in Root.Transitions)
            {
                node.Failure = Root;
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
                        node.Failure = Root;
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