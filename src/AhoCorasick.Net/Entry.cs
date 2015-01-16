namespace AhoCorasick.Net
{
    internal struct Entry
    {
        public char Key;
        public int Next;
        public AhoCorasickTreeNode Value;
    }
}