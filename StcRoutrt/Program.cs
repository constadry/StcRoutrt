using StcRoutrt;

var r = new Router();

r.RegisterRoute<int>("/foo/bar/{p:int}/", p => Console.WriteLine(p));
r.RegisterRoute<DateTime>("/events/{d:datetime}/", d => Console.WriteLine(d.ToString("O")));
r.RegisterRoute<Guid>("/users/{id:guid}/", id => Console.WriteLine(id));
r.RegisterRoute<string>("/hello/{name:string}/", name => Console.WriteLine($"Hello, {name}"));
r.RegisterRoute<decimal>("/price/{value:decimal}/", v => Console.WriteLine(v));

r.Route("/foo/bar/1");                                          // 1
r.Route("/events/2026-10-17T14:46:15/");                        // 2026-10-17T14:46:15.0000000
r.Route("/users/3f2504e0-4f89-11d3-9a0c-0305e82c3301");         // 3f2504e0-4f89-11d3-9a0c-0305e82c3301
r.Route("/hello/world");                                        // Hello, world
r.Route("/price/19.99");


r.RegisterRoute<int, int>("/sum/{a:int}/{b:int}/", (a, b) => Console.WriteLine(a + b));
r.RegisterRoute<DateTime, int>("/schedule/{d:datetime}/{n:int}/", (d, n) => Console.WriteLine(d.ToString("yyyy-MM-dd") + " x" + n));
r.RegisterRoute<Guid, string>("/actions/{id:guid}/{action:string}/", (id, action) => Console.WriteLine(id + " " + action));
r.RegisterRoute<string, decimal>("/catalog/{name:string}/{p:decimal}/", (name, p) => Console.WriteLine(name + " = " + p));
r.RegisterRoute<Guid, DateTime>("/bookings/{id:guid}/{d:datetime}/", (id, d) => Console.WriteLine(id + " at " + d.ToString("O")));

r.Route("/sum/1/2");                                                        // 3
r.Route("/schedule/2026-10-17/5");                                          // 2026-10-17 x5
r.Route("/actions/3f2504e0-4f89-11d3-9a0c-0305e82c3301/delete");            // 3f2504e0-... delete
r.Route("/catalog/coffee/19.99");                                           // coffee = 19.99
r.Route("/bookings/3f2504e0-4f89-11d3-9a0c-0305e82c3301/2026-10-17T14:50:00"); // 3f2504e0-... at 2026-10-17T14:50:00.0000000