using PracticeTracker.Persistence.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace PracticeTracker.Pages.CategoryPages
{
    public partial class CategoryPage : ContentPage
    {
        private ObservableCollection<Category> _categories;

        public CategoryPage()
        {
            InitializeComponent();
        }

        async void btnAdd_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CategoryCEPage());
        }

        async void btnEdit_Clicked(object sender, EventArgs e)
        {
            int categoryId = (int)(sender as Button).CommandParameter;

            var category = await App.Database.GetCategoryByIdAsync(categoryId);

            if (category != null)
                await Navigation.PushAsync(new CategoryCEPage(category));
        }

        protected override async void OnAppearing()
        {
            try
            {
                // get the list of exercises
                var categories = await App.Database.GetCategoriesOrderByNameAsync();

                // add to observable collection
                // use in listview
                _categories = new ObservableCollection<Category>(categories);

                // set item source for list view
                lstCategories.ItemsSource = _categories;

                if (_categories.Count == 0)
                    SwapVisibility(false);
                else
                    SwapVisibility(true);

                // terminatee the loading indicator
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", "Oops! Something went wrong. Try again later.", "OK");
            }

            base.OnAppearing();
        }

        async void btnDelete_Clicked(object sender, EventArgs e)
        {
            var categoryId = (int)(sender as Button).CommandParameter;
            var category = _categories.Where(s => s.Id == categoryId).SingleOrDefault();

            if (category == null)
                return;

            var decision = await DisplayAlert("Delete Category", "Are you sure you want to delete " + category.Name + "?", "Yes", "No");

            if (decision)
            {
                // get all related exercise data to this category
                var exercises = await App.Database.GetExercisesByCategoryIdAsync(categoryId);

                // if there is related data, prompt user
                if (exercises.Count > 0)
                {
                    // prompt user all related data will be deleted
                    var deleteAllRelated = await DisplayAlert("Delete Category", "If you delete the category named " + category.Name + ", all related Exercises and Practice Sessions will be deleted with it, are you sure you want to proceed?", "Yes", "No");

                    if (deleteAllRelated)
                    {
                        // loop through each exercise to get each practice
                        foreach (var exercise in exercises)
                        {
                            // get all practices related to this exercise
                            var practices = await App.Database.GetPracticesByExerciseIdAsync(exercise.Id);
                            foreach (var practice in practices)
                            {
                                // delete this practice session
                                await App.Database.DeleteAsync(practice);
                            }

                            // delete this exercise
                            await App.Database.DeleteAsync(exercise);
                        }

                        // delete this category
                        await App.Database.DeleteAsync(category);

                        // remove from gui
                        _categories.Remove(category);
                    }
                }
                else
                {
                    // delete this category
                    await App.Database.DeleteAsync(category);

                    // remove from gui
                    _categories.Remove(category);
                }
            }

            if (_categories.Count == 0)
                SwapVisibility(false);
        }

        void SwapVisibility(bool isVisible)
        {
            slNoCategories.IsVisible = !isVisible;
            lstCategories.IsVisible = isVisible;
        }

        void lstCategories_ItemTapped(object sender, ItemTappedEventArgs e)
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
                lstCategories.ItemsSource = _categories;

            if (_categories != null)
            {
                // filter by category and exercise name
                var results = _categories.Where(c => c.Name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase));

                if (results.Count() == 0)
                {
                    SwapVisibility(false);
                    if (_categories.Count > 0)
                        btnAdd.IsVisible = false;
                    return;
                }

                SwapVisibility(true);
                btnAdd.IsVisible = true;
                lstCategories.ItemsSource = results;
            }
        }
    }
}
