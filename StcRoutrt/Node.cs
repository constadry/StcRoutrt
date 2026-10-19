using System.Collections.Concurrent;

namespace StcRoutrt
{
    internal class Node
    {
        public ConcurrentDictionary<string, Node> Static = new();
        public Node? Param;
        public Func<string[], bool>? Handler;
    }
}
