using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PracticeTracker.Helper
{
    public class OnPropertyChangedHelper : INotifyPropertyChanged
    {
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
