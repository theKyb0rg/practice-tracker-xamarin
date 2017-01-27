using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using PracticeTracker.ViewModels;

namespace PracticeTracker.Pages.ExercisePages
{
    public partial class ExerciseListPage : ContentPage
    {
        private ObservableCollection<ExerciseViewModel> _exercises;
        public ListView Exercises { get { return lstExercises; } }

        public ExerciseListPage()
        {
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            if (await App.Database.CountExerciseAsync() == 0)
            {
                await DisplayAlert("Information", "No exercises found.", "OK");
                await Navigation.PopAsync();
                return;
            }

            // add to observable collection
            // use in listview
            _exercises = new ObservableCollection<ExerciseViewModel>(await GetExercises());

            // set item source for list view
            lstExercises.ItemsSource = _exercises;

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            lstExercises.IsVisible = true;

            base.OnAppearing();
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

        async Task<IEnumerable<ExerciseViewModel>> GetExercises()
        {
            // get the list of exercises
            var exercises = await App.Database.GetExercisesOrderByNameAsync();
            List<ExerciseViewModel> exercisesVM = new List<ExerciseViewModel>();
            foreach (var exercise in exercises)
            {
                var category = await App.Database.GetCategoryByIdAsync(exercise.CategoryId);

                exercisesVM.Add(new ExerciseViewModel()
                {
                    Name = exercise.Name,
                    Id = exercise.Id,
                    CategoryName = category.Name
                });
            }
            return exercisesVM;
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
                    lstExercises.ItemsSource = new ObservableCollection<ExerciseViewModel>()
                    {
                        new ExerciseViewModel { Name = "No results found." }
                    };
                    return;
                }
                lstExercises.ItemsSource = results;
            }
        }
    }
}
