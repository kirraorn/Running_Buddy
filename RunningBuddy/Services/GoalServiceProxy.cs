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
    internal class GoalServiceProxy
    {
        //public List<Goal> goals;

        private List<Goal> _goalList;
        //private List<Goal> _dummyList;
        public List<Goal> GoalList
        {
            get
            {
                if (_goalList != null)
                {
                    return _goalList.ToList();
                }
                else
                {
                    return null;
                }
            }

            private set
            {
                if (value != _goalList)
                {
                    _goalList = value;
                }
            }
        }



        private GoalServiceProxy()
        {
            string path = FileSystem.AppDataDirectory;
            string fileName = "goals.json";
            string fullPath = Path.Combine(path, fileName);

            // 1. Check if the file even exists before trying to read it
            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _goalList = JsonSerializer.Deserialize<List<Goal>>(rawData) ?? new List<Goal>();
                }
                catch (Exception)
                {
                    // If the JSON is corrupted, fall back to a new list
                    _goalList = new List<Goal>();
                }
            }
            else
            {
                // 2. Initialize with default data if it's the first time running
                _goalList = new List<Goal>
                {
                    new Goal { Id = 0, Distance = 1, Time = new TimeSpan(0, 4, 37), Date = new DateTime(2027, 1, 1)},
                    new Goal { Id = 1, Distance = 3.1, Time = new TimeSpan(0, 16, 40)},
                    new Goal { Id = 2, Distance = 0.5, Time = new TimeSpan(0, 1, 59)}
                };

                // Optionally save this default file immediately
                Save();
            }

        }

        private static GoalServiceProxy? instance;

        private int nextKey
        {
            get
            {
                if (GoalList.Any())
                {
                    return GoalList.Select(t => t.Id).Max() + 1;
                }
                return 1;
            }
        }

        public static GoalServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new GoalServiceProxy();
                }

                return instance;
            }
        }
        public Goal? AddOrUpdate(Goal? goal)
        {
            if (goal != null && goal.Id == 0)
            {
                goal.Id = nextKey;
                //Goals.Add(goal);
                _goalList.Add(goal);

            }
            else if (goal != null)
            {
                var existingGoal = _goalList.FirstOrDefault(t => t.Id == goal.Id);

                if (existingGoal != null)
                {
                    var index = _goalList.IndexOf(existingGoal);
                    _goalList.RemoveAt(index); //remove old goal
                    _goalList.Insert(index, goal); //replace with new edited goal
                }


            }

            Save();

            return goal; //was void fn
        }

        public void DisplayGoals()
        {
            GoalList.ForEach(Console.WriteLine);

        }

        public Goal? GetById(int id)
        {
            return GoalList.FirstOrDefault(t => t.Id == id);
        }

        public void DeleteGoal(int id)
        {

            var localGoal = _goalList.FirstOrDefault(t => t.Id == id);
            if (localGoal != null)
            {
                _goalList.Remove(localGoal);
            }
            Save(); //doesnt hurt to have extra saving
        }




        public void Save()
        {
            //save
            string path = FileSystem.AppDataDirectory;
            string fileName = $"goals.json";
            string fullPath = Path.Combine(path, fileName);
            var serializedData = JsonSerializer.Serialize(_goalList);
            File.WriteAllText(fullPath, serializedData);
        }


    }
}
