using System.Threading.Tasks;

namespace PracticeTracker.Services
{
    public interface IAudioService
    {
        void PlayAudioFile();
        Task OpenFile(string fileName);
        void CloseFile();
    }
}
