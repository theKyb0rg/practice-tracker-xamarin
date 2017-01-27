using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeTracker.ViewModels
{
    public class PracticeViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Tempo { get; set; }
        public int Time { get; set; }
        public string ExerciseName { get; set; }
        public string TimeAndTempoDetails { get; set; }

        public bool IsNewPractice { get; set; }
    }
}
