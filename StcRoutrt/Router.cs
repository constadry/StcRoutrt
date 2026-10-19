using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StcRoutrt
{
    internal class Router
    {
        private readonly ConcurrentDictionary<string, Action<string>> _routesOneArg = new();
        private readonly ConcurrentDictionary<string, Action<string, string>> _routesTwoArg = new();

        internal void RegisterRoute<T1>(string route, Action<T1> action) where T1 : IParsable<T1>
        {
            var parametr1 = route.Split('/').Where(x => x.Contains(':')).FirstOrDefault();
            var routeWithoutOneParameter = route.Remove(route.Length - 1).Replace($"/{parametr1}", "");

            if (!_routesOneArg.TryAdd(routeWithoutOneParameter, (obj) => action(Parse<T1>(obj))))
            {
                throw new InvalidOperationException("Маршрут уже зарегистрирован");
            }
        }

        internal void RegisterRoute<T1, T2>(string route, Action<T1, T2> action) where T1 : IParsable<T1> where T2 : IParsable<T2>
        {
            var parametr1 = route.Remove(route.Length - 1).Split('/').ToList().LastOrDefault();
            var parametr2 = route.Remove(route.Length - 1).Split('/').ToList().TakeLast(2).FirstOrDefault();

            var routeWithoutOneParameter = route.Remove(route.Length - 1).Replace($"/{parametr1}", "");
            var routeWithoutTwoParameters = routeWithoutOneParameter.Replace($"/{parametr2}", "");
            if (!_routesTwoArg.TryAdd(routeWithoutTwoParameters, (obj1, obj2) => action(Parse<T1>(obj1), Parse<T2>(obj2))))
            {
                throw new InvalidOperationException("Маршрут уже зарегистрирован");
            }
        }

        internal void Route(string route)
        {
            var parametr1 = route.Split('/').ToList().TakeLast(2).FirstOrDefault();
            var parametr2 = route.Split('/').ToList().TakeLast(2).LastOrDefault();

            var routeWithoutOneParameter = route.Replace($"/{parametr2}", "");
            var routeWithoutTwoParameters = routeWithoutOneParameter.Replace($"/{parametr1}", "");
            if (_routesOneArg.TryGetValue(routeWithoutOneParameter, out var action) && action != null) 
            {
                if (parametr2 != null) 
                {
                    action(parametr2);
                }
            } else if (_routesTwoArg.TryGetValue(routeWithoutTwoParameters, out var actionTwo) && actionTwo != null)
            {
                if (parametr1 != null && parametr2 != null)
                {
                    actionTwo(parametr1, parametr2);
                }
            }
        }

        static T Parse<T>(string s) where T : IParsable<T> => T.Parse(s, CultureInfo.InvariantCulture);
    }
}
