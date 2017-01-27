using PracticeTracker.Services;
using Android.Media;
using Xamarin.Forms;
using PracticeTracker.Droid.Services;
using Android.Content.Res;
using System;
using System.Threading.Tasks;

[assembly: Dependency(typeof(AudioService))]
namespace PracticeTracker.Droid.Services
{
    public class AudioService : IAudioService
    {
        static MediaPlayer _player;
        static AssetFileDescriptor _fd;

        public async Task OpenFile(string fileName)
        {
            try
            {
                _player = new MediaPlayer();
                _fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                _player.SetDataSource(_fd.FileDescriptor, _fd.StartOffset, _fd.Length);
                _player.Prepare();
            }
            catch (Exception e)
            {

                throw;
            }
           
        }
        public void CloseFile()
        {
            if (_fd != null)
            {
                _fd.Close();
                //_fd.Dispose();
            }

            if (_player != null)
            {
                //_player.Release();
                _player.Reset();
                //_player.Dispose();
            }
        }

        public void PlayAudioFile()
        {
            //_player.Reset();
            _player.Start();
        }
    }
}