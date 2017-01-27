using PracticeTracker.Pages.CategoryPages;
using PracticeTracker.Persistence.Models;
using System;
using Xamarin.Forms;

namespace PracticeTracker.Pages.ExercisePages
{
    public partial class ExerciseCEPage : ContentPage
    {
        private Category _selectedCategory;
        private Exercise _exercise;
        private bool _useOldCategory = true;
        private bool fromCategoriesListPage = false;

        public ExerciseCEPage(Exercise exercise = null)
        {
            InitializeComponent();

            // get exercise from  main page for updating
            _exercise = exercise;

            // set page title
            this.Title = (_exercise == null) ? "Create New Exercise" : "Edit Exercise";
        }

        protected override async void OnAppearing()
        {
            // check if we are going to use the old category
            if (_useOldCategory && _exercise != null)
                _selectedCategory = await App.Database.GetCategoryByIdAsync(_exercise.CategoryId);

            // prepopulate all controls
            if (!fromCategoriesListPage)
            {
                if (_exercise != null)
                {
                    txtName.Text = _exercise.Name;
                    txtGoal.Text = _exercise.Goal.ToString();
                    lblCategory.Text = _selectedCategory.Name;
                }
            }

            base.OnAppearing();
        }

        void lblCategory_Tapped(object sender, EventArgs e)
        {
            var page = new CategoryListPage();

            page.Categories.ItemSelected += (source, args) =>
            {
                _useOldCategory = false;
                _selectedCategory = (Category)args.SelectedItem;
                lblCategory.Text = _selectedCategory.Name;
                fromCategoriesListPage = true;
                Navigation.PopAsync();
            };

            Navigation.PushAsync(page);
        }

        async void btnSave_Clicked(object sender, EventArgs e)
        {
            if (!IsFormValid())
                return;

            // get the new data
            string name = txtName.Text;

            try
            {
                // see if exercise already exists
                var existingExercise = await App.Database.FindExerciseByNameAsync(name);

                // check if updating or adding
                if (_exercise != null)
                {
                    // check for existing category by that name
                    if (existingExercise != null)
                    {
                        // if we are updating the same category we are looking at's name
                        if (_exercise.Id != existingExercise.Id)
                        {
                            ShowError(name);
                            return;
                        }
                    }

                    // updating
                    _exercise.Name = name;
                    _exercise.Goal = Convert.ToInt32(txtGoal.Text);
                    _exercise.CategoryId = _selectedCategory.Id;

                    // if error it wont get this far
                    await App.Database.UpdateAsync(_exercise);
                }
                else
                {
                    if (existingExercise != null)
                    {
                        ShowError(name);
                        return;
                    }

                    // adding
                    _exercise = new Exercise { Name = name, CategoryId = _selectedCategory.Id, Goal = Convert.ToInt32(txtGoal.Text) };

                    // insert to db
                    await App.Database.InsertAsync(_exercise);
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Oops! Something went wrong. Try again later.", "OK");
            }
        }

        void ShowError(string name)
        {
            // show the label
            lblResult.IsVisible = true;
            lblResult.TextColor = Color.Red;
            lblResult.Text = "The Exercise named " + name + " already exists.";
        }

        void btnReset_Clicked(object sender, EventArgs e)
        {
            // name
            txtName.Text = "";
            lblNameError.IsVisible = false;

            // name
            txtGoal.Text = "";
            lblGoalError.IsVisible = false;

            // category
            _selectedCategory = null;
            lblCategory.Text = "Choose a Category...";
            lblCategoryError.IsVisible = false;

            lblResult.IsVisible = false;
        }

        public bool IsFormValid()
        {
            int errorCount = 0;
            if (txtName.Text == null || txtName.Text == "")
            {
                lblNameError.IsVisible = true;
                errorCount++;
            }
            else
                lblNameError.IsVisible = false;

            if (_selectedCategory == null)
            {
                lblCategoryError.IsVisible = true;
                errorCount++;
            }
            else
                lblCategoryError.IsVisible = false;

            if (txtGoal.Text == null | txtGoal.Text == "")
            {
                lblGoalError.IsVisible = true;
                errorCount++;
            }
            else
                lblGoalError.IsVisible = false;

            if (errorCount != 0)
                return false;

            return true;
        }
    }
}
