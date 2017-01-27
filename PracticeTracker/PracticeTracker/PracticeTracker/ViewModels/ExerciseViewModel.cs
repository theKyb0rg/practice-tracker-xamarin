using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeTracker.ViewModels
{
    public class ExerciseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }

        public bool IsNewExercise { get; set; }
        public int Goal { get; set; }
        public string MaxTempoMessage { get; set; }
        public string GoalTempoMessage { get; set; }
    }
}
