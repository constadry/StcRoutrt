using System.Globalization;

namespace StcRoutrt
{

    internal class Router : IRouter
    {
        private readonly Node _tree = new();
        private readonly object _registerLock = new();

        public void RegisterRoute<T1>(string route, Action<T1> action)
            where T1 : IParsable<T1>
            => RegisterRoute(route, args =>
            {
                if (!T1.TryParse(args[0], CultureInfo.InvariantCulture, out var a)) return false;
                action(a);
                return true;
            });

        public void RegisterRoute<T1, T2>(string route, Action<T1, T2> action) 
            where T1 : IParsable<T1> where T2 : IParsable<T2>
            => RegisterRoute(route, args =>
            {
                if (!T1.TryParse(args[0], CultureInfo.InvariantCulture, out var a)) return false;
                if (!T2.TryParse(args[1], CultureInfo.InvariantCulture, out var b)) return false;
                action(a, b);
                return true;
            });

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
                        current.Param ??= new Node();
                        current = current.Param;
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
            var current = _tree;

            string? par1 = null;
            string? par2 = null;

            foreach (var element in elements)
            {
                if (!current.Static.TryGetValue(element, out var childNode))
                {
                    var paramNode = current.Param;
                    if (paramNode is null)
                    {
                        Console.WriteLine($"Маршрут '{route}' не найден!");
                        return;
                    } else
                    {
                        if (par1 is null)
                            par1 = element;
                        else if (par2 is null)
                            par2 = element;
                        else
                        {
                            Console.WriteLine($"Маршрут '{route}' не найден!");
                            return;
                        }
                        current = paramNode;
                    }
                } else
                {
                    current = childNode;
                }
            }

            if (current.Handler is null)
            {
                Console.WriteLine($"Маршрут '{route}' не найден!");
                return;
            }
            else
            {
                if (!current.Handler([par1!, par2!]))
                    Console.WriteLine($"Маршрут '{route}' найден, но параметры не прошли проверку!");
            }
        }
    }
}
