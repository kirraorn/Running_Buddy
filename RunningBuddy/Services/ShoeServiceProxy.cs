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
    internal class ShoeServiceProxy
    {
        //public List<Shoe> shoes;

        private List<Shoe> _shoeList;
        //private List<Shoe> _dummyList;
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

            private set
            {
                if (value != _shoeList)
                {
                    _shoeList = value;
                }
            }
        }



        private ShoeServiceProxy()
        {
            string path = FileSystem.AppDataDirectory;
            string fileName = "shoes.json";
            string fullPath = Path.Combine(path, fileName);

            // 1. Check if the file even exists before trying to read it
            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _shoeList = JsonSerializer.Deserialize<List<Shoe>>(rawData) ?? new List<Shoe>();
                }
                catch (Exception)
                {
                    // If the JSON is corrupted, fall back to a new list
                    _shoeList = new List<Shoe>();
                }
            }
            else
            {
                // 2. Initialize with default data if it's the first time running
                _shoeList = new List<Shoe>
                {
                    new Shoe { Id = 0, Name = "Launch 12", BrandName = "Brooks", CurrentMilage = 121, MaxMilage = 400},
                    new Shoe { Id = 1, Name = "Example Shoe 2", BrandName = "Nike", CurrentMilage = 200, MaxMilage = 300},
                    new Shoe { Id = 2, Name = "Example Shoe 3", BrandName = "Addias", CurrentMilage = 65, MaxMilage = 500}
                };

                // Optionally save this default file immediately
                Save();
            }

        }

        private static ShoeServiceProxy? instance;

        private int nextKey
        {
            get
            {
                if (ShoeList.Any())
                {
                    return ShoeList.Select(t => t.Id).Max() + 1;
                }
                return 1;
            }
        }

        public static ShoeServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new ShoeServiceProxy();
                }

                return instance;
            }
        }
        public Shoe? AddOrUpdate(Shoe? shoe)
        {
            if (shoe != null && shoe.Id == 0)
            {
                shoe.Id = nextKey;
                //Shoes.Add(shoe);
                _shoeList.Add(shoe);

            }
            else if (shoe != null)
            {
                var existingShoe = _shoeList.FirstOrDefault(t => t.Id == shoe.Id);

                if (existingShoe != null)
                {
                    var index = _shoeList.IndexOf(existingShoe);
                    _shoeList.RemoveAt(index); //remove old shoe
                    _shoeList.Insert(index, shoe); //replace with new edited shoe
                }


            }

            Save();

            return shoe; //was void fn
        }

        public void DisplayShoes()
        {
            ShoeList.ForEach(Console.WriteLine);

        }

        public Shoe? GetById(int id)
        {
            return ShoeList.FirstOrDefault(t => t.Id == id);
        }

        public void DeleteShoe(int id)
        {

            var localShoe = _shoeList.FirstOrDefault(t => t.Id == id);
            if (localShoe != null)
            {
                _shoeList.Remove(localShoe);
            }
            Save(); //doesnt hurt to have extra saving
        }




        public void Save()
        {
            //save
            string path = FileSystem.AppDataDirectory;
            string fileName = $"shoes.json";
            string fullPath = Path.Combine(path, fileName);
            var serializedData = JsonSerializer.Serialize(_shoeList);
            File.WriteAllText(fullPath, serializedData);
        }


    }
}
