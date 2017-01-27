using PracticeTracker.Persistence.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PracticeTracker.Persistence
{
    public class PracticeTrackerDb
    {
        readonly SQLiteAsyncConnection _database;

        public PracticeTrackerDb(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Category>().Wait();
            _database.CreateTableAsync<Exercise>().Wait();
            _database.CreateTableAsync<Practice>().Wait();
        }

        #region Category Methods

        public Task<Category> FindCategoryByNameAsync(string name)
        {
            return _database.FindAsync<Category>(s => s.Name == name);
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().ToListAsync();
        }

        public Task<Category> GetCategoryByIdAsync(int id)
        {
            return _database.Table<Category>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<Category>> GetCategoriesOrderByNameAsync()
        {
            return _database.Table<Category>().OrderBy(s => s.Name).ToListAsync();
        }

        #endregion

        #region Exercise Methods

        public Task<List<Exercise>> GetExercisesAsync()
        {
            return _database.Table<Exercise>().ToListAsync();
        }

        public Task<int> CountExerciseAsync()
        {
            return _database.Table<Exercise>().CountAsync();
        }

        public Task<Exercise> GetExerciseByIdAsync(int id)
        {
            return _database.Table<Exercise>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<Exercise>> GetExercisesByCategoryIdAsync(int categoryId)
        {
            return _database.Table<Exercise>().Where(s => s.CategoryId == categoryId).ToListAsync();
        }

        public Task<List<Exercise>> GetExercisesOrderByNameAsync()
        {
            return _database.Table<Exercise>().OrderBy(s => s.Name).ToListAsync();
        }

        public Task<Exercise> FindExerciseByNameAsync(string name)
        {
            return _database.FindAsync<Exercise>(s => s.Name == name);
        }


        #endregion

        #region Practice Methods

        public Task<List<Practice>> GetPracticesAsync()
        {
            return _database.Table<Practice>().ToListAsync();
        }

        public Task<int> CountPracticeAsync()
        {
            return _database.Table<Practice>().CountAsync();
        }

        public Task<int> CountPracticeByExerciseIdAsync(int exerciseId)
        {
            return _database.Table<Practice>()
                    .Where(s => s.ExerciseId == exerciseId)
                    .CountAsync();
        }

        public Task<List<Practice>> GetLastYearsPracticesFromTodayByExerciseIdAsync(int exerciseId)
        {
            return _database.Table<Practice>()
                       .Where(s => s.ExerciseId == exerciseId)
                       .OrderByDescending(s => s.Date)
                       .Take(365)
                       .ToListAsync();
        }

        public Task<List<Practice>> GetPracticesByExerciseIdOrderByDateDescendingAsync(int exerciseId)
        {
            return _database.Table<Practice>()
                       .Where(s => s.ExerciseId == exerciseId)
                       .OrderByDescending(s => s.Date)
                       .ToListAsync();
        }

        public Task<List<Practice>> GetPracticesByExerciseIdAsync(int exerciseId)
        {
            return _database.Table<Practice>().Where(s => s.ExerciseId == exerciseId).ToListAsync();
        }

        public Task<Practice> GetPracticeByIdAsync(int id)
        {
            return _database.Table<Practice>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<Practice>> GetPracticesOrderByDateDescendingAsync()
        {
            return _database.Table<Practice>().OrderByDescending(s => s.Date).ToListAsync();
        }

        public Task<Practice> GetPracticeWithMaxTempoByExerciseIdAsync(int exerciseId)
        {
            return _database.Table<Practice>()
                .Where(s => s.ExerciseId == exerciseId)
                .OrderByDescending(s => s.Tempo)
                .Take(1)
                .FirstOrDefaultAsync();
        }
        #endregion

        #region GenericCRUD

        public Task DeleteAsync<T>(T entity)
        {
            return _database.DeleteAsync(entity);
        }

        public Task InsertAsync<T>(T entity)
        {
            return _database.InsertAsync(entity);
        }

        public Task UpdateAsync<T>(T entity)
        {
            return _database.UpdateAsync(entity);
        }

        #endregion

        #region Helpers
        public void DropAllTables()
        {
            _database.DropTableAsync<Category>().Wait();
            _database.DropTableAsync<Exercise>().Wait();
            _database.DropTableAsync<Practice>().Wait();
        }

        public void SeedTestData()
        {
            Random rnd = new Random();
            _database.InsertAllAsync(new List<Category>
            {
                new Category { Name = "Category 1" },
                new Category { Name = "Category 2" },
                new Category { Name = "Category 3" },
            }).Wait();

            _database.InsertAllAsync(new List<Exercise>
            {
                new Exercise { Name = "Exercise 1", CategoryId = 1 },
                new Exercise { Name = "Exercise 2", CategoryId = 2 },
                new Exercise { Name = "Exercise 3", CategoryId = 3 },
                new Exercise { Name = "Exercise 4", CategoryId = 3 }
            }).Wait();

            for (int i = 1; i <= 4; i++)
            {
                for (int j = 0; j < 5000; j++)
                {
                    _database.InsertAsync(new Practice
                    {
                        Date = DateTime.Now.AddDays(j),
                        ExerciseId = i,
                        Tempo = rnd.Next(1, 200),
                        Time = rnd.Next(1, 60)
                    }).Wait();
                }
            }
        }

        #endregion
    }
}
