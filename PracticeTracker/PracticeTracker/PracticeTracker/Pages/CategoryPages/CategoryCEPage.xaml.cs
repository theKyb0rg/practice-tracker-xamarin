using PracticeTracker.Persistence.Models;
using System;
using Xamarin.Forms;

namespace PracticeTracker.Pages.CategoryPages
{
    public partial class CategoryCEPage : ContentPage
    {
        private Category _category;

        public CategoryCEPage(Category category = null)
        {
            InitializeComponent();

            // category to edit
            _category = category;

            // set page title
            this.Title = (_category == null) ? "Create New Category" : "Edit Category";
        }

        protected override void OnAppearing()
        {
            if (_category != null)
                txtName.Text = _category.Name;

            base.OnAppearing();
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
                var existingCategory = await App.Database.FindCategoryByNameAsync(name);

                // check if updating or adding
                if (_category != null)
                {
                    // check for existing category by that name
                    if (existingCategory != null)
                    {
                        // if we are updating the same category we are looking at's name
                        if (_category.Id != existingCategory.Id)
                        {
                            ShowError(name);
                            return;
                        }
                    }

                    // updating
                    _category.Name = name;

                    // if error it wont get this far
                    await App.Database.UpdateAsync(_category);
                }
                else
                {
                    if (existingCategory != null)
                    {
                        ShowError(name);
                        return;
                    }

                    // adding
                    _category = new Category { Name = name };

                    // insert to db
                    await App.Database.InsertAsync(_category);
                }

                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Oops! Something went wrong. Try again later.", "OK");
            }
        }

        void btnReset_Clicked(object sender, EventArgs e)
        {
            // name
            txtName.Text = "";
            lblNameError.IsVisible = false;

            lblResult.IsVisible = false;
        }

        void ShowError(string name)
        {
            // show the label
            lblResult.IsVisible = true;
            lblResult.TextColor = Color.Red;
            lblResult.Text = "The Category named " + name + " already exists.";
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

            if (errorCount != 0)
                return false;

            return true;
        }
    }
}
