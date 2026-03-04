using Microsoft.Maui.Storage;
using RunningBuddy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json; //for saving
using System.Threading.Tasks;

namespace RunningBuddy.Services
{
    internal class RouteServiceProxy
    {
        //public List<Route> routes;

        private List<Route> _routeList;
        //private List<Route> _dummyList;
        public List<Route> RouteList
        {
            get
            {
                if (_routeList != null)
                {
                    return _routeList.ToList();
                }
                else
                {
                    return null;
                }
            }

            private set
            {
                if (value != _routeList)
                {
                    _routeList = value;
                }
            }
        }

        

        private RouteServiceProxy()
        {
            string path = FileSystem.AppDataDirectory;
            string fileName = "routes.json";
            string fullPath = Path.Combine(path, fileName);

            // 1. Check if the file even exists before trying to read it
            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _routeList = JsonSerializer.Deserialize<List<Route>>(rawData) ?? new List<Route>();
                }
                catch (Exception)
                {
                    // If the JSON is corrupted, fall back to a new list
                    _routeList = new List<Route>();
                }
            }
            else
            {
                // 2. Initialize with default data if it's the first time running
                _routeList = new List<Route>
                {
                    new Route { Id = 0, Name = "Example Route 1", Length = 6, Elevation = 192},
                    new Route { Id = 1, Name = "Example Route 2", Length = 4, Elevation= 117},
                    new Route { Id = 2, Name = "Example Route 3", Length = 15, Elevation = 12}
                };

                // Optionally save this default file immediately
                Save();
            }

        }

        private static RouteServiceProxy? instance;

        private int nextKey
        {
            get
            {
                if (RouteList.Any())
                {
                    return RouteList.Select(t => t.Id).Max() + 1;
                }
                return 1;
            }
        }

        public static RouteServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new RouteServiceProxy();
                }

                return instance;
            }
        }
        public Route? AddOrUpdate(Route? route)
        {
            if (route != null && route.Id == 0)
            {
                route.Id = nextKey;
                //Routes.Add(route);
                _routeList.Add(route);

            }
            else if (route != null)
            {
                var existingRoute = _routeList.FirstOrDefault(t => t.Id == route.Id);

                if (existingRoute != null)
                {
                    var index = _routeList.IndexOf(existingRoute);
                    _routeList.RemoveAt(index); //remove old route
                    _routeList.Insert(index, route); //replace with new edited route
                }


            }

            Save();

            return route; //was void fn
        }

        public void DisplayRoutes()
        {
            RouteList.ForEach(Console.WriteLine);

        }

        public Route? GetById(int id)
        {
            return RouteList.FirstOrDefault(t => t.Id == id);
        }

        public void DeleteRoute(int id)
        {
            
            var localRoute = _routeList.FirstOrDefault(t => t.Id == id);
            if (localRoute != null)
            {
                _routeList.Remove(localRoute);
            }
            Save(); //doesnt hurt to have extra saving
        }


        

        public void Save()
        {
            //save
            string path = FileSystem.AppDataDirectory;
            string fileName = $"routes.json";
            string fullPath = Path.Combine(path, fileName);
            var serializedData = JsonSerializer.Serialize(_routeList);
            File.WriteAllText(fullPath, serializedData);
        }


    }
}
