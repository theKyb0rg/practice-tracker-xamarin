using System;
namespace PracticeTracker.Extensions
{
    public class Timer
    {
        private double _interval;
        public double Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                if (Enabled)
                {
                    Start();
                }
            }
        }
        private Action _callBack;
        public Action CallBack
        {
            get { return _callBack; }
            set
            {
                _callBack = value;
                if (Enabled)
                {
                    Start();
                }
            }
        }
        private TimerHolder timer = null;
        public Timer(double interval, Action callBack)
        {
            Interval = interval;
            CallBack = callBack;
        }
        public bool Enabled
        {
            get { return timer != null; }
        }
        public void Start()
        {
            Stop();
            timer = new TimerHolder(Interval, CallBack);
        }
        public void Stop()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }
    }

    class TimerHolder
    {
        public bool Enabled { get; set; }
        public TimerHolder(double interval, Action callBack)
        {
            Enabled = true;
            Xamarin.Forms.Device.StartTimer(TimeSpan.FromMilliseconds(interval), delegate
            {
                if (Enabled)
                {
                    callBack();
                }
                return Enabled;
            });
        }
        public void Stop()
        {
            Enabled = false;
        }
    }

}
