using System.Globalization;

namespace StcRoutrt
{

    internal class Router : IRouter
    {
        private readonly Node _tree = new();
        private readonly object _registerLock = new();

        public void RegisterRoute<T1>(string route, Action<T1> action)
            where T1 : IParsable<T1>
        {
            var map = MapArgs(route, action);
            RegisterRoute(route, args =>
            {
                if (!T1.TryParse(args[map[0]], CultureInfo.InvariantCulture, out var a)) return false;
                action(a);
                return true;
            });
        }

        public void RegisterRoute<T1, T2>(string route, Action<T1, T2> action) 
            where T1 : IParsable<T1> where T2 : IParsable<T2>
        {
            var map = MapArgs(route, action);
            RegisterRoute(route, args =>
            {
                if (!T1.TryParse(args[map[0]], CultureInfo.InvariantCulture, out var a)) return false;
                if (!T2.TryParse(args[map[1]], CultureInfo.InvariantCulture, out var b)) return false;
                action(a, b);
                return true;
            });
        }

        // map[i] - позиция в пути параметра делегата с индексом i (сопоставление по имени)
        private static int[] MapArgs(string route, Delegate action)
        {
            var names = route.Split('/', StringSplitOptions.RemoveEmptyEntries)
                .Where(e => e.StartsWith('{') && e.EndsWith('}'))
                .Select(e => e[1..^1].Split(':')[0])
                .ToList();

            var parameters = action.Method.GetParameters();
            if (parameters.Length != names.Count)
                throw new ArgumentException($"Маршрут '{route}': параметров в шаблоне {names.Count}, в обработчике {parameters.Length}");

            var map = new int[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                map[i] = names.IndexOf(parameters[i].Name!);
                if (map[i] < 0)
                    throw new ArgumentException($"Маршрут '{route}': параметра '{parameters[i].Name}' нет в шаблоне");
            }
            return map;
        }

        internal void RegisterRoute(string route, Func<string[], bool> handler)
        {
            lock (_registerLock)
            {
                var elements = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var current = _tree;

                foreach (var element in elements)
                {
                    if (element.StartsWith('{') && element.EndsWith('}'))
                    {
                        var parts = element[1..^1].Split(':');
                        var type = parts.Length > 1 ? parts[1].ToLowerInvariant() : "";
                        current = current.Params.GetOrAdd(type, _ => new Node());
                    }
                    else
                    {
                        if (!current.Static.TryGetValue(element, out var childNode))
                        {
                            childNode = new Node();
                            current.Static[element] = childNode;
                        }
                        current = childNode;
                    }
                }

                if (current.Handler is not null)
                    throw new ArgumentException($"Маршрут '{route}' уже зарегестрирован!");
                current.Handler = handler;
            }
        }

        public void Route(string route)
        {
            var elements = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var found = false;

            if (TryRoute(_tree, elements, 0, new List<string>(), ref found))
                return;

            if (found)
                Console.WriteLine($"Маршрут '{route}' найден, но параметры не прошли проверку!");
            else
                Console.WriteLine($"Маршрут '{route}' не найден!");
        }

        // поиск с откатом: сначала Static, потом каждый узел из Params;
        // если ветка не дошла до обработчика или он вернул false - пробуем следующую
        private static bool TryRoute(Node node, string[] elements, int index, List<string> args, ref bool found)
        {
            if (index == elements.Length)
            {
                if (node.Handler is null)
                    return false;
                found = true;
                return node.Handler(args.ToArray());
            }

            var element = elements[index];

            if (node.Static.TryGetValue(element, out var childNode)
                && TryRoute(childNode, elements, index + 1, args, ref found))
                return true;

            foreach (var paramNode in node.Params.Values)
            {
                args.Add(element);
                if (TryRoute(paramNode, elements, index + 1, args, ref found))
                    return true;
                args.RemoveAt(args.Count - 1);
            }

            return false;
        }
    }
}
