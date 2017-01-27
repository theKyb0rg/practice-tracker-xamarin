using System;
using PracticeTracker.iOS.Persistence;
using Xamarin.Forms;
using PracticeTracker.Persistence;
using System.IO;

[assembly: Dependency(typeof(FileHelper))]

namespace PracticeTracker.iOS.Persistence
{
    class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}