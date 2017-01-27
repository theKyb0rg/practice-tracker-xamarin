using PracticeTracker.Services;
using PracticeTracker.UWP.Services;
using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

[assembly: Dependency(typeof(AudioService))]
namespace PracticeTracker.UWP.Services
{
    public class AudioService : IAudioService
    {
        MediaElement _player;

        public void PlayAudioFile()
        {
            _player.Play();
        }

        public async Task OpenFile(string fileName)
        {
            try
            {
                StorageFolder folder = await Package.Current.InstalledLocation.GetFolderAsync("Assets");
                StorageFile sf = await folder.GetFileAsync(fileName);
                _player = new MediaElement();
                _player.AudioCategory = AudioCategory.Media;
                _player.SetSource(await sf.OpenAsync(FileAccessMode.Read), sf.ContentType);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void CloseFile()
        {

        }
    }
}
