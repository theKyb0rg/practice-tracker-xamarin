using PracticeTracker.Helper;
using SQLite;
using System.Collections.Generic;

namespace PracticeTracker.Persistence.Models
{
    // if adding a new column to the table add to the end or error will be thrown
    public class Category : OnPropertyChangedHelper
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        private string _name;

        [MaxLength(255)]
        [Unique]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;

                OnPropertyChanged();
            }
        }
    }
}