using PracticeTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace PracticeTracker.Pages.PracticePages
{
    public partial class PracticePage : ContentPage
    {
        private ObservableCollection<PracticeViewModel> _practices;

        public PracticePage()
        {
            InitializeComponent();

        }

        async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PracticeCEPage());
        }

        async void btnEdit_Clicked(object sender, EventArgs e)
        {
            int practiceId = (int)(sender as Button).CommandParameter;

            var practice = await App.Database.GetPracticeByIdAsync(practiceId);

            if (practice != null)
                await Navigation.PushAsync(new PracticeCEPage(practice));
        }

        protected override async void OnAppearing()
        {
            // get the list of exercises
            var practices = await App.Database.GetPracticesOrderByDateDescendingAsync();

            // make list of view model so we can get the exercise name
            var practicesVM = new List<PracticeViewModel>();

            // loop through all practices and get exercisename for each practice item
            foreach (var item in practices)
            {
                // get the exercise for this practice session
                var exercise = await App.Database.GetExerciseByIdAsync(item.ExerciseId);

                /// truncate name if too long for cell
                string name = (exercise.Name.Length > 200) ? exercise.Name.Substring(0, 13) + "..." : exercise.Name;

                // add to observable collection
                practicesVM.Add(new PracticeViewModel {
                    Id = item.Id,
                    TimeAndTempoDetails = item.Time + " min at " + item.Tempo + " bpm",
                    Date = item.Date,
                    Tempo = item.Tempo,
                    Time = item.Time,
                    ExerciseName = name
                });
            }
            
            // add to observable collection
            // use in listview
            _practices = new ObservableCollection<PracticeViewModel>(practicesVM);

            // set item source for list view
            lstPractices.ItemsSource = _practices;

            if (_practices.Count == 0)
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
            var practiceId = (int)(sender as Button).CommandParameter;
            var practiceVM = _practices.Where(s => s.Id == practiceId).SingleOrDefault();

            if (practiceVM == null)
                return;

            var decision = await DisplayAlert("Delete Practice Item", "Are you sure you want to delete this practice item?", "Yes", "No");

            if (decision)
            {
                // find practice object from vm
                var practice = await App.Database.GetPracticeByIdAsync(practiceVM.Id);

                if(practice != null)
                {
                    // delete form db
                    await App.Database.DeleteAsync(practice);

                    // remove from list in app
                    _practices.Remove(practiceVM);
                }
            }

            if (_practices.Count == 0)
                SwapVisibility(false);
        }

        void SwapVisibility(bool isVisible)
        {
            slNoPractices.IsVisible = !isVisible;
            lstPractices.IsVisible = isVisible;
        }


        void lstPractices_ItemTapped(object sender, ItemTappedEventArgs e)
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
                lstPractices.ItemsSource = _practices;

            if (_practices != null)
            {
                // filter by category and exercise name
                var results = _practices.Where(c => c.ExerciseName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase));

                if (results.Count() == 0)
                {
                    SwapVisibility(false);
                    if (_practices.Count > 0)
                        btnAdd.IsVisible = false;
                    return;
                }

                SwapVisibility(true);
                btnAdd.IsVisible = true;
                lstPractices.ItemsSource = results;
            }
        }

        async void btnComments_Clicked(object sender, EventArgs e)
        {
            int practiceId = (int)(sender as Button).CommandParameter;
            var practice = await App.Database.GetPracticeByIdAsync(practiceId);
            string comments = (practice.Comment == null || string.IsNullOrWhiteSpace(practice.Comment)) ? "No comments recorded." : practice.Comment;
            await DisplayAlert("Comments", comments, "OK");
        }
    }
}
