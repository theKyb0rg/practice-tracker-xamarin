using AVFoundation;
using Foundation;
using PracticeTracker.iOS.Services;
using PracticeTracker.Services;
using Xamarin.Forms;
using System.IO;
using System;

[assembly: Dependency(typeof(AudioService))]
namespace PracticeTracker.iOS.Services
{
    public class AudioService : NSObject, IAudioService, IAVAudioPlayerDelegate
    {
        public AudioService() { }

        public void PlayAudioFile(string fileName)
        {
            NSError error = null;
            AVAudioSession.SharedInstance().SetCategory(AVAudioSession.CategoryPlayback, out error);

            string sFilePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(fileName), Path.GetExtension(fileName));
            var url = NSUrl.FromString(sFilePath);
            var _player = AVAudioPlayer.FromUrl(url);
            _player.Delegate = this;
            _player.Volume = 100f;
            _player.PrepareToPlay();
            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
            };
            _player.Play();
        }

        public void PlayClickTrack()
        {
            throw new NotImplementedException();
        }
    }
}