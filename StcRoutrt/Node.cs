using System.Collections.Concurrent;

namespace StcRoutrt
{
    internal class Node
    {
        public ConcurrentDictionary<string, Node> Static = new();
        public ConcurrentDictionary<string, Node> Params = new(); // тип из шаблона ("guid", "int") -> узел
        public Func<string[], bool>? Handler;
    }
}
