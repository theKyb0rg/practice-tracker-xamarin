using Xamarin.Forms;

namespace PracticeTracker.Pages.PracticePages
{
    public partial class PracticeCommentsPage : ContentPage
    {
        public Editor Comments { get { return txtComments; } }
        private string _text;
        public PracticeCommentsPage(string text)
        {
            InitializeComponent();
            _text = text;
        }

        protected override void OnAppearing()
        {
            txtComments.Text = _text;
            txtComments.Focus();
            base.OnAppearing();
        }
    }
}
