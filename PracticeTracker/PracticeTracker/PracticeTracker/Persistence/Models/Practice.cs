using PracticeTracker.Helper;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeTracker.Persistence.Models
{
    // if adding a new column to the table add to the end or error will be thrown
    public class Practice : OnPropertyChangedHelper
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Tempo { get; set; }
        public int Time { get; set; }

        [MaxLength(2000)]
        public string Comment { get; set; }

        public int ExerciseId { get; set; }
    }
}
