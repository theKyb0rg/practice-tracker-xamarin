using PracticeTracker.Persistence.Models;
using PracticeTracker.ViewModels;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PracticeTracker.Pages.StatisticPages
{
    public partial class StatisticDetailsPage : ContentPage
    {
        private ExerciseViewModel _exercise;
        private bool _chartLoaded;

        public StatisticDetailsPage(ExerciseViewModel exercise)
        {
            InitializeComponent();
            _exercise = exercise;
            _chartLoaded = false;
        }

        protected override async void OnAppearing()
        {
            if (!_chartLoaded)
            {
                // get total practices
                var practiceCount = await App.Database.CountPracticeByExerciseIdAsync(_exercise.Id);

                List<Practice> practices;
                if(practiceCount > 365)
                {
                    // just get the last 365 of them
                    practices = await App.Database.GetLastYearsPracticesFromTodayByExerciseIdAsync(_exercise.Id);
                }
                else
                {
                    // get everything
                    practices = await App.Database.GetPracticesByExerciseIdOrderByDateDescendingAsync(_exercise.Id);
                }

                // Set the goal text
                lblGoal.Text = _exercise.Goal + " bpm";

                // preset maxTempo to use for chart as well
                int maxTempo = 0;

                // do calculations if there is data
                if (practices.Count != 0)
                {
                    maxTempo = practices.Max(s => s.Tempo);
                    double goalProgress = Math.Round((double)maxTempo / _exercise.Goal * 100.0);

                    lblAvgPracticeTime.Text = Math.Round(practices.Average(s => s.Time)) + " min";
                    lblAvgTempo.Text = Math.Round(practices.Average(s => s.Tempo)) + " bpm";
                    lblLastPracticed.Text = practices.Max(s => s.Date).Date.ToString("MM/dd/yyyy");
                    lblMaxPracticeTime.Text = practices.Max(s => s.Time) + " min";
                    lblMaxTempo.Text = maxTempo + " bpm";
                    lblTotalPracticeTime.Text = Math.Round(Convert.ToDouble(practices.Sum(s => s.Time)) / 60, 2) + " hrs";
                    lblGoalProgress.Text = (goalProgress >= 100) ? "100%" : goalProgress + "%";
                }

                var chart = new SfChart()
                {

                    Legend = new ChartLegend(),
                    PrimaryAxis = new DateTimeAxis()
                    {
                        //IntervalType = (exercises.Count > 365) ? DateTimeIntervalType.Months : DateTimeIntervalType.Days,
                        //Interval = 6,
                        //RangePadding = DateTimeRangePadding.Additional,
                        EdgeLabelsDrawingMode = EdgeLabelsDrawingMode.Shift,
                        ShowMajorGridLines = true,
                        ShowMinorGridLines = true,
                        MinorTicksPerInterval = 1,
                        LabelStyle = new ChartAxisLabelStyle()
                        {
                            LabelFormat = "MM/dd/yyy"
                        },
                        IsVisible = (practices.Count > 1) ? true : false
                    },
                    SecondaryAxis = new NumericalAxis()
                    {
                        Minimum = 0,
                        Maximum = _exercise.Goal + maxTempo + 50,
                        //Interval = 50,
                        RangePadding = NumericalPadding.Additional,
                        StripLines = new NumericalStripLineCollection()
                        {
                            new NumericalStripLine()
                            {
                                 Start = _exercise.Goal,
                                 Width = 1,
                                 Text = "Goal",
                                 FillColor = Color.Red,
                                 LabelStyle = new ChartStripLineLabelStyle()
                                 {
                                       VerticalAlignment = ChartLabelAlignment.Far
                                 }
                            }
                        }
                    },
                    Series = new ChartSeriesCollection()
                    {
                        new SplineSeries()
                        {
                            ItemsSource = practices.Select(s => new ChartDataPoint(s.Date, s.Tempo)),
                            Label = "Tempo (bpm)"
                        },
                        //new SplineSeries()
                        //{
                        //    ItemsSource = practices.Select(s => new ChartDataPoint(s.Date, s.Time)),
                        //    Label = "Time (min)"
                        //}
                    },
                    Title = new ChartTitle() { Text = _exercise.Name },
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                // add to page
                Chart.Children.Add(chart);

                // terminatee the loading indicator
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;

                _chartLoaded = true;
            }

            base.OnAppearing();
        }
    }
}
