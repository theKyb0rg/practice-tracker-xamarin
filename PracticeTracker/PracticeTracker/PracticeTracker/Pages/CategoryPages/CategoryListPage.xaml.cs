using PracticeTracker.Persistence;
using PracticeTracker.Persistence.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PracticeTracker.Pages.CategoryPages
{
    public partial class CategoryListPage : ContentPage
    {
        private ObservableCollection<Category> _categories;
        public ListView Categories { get { return lstCategories; } }

        public CategoryListPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            // get the list of exercises
            var categories = await App.Database.GetCategoriesOrderByNameAsync();

            if (categories.Count == 0)
            {
                await DisplayAlert("Information", "No categories found.", "OK");
                await Navigation.PopAsync();
                return;
            }

            // add to observable collection
            // use in listview
            _categories = new ObservableCollection<Category>(categories);

            // set item source for list view
            lstCategories.ItemsSource = _categories;

            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            lstCategories.IsVisible = true;

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
                    lstCategories.ItemsSource = new ObservableCollection<Category>()
                    {
                        new Category { Name = "No results found." }
                    };
                    return;
                }
                lstCategories.ItemsSource = results;
            }
        }
    }
}
