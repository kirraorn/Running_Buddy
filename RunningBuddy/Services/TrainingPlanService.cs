using Microsoft.Maui.Storage;
using RunningBuddy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RunningBuddy.Services
{
    public class TrainingPlanService
    {
        private List<TrainingPlan> _plans;

        public List<TrainingPlan> PlanList
        {
            get => _plans?.ToList() ?? new List<TrainingPlan>();
            private set => _plans = value;
        }

        private TrainingPlanService()
        {
            string path = FileSystem.AppDataDirectory;
            string fullPath = Path.Combine(path, "training_plans.json");

            if (File.Exists(fullPath))
            {
                try
                {
                    var rawData = File.ReadAllText(fullPath);
                    _plans = JsonSerializer.Deserialize<List<TrainingPlan>>(rawData) ?? new List<TrainingPlan>();
                }
                catch (Exception)
                {
                    _plans = new List<TrainingPlan>();
                }
            }
            else
            {
                _plans = new List<TrainingPlan>();
                Save();
            }
        }

        private static TrainingPlanService? instance;

        public static TrainingPlanService Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new TrainingPlanService();
                }
                return instance;
            }
        }

        private int NextKey
        {
            get
            {
                if (_plans.Any())
                    return _plans.Select(p => p.Id).Max() + 1;
                return 1;
            }
        }

        /// <summary>
        /// Returns the currently active training plan, or null if none.
        /// </summary>
        public TrainingPlan? GetActivePlan()
        {
            return _plans.FirstOrDefault(p => p.IsActive);
        }

        /// <summary>
        /// Gets the duration in weeks for a given plan type.
        /// </summary>
        public static int GetDurationForPlanType(string planType)
        {
            return planType switch
            {
                "5K" => 8,
                "10K" => 10,
                "Half Marathon" => 12,
                "Marathon" => 16,
                _ => 8
            };
        }

        /// <summary>
        /// Gets a description for a given plan type.
        /// </summary>
        public static string GetDescriptionForPlanType(string planType)
        {
            return planType switch
            {
                "5K" => "Build up to racing 3.1 miles. Perfect for beginners or runners looking to set a new PR.",
                "10K" => "Train for 6.2 miles with a mix of easy runs, tempo work, and long runs.",
                "Half Marathon" => "Prepare for 13.1 miles with progressive long runs and speed sessions.",
                "Marathon" => "The full 26.2 mile journey. A structured 16-week plan building endurance and race readiness.",
                _ => ""
            };
        }

        /// <summary>
        /// Generates a new training plan, creating workouts on the calendar.
        /// Deletes any existing active plan and its workouts before creating the new one.
        /// </summary>
        public TrainingPlan GeneratePlan(string planType, DateTime startDate)
        {
            // Delete any existing active plan and its workouts
            var activePlan = GetActivePlan();
            if (activePlan != null)
            {
                DeletePlanAndWorkouts(activePlan);
            }

            int durationWeeks = GetDurationForPlanType(planType);

            var plan = new TrainingPlan
            {
                Id = NextKey,
                PlanType = planType,
                StartDate = startDate,
                DurationWeeks = durationWeeks,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _plans.Add(plan);
            Save();

            // Generate workouts for each week
            GenerateWorkouts(plan);

            // Create a goal tied to this plan
            var goal = new Goal
            {
                PlanType = planType,
                TrainingPlanId = plan.Id,
                Date = startDate.AddDays(durationWeeks * 7),
                Distance = GetGoalDistanceForPlan(planType)
            };
            GoalServiceProxy.Current.AddOrUpdate(goal);

            return plan;
        }

        /// <summary>
        /// Generates workouts for each training day in the plan.
        /// </summary>
        private void GenerateWorkouts(TrainingPlan plan)
        {
            var template = GetWeeklyTemplate(plan.PlanType);
            var workoutSvc = WorkoutServiceProxy.Current;

            for (int week = 0; week < plan.DurationWeeks; week++)
            {
                foreach (var day in template)
                {
                    if (day.IsRest) continue;

                    DateTime workoutDate = plan.StartDate.AddDays(week * 7 + day.DayOfWeek);

                    // Scale distance based on week progression
                    double progressFactor = 1.0 + (week * 0.12); // ~12% increase per week
                    double distance = Math.Round(day.BaseDistance * progressFactor, 1);

                    string workoutName = $"{plan.PlanType} — Wk {week + 1}: {day.WorkoutName}";

                    var workout = new Workout
                    {
                        RouteId = 0,
                        RouteName = workoutName,
                        Distance = distance,
                        Date = workoutDate,
                        Notes = $"Training Plan: {plan.PlanType} | Week {week + 1} of {plan.DurationWeeks}"
                    };

                    workoutSvc.AddOrUpdate(workout);
                }
            }
        }

        /// <summary>
        /// Returns the race distance for the plan type.
        /// </summary>
        private double GetGoalDistanceForPlan(string planType)
        {
            return planType switch
            {
                "5K" => 3.1,
                "10K" => 6.2,
                "Half Marathon" => 13.1,
                "Marathon" => 26.2,
                _ => 3.1
            };
        }

        /// <summary>
        /// Returns the weekly training template for a given plan type.
        /// Each entry defines a day of the week (0=Sun, 1=Mon, etc.), workout type, and base distance.
        /// </summary>
        private List<TrainingDayTemplate> GetWeeklyTemplate(string planType)
        {
            return planType switch
            {
                "5K" => new List<TrainingDayTemplate>
                {
                    new() { DayOfWeek = 0, IsRest = true },                                       // Sun - Rest
                    new() { DayOfWeek = 1, WorkoutName = "Easy Run", BaseDistance = 1.5 },         // Mon
                    new() { DayOfWeek = 2, IsRest = true },                                       // Tue - Rest
                    new() { DayOfWeek = 3, WorkoutName = "Tempo Run", BaseDistance = 1.5 },        // Wed
                    new() { DayOfWeek = 4, IsRest = true },                                       // Thu - Rest
                    new() { DayOfWeek = 5, WorkoutName = "Long Run", BaseDistance = 2.0 },         // Fri
                    new() { DayOfWeek = 6, WorkoutName = "Recovery Run", BaseDistance = 1.0 },     // Sat
                },

                "10K" => new List<TrainingDayTemplate>
                {
                    new() { DayOfWeek = 0, IsRest = true },                                       // Sun - Rest
                    new() { DayOfWeek = 1, WorkoutName = "Easy Run", BaseDistance = 2.5 },         // Mon
                    new() { DayOfWeek = 2, WorkoutName = "Intervals", BaseDistance = 2.0 },        // Tue
                    new() { DayOfWeek = 3, IsRest = true },                                       // Wed - Rest
                    new() { DayOfWeek = 4, WorkoutName = "Tempo Run", BaseDistance = 2.5 },        // Thu
                    new() { DayOfWeek = 5, IsRest = true },                                       // Fri - Rest
                    new() { DayOfWeek = 6, WorkoutName = "Long Run", BaseDistance = 4.0 },         // Sat
                },

                "Half Marathon" => new List<TrainingDayTemplate>
                {
                    new() { DayOfWeek = 0, IsRest = true },                                       // Sun - Rest
                    new() { DayOfWeek = 1, WorkoutName = "Easy Run", BaseDistance = 3.0 },         // Mon
                    new() { DayOfWeek = 2, WorkoutName = "Speed Work", BaseDistance = 3.0 },       // Tue
                    new() { DayOfWeek = 3, WorkoutName = "Recovery Run", BaseDistance = 2.0 },     // Wed
                    new() { DayOfWeek = 4, WorkoutName = "Tempo Run", BaseDistance = 4.0 },        // Thu
                    new() { DayOfWeek = 5, IsRest = true },                                       // Fri - Rest
                    new() { DayOfWeek = 6, WorkoutName = "Long Run", BaseDistance = 6.0 },         // Sat
                },

                "Marathon" => new List<TrainingDayTemplate>
                {
                    new() { DayOfWeek = 0, IsRest = true },                                       // Sun - Rest
                    new() { DayOfWeek = 1, WorkoutName = "Easy Run", BaseDistance = 4.0 },         // Mon
                    new() { DayOfWeek = 2, WorkoutName = "Speed Work", BaseDistance = 4.0 },       // Tue
                    new() { DayOfWeek = 3, WorkoutName = "Recovery Run", BaseDistance = 3.0 },     // Wed
                    new() { DayOfWeek = 4, WorkoutName = "Tempo Run", BaseDistance = 5.0 },        // Thu
                    new() { DayOfWeek = 5, IsRest = true },                                       // Fri - Rest
                    new() { DayOfWeek = 6, WorkoutName = "Long Run", BaseDistance = 10.0 },        // Sat
                },

                _ => new List<TrainingDayTemplate>()
            };
        }

        /// <summary>
        /// Deletes a plan and all its associated workouts from the calendar.
        /// </summary>
        private void DeletePlanAndWorkouts(TrainingPlan plan)
        {
            // Delete all workouts that belong to this plan
            var workoutSvc = WorkoutServiceProxy.Current;
            string planMarker = $"Training Plan: {plan.PlanType}";

            var workoutsToDelete = workoutSvc.WorkoutList?
                .Where(w => w.Notes != null && w.Notes.Contains(planMarker))
                .ToList() ?? new List<Workout>();

            foreach (var workout in workoutsToDelete)
            {
                workoutSvc.DeleteWorkout(workout.Id);
            }

            // Delete goals tied to this plan
            var goals = GoalServiceProxy.Current.GoalList?
                .Where(g => g.TrainingPlanId == plan.Id)
                .ToList() ?? new List<Goal>();

            foreach (var goal in goals)
            {
                GoalServiceProxy.Current.DeleteGoal(goal.Id);
            }

            // Remove the plan itself
            _plans.Remove(plan);
            Save();
        }

        /// <summary>
        /// Deletes a plan by ID (public API).
        /// </summary>
        public void DeletePlan(int id)
        {
            var plan = _plans.FirstOrDefault(p => p.Id == id);
            if (plan != null)
            {
                DeletePlanAndWorkouts(plan);
            }
        }

        /// <summary>
        /// Gets the current week number for the active plan.
        /// </summary>
        public int GetCurrentWeek(TrainingPlan plan)
        {
            if (plan == null) return 0;
            int daysSinceStart = (DateTime.Today - plan.StartDate.Date).Days;
            if (daysSinceStart < 0) return 0; // Plan hasn't started yet
            int week = (daysSinceStart / 7) + 1;
            return Math.Min(week, plan.DurationWeeks);
        }

        private void Save()
        {
            try
            {
                string path = FileSystem.AppDataDirectory;
                string fullPath = Path.Combine(path, "training_plans.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(_plans, options);
                File.WriteAllText(fullPath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving training plans: {ex.Message}");
            }
        }

        /// <summary>
        /// Internal template class for defining weekly workout structure.
        /// </summary>
        private class TrainingDayTemplate
        {
            public int DayOfWeek { get; set; }
            public bool IsRest { get; set; }
            public string WorkoutName { get; set; } = "";
            public double BaseDistance { get; set; }
        }
    }
}
