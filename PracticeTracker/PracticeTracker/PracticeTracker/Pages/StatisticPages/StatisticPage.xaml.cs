using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PracticeTracker.Pages.StatisticPages
{
    public partial class StatisticPage : ContentPage
    {
        private bool _chartLoaded;

        public StatisticPage()
        {
            InitializeComponent();

            _chartLoaded = false;
        }

        protected override async void OnAppearing()
        {
            if (!_chartLoaded)
            {
                // load all initial data
                var practices = await App.Database.GetPracticesAsync();
                var categories = await App.Database.GetCategoriesAsync();
                var exercises = await App.Database.GetExercisesAsync();

                // do calculations if there is data
                if (practices.Count != 0)
                {
                    lblAvgPracticeTime.Text = Math.Round(practices.Average(s => s.Time)) + " min";
                    lblAvgTempo.Text = Math.Round(practices.Average(s => s.Tempo)) + " bpm";
                    lblLastPracticed.Text = practices.Max(s => s.Date).Date.ToString("MM/dd/yyyy");
                    lblStartDate.Text = practices.Min(s => s.Date).Date.ToString("MM/dd/yyyy");
                    lblMaxPracticeTime.Text = practices.Max(s => s.Time) + " min";
                    lblMaxTempo.Text = practices.Max(s => s.Tempo) + " bpm";
                    lblTotalPracticeSessions.Text = practices.Count.ToString();
                    lblTotalPracticeTime.Text = Math.Round(Convert.ToDouble(practices.Sum(s => s.Time)) / 60, 2) + " hrs";
                }

                if (exercises.Count != 0)
                    lblTotalExercises.Text = exercises.Count.ToString();

                if (categories.Count != 0)
                    lblTotalCategories.Text = categories.Count.ToString();

                // create chart
                var chart = new SfChart()
                {
                    PrimaryAxis = new CategoryAxis()
                    {
                        LabelPlacement = LabelPlacement.BetweenTicks,
                        LabelStyle = new ChartAxisLabelStyle()
                        {
                            Font = Font.SystemFontOfSize(12)
                        }
                    },
                    SecondaryAxis = new NumericalAxis()
                    {
                        RangePadding = NumericalPadding.Auto
                    },
                    Series = new ChartSeriesCollection()
                    {
                        new ColumnSeries()
                        {
                            ItemsSource = new ObservableCollection<ChartDataPoint>()
                            {
                                new ChartDataPoint("Categories", categories.Count),
                                new ChartDataPoint("Exercises", exercises.Count),
                                new ChartDataPoint("Practice Sessions", practices.Count)
                            }
                        }
                    },
                    Title = new ChartTitle() { Text = "Totals" },
                    VerticalOptions = LayoutOptions.FillAndExpand
                };

                // modify custom color pallette
                chart.Series[0].ColorModel.Palette = ChartColorPalette.Custom;
                chart.Series[0].ColorModel.CustomBrushes = new List<Color>()
                {
                    Color.Red,
                    Color.Blue,
                    Color.Green
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
