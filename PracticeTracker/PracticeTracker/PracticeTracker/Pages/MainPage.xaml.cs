using PracticeTracker.Pages.CategoryPages;
using PracticeTracker.Pages.ExercisePages;
using PracticeTracker.Pages.PracticePages;
using PracticeTracker.Pages.StatisticPages;
using PracticeTracker.Pages.ToolPages;
using PracticeTracker.Pages.TutorialPages;
using PracticeTracker.Persistence;
using PracticeTracker.Persistence.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using PracticeTracker.Helpers;

namespace PracticeTracker.Pages
{
    public partial class MainPage : ContentPage
    {
        private bool _firstTimeUser;

        private const string AddCategoryButtonText = "Category";
        private const string AddExerciseButtonText = "Exercise";
        private const string AddPracticeButtonText = "Practice Session";

        public MainPage(bool firstTimeUser)
        {
            InitializeComponent();
            _firstTimeUser = firstTimeUser;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_firstTimeUser)
            {
                var result = await DisplayAlert("Welcome", "It looks like this is your first time using Musician's Practice Tracker.\n\nRead the tutorial?", "Yes, Show Me", "No, I got this");

                if (result)
                    await Navigation.PushAsync(new TutorialPage());
                else
                {
                    Settings.FirstTimeUser = false;
                    await DisplayAlert("No Problem", "You can view the Tutorial any time in the Help section.", "OK");
                }
            }
        }

        protected override void OnDisappearing()
        {
            _firstTimeUser = false;
            base.OnDisappearing();
        }

        

        async void btnAdd_Clicked(object sender, EventArgs e)
        {
            string[] buttons = new string[] { AddCategoryButtonText, AddExerciseButtonText, AddPracticeButtonText };
            string result = await DisplayActionSheet("Create New", "Cancel", null, buttons);
            switch (result)
            {
                case AddCategoryButtonText:
                    await Navigation.PushAsync(new CategoryCEPage());
                    break;
                case AddExerciseButtonText:
                    await Navigation.PushAsync(new ExerciseCEPage());
                    break;
                case AddPracticeButtonText:
                    await Navigation.PushAsync(new PracticeCEPage());
                    break;
            }
        }

        async void Help_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TutorialPage());
        }

        async void Categories_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CategoryPage());
        }

        async void Exercises_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExercisePage());
        }

        async void Practice_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PracticePage());
        }

        async void Statistic_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StatisticPage());
        }

        async void Tools_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToolPage());
        }
    }
}
