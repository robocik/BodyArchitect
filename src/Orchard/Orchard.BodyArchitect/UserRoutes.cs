using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Orchard.BodyArchitect
{
    public class UserRoutes : IRouteProvider
    {
        public UserRoutes()
        {
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                             new RouteDescriptor {
                                                    Priority = 9,
                                                    Route = new Route(
                                                        "Users/Account/Register",
                                                        new RouteValueDictionary {
                                                                                    {"area", "Orchard.BodyArchitect"},
                                                                                    {"controller", "Account"},
                                                                                    {"action", "Register"}
                                                                                },
                                                        new RouteValueDictionary(),
                                                        new RouteValueDictionary {
                                                                                    {"area", "Orchard.BodyArchitect"}
                                                                                },
                                                        new MvcRouteHandler())
                                                    },
                            new RouteDescriptor {
                                                    Priority = 9,
                                                    Route = new Route(
                                                        "Users/Account/LogOn",
                                                        new RouteValueDictionary {
                                                                                    {"area", "Orchard.BodyArchitect"},
                                                                                    {"controller", "Account"},
                                                                                    {"action", "LogOn"}
                                                                                },
                                                        new RouteValueDictionary(),
                                                        new RouteValueDictionary {
                                                                                    {"area", "Orchard.BodyArchitect"}
                                                                                },
                                                        new MvcRouteHandler())
                                                    }
            };

        }
    }
}