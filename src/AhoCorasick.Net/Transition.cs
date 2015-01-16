namespace AhoCorasick.Net
{
    internal struct Transition
    {
        public char Key;
        public int Next;
        public AhoCorasickTreeNode Value;
    }
}