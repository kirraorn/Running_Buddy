//using Android.Media;
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




//This class handles all the user data that needs to be persistant

//TODO: Move commented out code to repsective service proxy classes
namespace RunningBuddy.Services
{
    internal class UserServiceProxy
    {


        //private vars---------------------------------------------------------
        private User _user1;
        //private List<Routes> _routeList;
        //private List<Shoe> _shoeList;



        //public access--------------------------------------------------------

        public User MainUser
        {
            get
            {
                return _user1;
            }

            set
            {
                if (value != _user1)
                {
                    _user1 = value;
                }
            }
        }

        /*
        public List<Routes> RouteList
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
            set
            {
                if (value != _routeList)
                {
                    _routeList = value;
                }
            }
        }

        public List<Shoe> ShoeList
        {
            get
            {
                if (_shoeList != null)
                {
                    return _shoeList.ToList();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != _shoeList)
                {
                    _shoeList = value;
                }
            }
        }
        */

       

        private UserServiceProxy()
        {
            string path = FileSystem.AppDataDirectory;
            string fileName = "user.json";
            string fullPath = Path.Combine(path, fileName);

            // 1. Check if the file even exists before trying to read it
            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _user1 = JsonSerializer.Deserialize<User>(rawData) ?? new User();
                }
                catch (Exception)
                {
                    // If the JSON is corrupted, fall back to a new list
                    _user1 = new User();
                }
            }
            else
            {
                // 2. Initialize with default data if it's the first time running
                _user1 = new User
                {
                    Name = "Default Dave",
                    Id = 0,
                    ColdPreference = 0,
                    TotalMilage = 0
                };

                // Optionally save this default file immediately
                Save("user.json", _user1);
            }

        }

        private static UserServiceProxy? instance;


        public static UserServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserServiceProxy();
                }

                return instance;
            }
        }
        public User? AddOrUpdate(User? user)
        {
            if (user != null && user.Id == 0)
            {
                
                _user1 = user;
            }
            

            Save("user.json", _user1);

            return user;
        }

        


        public void Save<T>(string fileName, T obj1)
        {
            // Path construction
            string path = FileSystem.AppDataDirectory;
            string fullPath = Path.Combine(path, fileName);

            // Serialization and writing to file
            string serializedData = JsonSerializer.Serialize(obj1);
            File.WriteAllText(fullPath, serializedData);
        }

        public void SaveAll()
        {
            Save("user.json", _user1);
            //Save("shoes.json", _shoeList);
            //Save("routes.json", _routeList);
        }
    }
}
