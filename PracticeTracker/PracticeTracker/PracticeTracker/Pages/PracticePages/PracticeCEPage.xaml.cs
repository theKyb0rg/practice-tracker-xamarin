using PracticeTracker.Pages.ExercisePages;
using PracticeTracker.Persistence.Models;
using System;
using Xamarin.Forms;
using PracticeTracker.ViewModels;

namespace PracticeTracker.Pages.PracticePages
{
    public partial class PracticeCEPage : ContentPage
    {
        private ExerciseViewModel _selectedExercise;
        private Practice _practice;
        private bool useOldExercise = true;
        private bool fromOtherPage = false;
        //private const string _emptyCommentText = "None (optional)";
        //private string _commentTextToSave = "";

        public PracticeCEPage(Practice practice = null)
        {
            InitializeComponent();

            // set practice to edit
            _practice = practice;

            // set page title
            this.Title = (_practice == null) ? "Create New Practice Session" : "Edit Practice Session";
        }

        protected override async void OnAppearing()
        {
            // check if we are going to use the old category
            if (useOldExercise && _practice != null)
            {
                var exercise = await App.Database.GetExerciseByIdAsync(_practice.ExerciseId);
                _selectedExercise = new ExerciseViewModel()
                {
                    Name = exercise.Name,
                    Id = exercise.Id
                };
            }

            // prepopulate all controls
            if (!fromOtherPage)
            {
                if (_practice != null)
                {
                    lblExercise.Text = _selectedExercise.Name;
                    txtDate.MaximumDate = DateTime.Today;
                    //_commentTextToSave = (_practice.Comment == null) ? "" : _practice.Comment;
                    //lblComments.Text = string.IsNullOrWhiteSpace(_commentTextToSave) ? _emptyCommentText : _commentTextToSave;
                    txtComments.Text = _practice.Comment;
                    txtDate.Date = _practice.Date;
                    txtTempo.Text = _practice.Tempo.ToString();
                    txtTime.Text = _practice.Time.ToString();
                }
            }

            base.OnAppearing();
        }

        void lblExercise_Tapped(object sender, EventArgs e)
        {
            var page = new ExerciseListPage();

            page.Exercises.ItemSelected += (source, args) =>
            {
                useOldExercise = false;
                _selectedExercise = (ExerciseViewModel)args.SelectedItem;
                lblExercise.Text = _selectedExercise.Name;
                fromOtherPage = true;
                Navigation.PopAsync();
            };

            Navigation.PushAsync(page);
        }

        async void btnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!IsFormValid())
                    return;

                // create new exercise
                DateTime date = txtDate.Date;
                int tempo = Convert.ToInt32(txtTempo.Text);
                int time = Convert.ToInt32(txtTime.Text);
                int exerciseId = _selectedExercise.Id;
                //string comments = _commentTextToSave;                
                string comments = txtComments.Text;

                if (_practice != null)
                {
                    // create new exercise
                    _practice.Date = date;
                    _practice.Tempo = tempo;
                    _practice.Time = time;
                    _practice.ExerciseId = exerciseId;
                    _practice.Comment = comments;
                    await App.Database.UpdateAsync(_practice);
                }
                else
                {
                    _practice = new Practice { Date = txtDate.Date, Tempo = Convert.ToInt32(txtTempo.Text), Time = Convert.ToInt32(txtTime.Text), Comment = comments/*_commentTextToSave*/, ExerciseId = _selectedExercise.Id };
                    await App.Database.InsertAsync(_practice);
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
            // tempo
            txtTempo.Text = "";
            lblTempoError.IsVisible = false;

            // time
            txtTime.Text = "";
            lblTimeError.IsVisible = false;

            // comments
            txtComments.Text = "";

            // category
            _selectedExercise = null;
            lblExercise.Text = "Choose an Exercise...";
            lblExerciseError.IsVisible = false;

            lblResult.IsVisible = false;
        }

        //async void lblComment_Tapped(object sender, EventArgs e)
        //{
        //    // pass text to comments page that contains editor
        //    var page = new PracticeCommentsPage(_commentTextToSave);

        //    page.Comments.Completed += (source, args) =>
        //    {
        //        // get the editor from the comment page
        //        var editor = (Editor)source;

        //        // get the text from the editor
        //        _commentTextToSave = editor.Text;

        //        // format nicely for the gui
        //        string displaytext = (_commentTextToSave.Length > 75) ? editor.Text.Substring(0, 72) + "..." : _commentTextToSave;

        //        // check for empty string
        //        lblComments.Text = string.IsNullOrWhiteSpace(displaytext) ? _emptyCommentText : displaytext;

        //        // set to true so it doesnt refresh the values of everything
        //        fromOtherPage = true;
        //    };

        //    await Navigation.PushAsync(page);
        //}

        public bool IsFormValid()
        {
            int errorCount = 0;
            if (string.IsNullOrWhiteSpace(txtTempo.Text))
            {
                lblTempoError.Text = "Tempo is required.";
                lblTempoError.IsVisible = true;
                errorCount++;
            }
            else
            {
                int tempo;
                bool isNumber = int.TryParse(txtTempo.Text, out tempo);
                if (isNumber)
                {
                    if (tempo <= 0)
                    {
                        lblTempoError.Text = "Tempo must be greater than 0.";
                        lblTempoError.IsVisible = true;
                        errorCount++;
                    }
                    else
                        lblTempoError.IsVisible = false;
                }
                else
                {
                    lblTempoError.Text = "Invalid input. Tempo must be a number.";
                    lblTempoError.IsVisible = true;
                    errorCount++;
                }
            }

            if (string.IsNullOrWhiteSpace(txtTime.Text))
            {
                lblTimeError.IsVisible = true;
                errorCount++;
            }
            else
            {
                int time;
                bool isNumber = int.TryParse(txtTime.Text, out time);
                if (isNumber)
                {
                    if (time <= 0)
                    {
                        lblTimeError.Text = "Time must be greater than 0.";
                        lblTimeError.IsVisible = true;
                        errorCount++;
                    }
                    else
                        lblTimeError.IsVisible = false;
                }
                else
                {
                    lblTimeError.Text = "Invalid input. Time must be a number.";
                    lblTimeError.IsVisible = true;
                    errorCount++;
                }
            }

            if (_selectedExercise == null)
            {
                lblExerciseError.IsVisible = true;
                errorCount++;
            }
            else
                lblExerciseError.IsVisible = false;

            if (errorCount != 0)
                return false;

            return true;
        }
    }
}
