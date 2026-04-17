using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RunningBuddy.Models;
using RunningBuddy.Services;

namespace RunningBuddy.ViewModels
{
    public class TrainingPlanViewModel : INotifyPropertyChanged
    {
        private string _selectedPlanType = "Easy/Base Runs";
        private DateTime _selectedStartDate = DateTime.Today;
        private string _planDescription = "";
        private string _planDuration = "";
        private string _activePlanInfo = "";
        private bool _hasActivePlan;

        public TrainingPlanViewModel()
        {
            PlanTypes = new ObservableCollection<string>
            {
                "5K",
                "10K",
                "Half Marathon",
                "Marathon"
            };

            SelectedPlanType = "5K";
            UpdatePlanPreview();
            LoadActivePlan();
        }

        public ObservableCollection<string> PlanTypes { get; }

        public string SelectedPlanType
        {
            get => _selectedPlanType;
            set
            {
                if (_selectedPlanType != value)
                {
                    _selectedPlanType = value;
                    OnPropertyChanged();
                    UpdatePlanPreview();
                }
            }
        }

        public DateTime SelectedStartDate
        {
            get => _selectedStartDate;
            set
            {
                if (_selectedStartDate != value)
                {
                    _selectedStartDate = value;
                    OnPropertyChanged();
                    UpdatePlanPreview();
                }
            }
        }

        public DateTime MinimumDate => DateTime.Today;

        public string PlanDescription
        {
            get => _planDescription;
            set
            {
                if (_planDescription != value)
                {
                    _planDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PlanDuration
        {
            get => _planDuration;
            set
            {
                if (_planDuration != value)
                {
                    _planDuration = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ActivePlanInfo
        {
            get => _activePlanInfo;
            set
            {
                if (_activePlanInfo != value)
                {
                    _activePlanInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasActivePlan
        {
            get => _hasActivePlan;
            set
            {
                if (_hasActivePlan != value)
                {
                    _hasActivePlan = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdatePlanPreview()
        {
            PlanDescription = TrainingPlanService.GetDescriptionForPlanType(_selectedPlanType);
            int weeks = TrainingPlanService.GetDurationForPlanType(_selectedPlanType);
            DateTime endDate = _selectedStartDate.AddDays(weeks * 7);
            PlanDuration = $"{weeks} weeks • {_selectedStartDate:MMM d} → {endDate:MMM d, yyyy}";
        }

        private void LoadActivePlan()
        {
            var activePlan = TrainingPlanService.Current.GetActivePlan();
            if (activePlan != null)
            {
                int currentWeek = TrainingPlanService.Current.GetCurrentWeek(activePlan);
                HasActivePlan = true;
                ActivePlanInfo = $"Active: {activePlan.PlanType} — Week {currentWeek} of {activePlan.DurationWeeks}";
            }
            else
            {
                HasActivePlan = false;
                ActivePlanInfo = "";
            }
        }

        /// <summary>
        /// Creates the plan and returns it
        /// </summary>
        public TrainingPlan CreatePlan()
        {
            var plan = TrainingPlanService.Current.GeneratePlan(_selectedPlanType, _selectedStartDate);
            LoadActivePlan();
            return plan;
        }

        public void Refresh()
        {
            LoadActivePlan();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
