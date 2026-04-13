using Microsoft.Maui.Storage;
using RunningBuddy.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RunningBuddy.Services
{
    public class WorkoutServiceProxy
    {
        private List<Workout> _workoutList;
        
        public List<Workout> WorkoutList
        {
            get
            {
                if (_workoutList != null)
                {
                    return _workoutList.ToList();
                }
                else
                {
                    return null;
                }
            }

            private set
            {
                if (value != _workoutList)
                {
                    _workoutList = value;
                }
            }
        }

        private WorkoutServiceProxy()
        {
            string path = FileSystem.AppDataDirectory;
            string fileName = "workouts.json";
            string fullPath = Path.Combine(path, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _workoutList = JsonSerializer.Deserialize<List<Workout>>(rawData) ?? new List<Workout>();
                }
                catch (Exception)
                {
                    _workoutList = new List<Workout>();
                }
            }
            else
            {
                _workoutList = new List<Workout>();
                Save();
            }
        }

        private static WorkoutServiceProxy instance;

        private int nextKey
        {
            get
            {
                if (WorkoutList.Any())
                {
                    return WorkoutList.Select(t => t.Id).Max() + 1;
                }
                return 1;
            }
        }

        public static WorkoutServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new WorkoutServiceProxy();
                }

                return instance;
            }
        }

        public Workout AddOrUpdate(Workout workout)
        {
            if (workout != null && workout.Id == 0)
            {
                workout.Id = nextKey;
                _workoutList.Add(workout);
            }
            else if (workout != null)
            {
                var existingWorkout = _workoutList.FirstOrDefault(t => t.Id == workout.Id);

                if (existingWorkout != null)
                {
                    var index = _workoutList.IndexOf(existingWorkout);
                    _workoutList.RemoveAt(index);
                    _workoutList.Insert(index, workout);
                }
            }

            Save();
            return workout;
        }

        public Workout GetById(int id)
        {
            return WorkoutList.FirstOrDefault(t => t.Id == id);
        }

        public List<Workout> GetUpcomingWorkouts()
        {
            return WorkoutList.Where(w => w.Date >= DateTime.Today).OrderBy(w => w.Date).ToList();
        }

        public void DeleteWorkout(int id)
        {
            var localWorkout = _workoutList.FirstOrDefault(t => t.Id == id);
            if (localWorkout != null)
            {
                _workoutList.Remove(localWorkout);
                Save();
            }
        }

        private void Save()
        {
            try
            {
                string path = FileSystem.AppDataDirectory;
                string fileName = "workouts.json";
                string fullPath = Path.Combine(path, fileName);

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(_workoutList, options);
                File.WriteAllText(fullPath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving workouts: {ex.Message}");
            }
        }
    }

    public class Workout
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public DateTime Date { get; set; }
        public string RouteName { get; set; }
        public double Distance { get; set; }
        public string Notes { get; set; }
    }
}
