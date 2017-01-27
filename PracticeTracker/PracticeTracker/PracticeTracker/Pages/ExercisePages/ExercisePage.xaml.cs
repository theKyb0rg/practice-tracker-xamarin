using PracticeTracker.Pages.StatisticPages;
using PracticeTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace PracticeTracker.Pages.ExercisePages
{
    public partial class ExercisePage : ContentPage
    {
        private ObservableCollection<ExerciseViewModel> _exercises;

        public ExercisePage()
        {
            InitializeComponent();
        }

        async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExerciseCEPage());
        }

        async void btnEdit_Clicked(object sender, EventArgs e)
        {
            int exerciseId = (int)(sender as Button).CommandParameter;

            var exercise = await App.Database.GetExerciseByIdAsync(exerciseId);

            if (exercise != null)
                await Navigation.PushAsync(new ExerciseCEPage(exercise));
        }

        protected override async void OnAppearing()
        {
            // get the list of exercises
            var exercises = await App.Database.GetExercisesOrderByNameAsync();

            // make list of view model so we can get the exercise name
            var exercisesVM = new List<ExerciseViewModel>();

            // loop through all practices and get exercisename for each practice item
            foreach (var item in exercises)
            {
                // get the exercise for this practice session
                var category = await App.Database.GetCategoryByIdAsync(item.CategoryId);

                // get the max tempo for this exercise
                var practice = await App.Database.GetPracticeWithMaxTempoByExerciseIdAsync(item.Id);

                // add to observable collection
                exercisesVM.Add(new ExerciseViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    CategoryName = category.Name,
                    Goal = item.Goal,
                    //MaxTempoMessage = "Max Tempo: " + practice.Tempo + " bpm",
                    //GoalTempoMessage = "Goal Tempo: " + item.Goal + " bpm"
                });
            }

            // add to observable collection
            // use in listview
            _exercises = new ObservableCollection<ExerciseViewModel>(exercisesVM);

            // set item source for list view
            lstExercises.ItemsSource = _exercises;

            if (_exercises.Count == 0)
                SwapVisibility(false);
            else
                SwapVisibility(true);

            // terminatee the loading indicator
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;

            base.OnAppearing();
        }

        async void btnDelete_Clicked(object sender, EventArgs e)
        {
            var exerciseId = (int)(sender as Button).CommandParameter;
            var exerciseVM = _exercises.Where(s => s.Id == exerciseId).SingleOrDefault();

            if (exerciseVM == null)
                return;

            var decision = await DisplayAlert("Delete Exercise", "Are you sure you want to delete " + exerciseVM.Name + "?", "Yes", "No");

            if (decision)
            {
                // find exercise object from vm
                var exercise = await App.Database.GetExerciseByIdAsync(exerciseVM.Id);

                // get related practices
                var practices = await App.Database.GetPracticesByExerciseIdAsync(exerciseId);

                if (practices.Count > 0)
                {
                    // prompt user all related data will be deleted
                    var deleteAllRelated = await DisplayAlert("Delete Exercise", "If you delete the exercise named " + exerciseVM.Name + ", all related Practice Sessions will be deleted with it, are you sure you want to proceed?", "Yes", "No");

                    if (deleteAllRelated)
                    {
                        foreach (var practice in practices)
                        {
                            // delete this practice session
                            await App.Database.DeleteAsync(practice);
                        }

                        // delete this exercise
                        await App.Database.DeleteAsync(exercise);

                        // remove from gui
                        _exercises.Remove(exerciseVM);
                    }
                }
                else
                {
                    // delete this exercise
                    await App.Database.DeleteAsync(exercise);

                    // remove from gui
                    _exercises.Remove(exerciseVM);
                }
            }

            if (_exercises.Count == 0)
                SwapVisibility(false);
        }

        void SwapVisibility(bool isVisible)
        {
            slNoExercises.IsVisible = !isVisible;
            lstExercises.IsVisible = isVisible;
        }

        void lstExercises_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // don't do anything if we just de-selected the row
            if (e.Item == null) return;
            // do something with e.SelectedItem
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterSearch(e.NewTextValue);
        }

        void btnSearchToggle_Clicked(object sender, EventArgs e)
        {
            bool isVisible = txtSearch.IsVisible;
            txtSearch.IsVisible = !isVisible;
        }

        void FilterSearch(string searchText = null)
        {
            // check for null or white space
            if (string.IsNullOrWhiteSpace(searchText))
                lstExercises.ItemsSource = _exercises;

            if (_exercises != null)
            {
                // filter by category and exercise name
                var results = _exercises.Where(c => c.Name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase) || c.CategoryName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase));

                if (results.Count() == 0)
                {
                    SwapVisibility(false);
                    if (_exercises.Count > 0)
                        btnAdd.IsVisible = false;
                    return;
                }

                SwapVisibility(true);
                btnAdd.IsVisible = true;
                lstExercises.ItemsSource = results;
            }
        }

        async void btnStats_Clicked(object sender, EventArgs e)
        {
            var exercise = (ExerciseViewModel)(sender as Button).CommandParameter;
            await Navigation.PushAsync(new StatisticDetailsPage(exercise));
        }
    }
}
