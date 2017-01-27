using PracticeTracker.Services;
using PracticeTracker.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PracticeTracker.Pages.ToolPages
{
    public partial class ToolPage : ContentPage
    {
        static IAudioService _audioService;

        // timer variables
        private int _amountOfTime = 0;
        private DateTime _endTime;
        private bool _isPaused = true;
        private int _timeElapsed = 0;
        public static CancellationTokenSource CancellationToken { get; set; }

        // metronome variables
        private TimerHolder _metronome;
        private int _beatCounter = 0;
        private int _metronomeSpeed;

        public ToolPage()
        {
            InitializeComponent();

            _audioService = DependencyService.Get<IAudioService>();
        }

        void ToggleMetronomeButtons(bool isEnabled)
        {
            btnMetronomeStart.IsEnabled = !isEnabled;
            btnMetronomeStop.IsEnabled = isEnabled;
        }

        void ToggleMetronomeTextBox(bool isEnabled)
        {
            txtMetronome.IsEnabled = isEnabled;
        }

        void StopMetronome()
        {
            if (_metronome != null)
                _metronome.Enabled = false;
        }

        void btnMetronome_Start(object sender, EventArgs e)
        {
            int tempo;
            bool isNumber = int.TryParse(txtMetronome.Text, out tempo);

            if (!isNumber && tempo <= 0)
            {
                lblMetronomeError.IsVisible = true;
                return;
            }

            lblMetronomeError.IsVisible = false;

            if (tempo > 250)
                txtMetronome.Text = "250";

            ToggleMetronomeButtons(true);
            ToggleMetronomeTextBox(false);

            // in millieseconds
            _metronomeSpeed = 60000 / tempo;

            // close file if file is open, then open file
            _audioService.CloseFile();
            _audioService.OpenFile("click.mp3");

            // show the text temporarily til we get sound
            _metronome = new TimerHolder(_metronomeSpeed, () =>
            {
                lblMetronome.Text = (_beatCounter++).ToString();
                _audioService.PlayAudioFile();
            });

        }

        void btnMetronome_Stop(object sender, EventArgs e)
        {
            ToggleMetronomeButtons(false);
            ToggleMetronomeTextBox(true);
            StopMetronome();

            // reset the text and beat counter
            _beatCounter = 0;
            lblMetronome.Text = _beatCounter.ToString();
        }



        void txtMetronome_TextChanged(object sender, TextChangedEventArgs e)
        {
            // make sure number again
            int tempo;
            bool isNumber = int.TryParse(e.NewTextValue, out tempo);

            // make sure tempo more than 0 and is in fact a number
            if (isNumber && tempo > 0)
            {
                if (tempo > 250)
                    txtMetronome.Text = "250";
            }
        }

        void ToggleTimerButtons(bool isEnabled)
        {
            btnTimerStart.IsEnabled = !isEnabled;
            btnTimerStop.IsEnabled = isEnabled;
            btnTimerPauseContinue.IsEnabled = isEnabled;
        }

        void ToggleTimerTextBox(bool isEnabled)
        {
            txtTimer.IsEnabled = isEnabled;
        }

        async void btnTimer_Start(object sender, EventArgs e)
        {
            if (_amountOfTime == 0)
            {
                lblTimerError.IsVisible = true;
                return;
            }

            lblTimerError.IsVisible = false;
            ToggleTimerButtons(true);
            ToggleTimerTextBox(false);
            ResetTimer();
            _isPaused = false;

            // start the timer on a background thread
            await Task.Run(async () => await StartTimer());
        }

        async void btnTimer_PauseContinue(object sender, EventArgs e)
        {
            if (!_isPaused)
            {
                btnTimerPauseContinue.Text = "Resume";
                btnTimerStop.Text = "Reset";
                _isPaused = true;
                CancellationToken.Cancel();
            }
            else
            {
                btnTimerPauseContinue.Text = "Pause";
                btnTimerStop.Text = "Stop";
                _isPaused = false;
                await Task.Run(async () => await StartTimer());
            }
        }

        void btnTimer_Stop(object sender, EventArgs e)
        {
            // stop the timer
            CancellationToken.Cancel();

            _timeElapsed = 0;

            // set timer
            ResetTimer();

            ToggleTimerButtons(false);

            ToggleTimerTextBox(true);

            btnTimerPauseContinue.Text = "Pause";
            btnTimerStop.Text = "Stop";
        }

        void txtTimer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnTimerStart.IsEnabled)
                // set timer
                ResetTimer();
        }

        void ResetTimer()
        {
            // make sure number again
            int time;
            bool isNumber = int.TryParse(txtTimer.Text, out time);

            // make sure time more than 0 and is in fact a number
            if (isNumber && time > 0)
            {
                // store total amount of time in seconds
                _amountOfTime = (time * 60) + 1;

                // windows likes to label stuff weird so we need to fix label
                int fixIt = 0;
                if (Device.OS != TargetPlatform.Android)
                    fixIt = 1;

                // find out the difference between now and the end time
                TimeSpan ts = DateTime.Now.AddSeconds(_amountOfTime - fixIt).Subtract(DateTime.Now);

                // dont allow more than this time limit
                // unrealistic but saves from parsing strings
                // 24 hours max
                if (_amountOfTime >= 86400)
                {
                    // reset the end time to 24 hours
                    // show the label in minutes, 1440 min in 24 hours
                    ts = DateTime.Now.AddHours(24).Subtract(DateTime.Now);
                    txtTimer.Text = "1440";
                }

                // parse the string hh:mm:ss
                lblTimer.Text = ts.ToString().Substring(0, 8);
            }
            else
            {
                _amountOfTime = 0;
                lblTimer.Text = "00:00:00";
            }
        }

        private async Task StartTimer()
        {
            // set the cancellation token
            CancellationToken = new CancellationTokenSource();

            // set the start and end time
            _endTime = DateTime.Now.AddSeconds(_amountOfTime - _timeElapsed);

            try
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    // check if cancel requested
                    CancellationToken.Token.ThrowIfCancellationRequested();

                    // every 1 second do this
                    await Task.Delay(1000, CancellationToken.Token).ContinueWith((arg) =>
                    {
                        // check if cancel requested again
                        if (!CancellationToken.Token.IsCancellationRequested)
                        {
                            // check again
                            CancellationToken.Token.ThrowIfCancellationRequested();

                            // do the actual work here
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                // check if we've reached the end time
                                if (_timeElapsed >= _amountOfTime - 1)
                                {

                                    // stop and close metronome file
                                    StopMetronome();
                                    ToggleMetronomeButtons(false);
                                    ToggleMetronomeTextBox(true);
                                    _audioService.CloseFile();

                                    // open alarm sound file and play
                                    _audioService.OpenFile("alarmsound.mp3");
                                    _audioService.PlayAudioFile();

                                    ToggleTimerButtons(false);
                                    ToggleTimerTextBox(true);

                                    CancellationToken.Cancel();
                                    _timeElapsed = 0;
                                }
                                else
                                {
                                    // update the label with the time remaining
                                    // hh:mm:ss
                                    lblTimer.Text = _endTime.Subtract(DateTime.Now).ToString().Substring(0, 8);

                                    // add one second to timeelapsed
                                    _timeElapsed++;
                                }
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Oops! Something went wrong. Try again later.", "OK");
            }
        }
        protected override async void OnAppearing()
        {
            try
            {
                ToggleMetronomeButtons(false);
                ToggleMetronomeTextBox(true);
                ToggleTimerTextBox(true);

                await _audioService.OpenFile("click.mp3");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Oops! Something went wrong. Try again later.", "OK");
            }
            base.OnAppearing();
        }

        protected override async void OnDisappearing()
        {
            try
            {
                ToggleMetronomeButtons(false);
                ToggleMetronomeTextBox(true);
                StopMetronome();

                if (CancellationToken != null)
                {
                    if (!_isPaused)
                    {
                        btnTimerPauseContinue.Text = "Resume";
                        btnTimerStop.Text = "Reset";
                        _isPaused = true;
                        CancellationToken.Cancel();
                    }
                }

                _audioService.CloseFile();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Oops! Something went wrong. Try again later.", "OK");
            }
            base.OnDisappearing();
        }
    }
}

