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
    internal class PrServiceProxy
    {
        //public List<PR> prs;

        private List<PR> _prList;
        //private List<PR> _dummyList;
        public List<PR> PRList
        {
            get
            {
                if (_prList != null)
                {
                    return _prList.ToList();
                }
                else
                {
                    return null;
                }
            }

            private set
            {
                if (value != _prList)
                {
                    _prList = value;
                }
            }
        }



        private PrServiceProxy()
        {
            string path = FileSystem.AppDataDirectory;
            string fileName = "prs.json";
            string fullPath = Path.Combine(path, fileName);

            // 1. Check if the file even exists before trying to read it
            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _prList = JsonSerializer.Deserialize<List<PR>>(rawData) ?? new List<PR>();
                }
                catch (Exception)
                {
                    // If the JSON is corrupted, fall back to a new list
                    _prList = new List<PR>();
                }
            }
            else
            {
                // 2. Initialize with default data if it's the first time running
                _prList = new List<PR>
                {
                    new PR { Id = 0, Distance = 0.5, BestTime = new TimeSpan(0, 1, 58), DateRan = new DateTime(2022, 3, 3)},
                    new PR { Id = 1, Distance = 1, BestTime = new TimeSpan(0, 4, 36), DateRan = new DateTime(2022, 2, 7)},
                    new PR { Id = 2, Distance = 3.1, BestTime = new TimeSpan(0, 16, 40), DateRan = new DateTime(2021, 11, 4)}
                };

                // Optionally save this default file immediately
                Save();
            }

        }

        private static PrServiceProxy? instance;

        private int nextKey
        {
            get
            {
                if (PRList.Any())
                {
                    return PRList.Select(t => t.Id).Max() + 1;
                }
                return 1;
            }
        }

        public static PrServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new PrServiceProxy();
                }

                return instance;
            }
        }
        public PR? AddOrUpdate(PR? pr)
        {
            if (pr != null && pr.Id == 0)
            {
                pr.Id = nextKey;
                //PRs.Add(pr);
                _prList.Add(pr);

            }
            else if (pr != null)
            {
                var existingPR = _prList.FirstOrDefault(t => t.Id == pr.Id);

                if (existingPR != null)
                {
                    var index = _prList.IndexOf(existingPR);
                    _prList.RemoveAt(index); //remove old pr
                    _prList.Insert(index, pr); //replace with new edited pr
                }


            }

            Save();

            return pr; //was void fn
        }

        public void DisplayPRs()
        {
            PRList.ForEach(Console.WriteLine);

        }

        public PR? GetById(int id)
        {
            return PRList.FirstOrDefault(t => t.Id == id);
        }

        public void DeletePR(int id)
        {

            var localPR = _prList.FirstOrDefault(t => t.Id == id);
            if (localPR != null)
            {
                _prList.Remove(localPR);
            }
            Save(); //doesnt hurt to have extra saving
        }




        public void Save()
        {
            //save
            string path = FileSystem.AppDataDirectory;
            string fileName = $"prs.json";
            string fullPath = Path.Combine(path, fileName);
            var serializedData = JsonSerializer.Serialize(_prList);
            File.WriteAllText(fullPath, serializedData);
        }


    }
}
