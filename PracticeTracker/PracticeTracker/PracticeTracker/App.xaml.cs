using PracticeTracker.Pages;
using PracticeTracker.Persistence;
using Xamarin.Forms;
using PracticeTracker.Helpers;

namespace PracticeTracker
{
    public partial class App : Application
    {
        static PracticeTrackerDb database;
        public static PracticeTrackerDb Database
        {
            get
            {
                if (database == null)
                {
                    database = new PracticeTrackerDb(DependencyService.Get<IFileHelper>().GetLocalFilePath("PracticeTracker.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage(Settings.FirstTimeUser))
            {
                BarBackgroundColor = Color.FromHex("4CAF50")
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
