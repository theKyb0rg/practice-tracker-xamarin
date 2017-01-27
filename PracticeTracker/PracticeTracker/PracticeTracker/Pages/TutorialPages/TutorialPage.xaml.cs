using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticeTracker.Helpers;

using Xamarin.Forms;

namespace PracticeTracker.Pages.TutorialPages
{
    public partial class TutorialPage : ContentPage
    {
        public TutorialPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Settings.FirstTimeUser = false;
            base.OnAppearing();
        }

        async void btnExit_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
