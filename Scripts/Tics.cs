using System;
using Godot;

namespace Wolf.Scripts
{
    public sealed class Tics
    {
        private Tics()
        {
        }

        private struct TimeValue
        {
            public long seconds;
            public long microseconds;
        }

        private static TimeValue GetTimeOfDay()
        {
            return new TimeValue()
            {
                seconds = (long)(Time.GetTicksMsec() / 1000),
                microseconds = (long)Time.GetTicksUsec()
            };
        }

        private static TimeValue _timeValue;
        private static long _timeCount;

        private static long _lastTimeCount;

        public static long Count
        {
            get;
            private set;
        }

        public static void SetTimeCount(long timeCount)
        {
            _timeCount = timeCount;
        }

        public static long GetTimeCount()
        {
            TimeValue time = GetTimeOfDay();

            long secs = time.seconds - _timeValue.seconds;
            long usecs = time.microseconds - _timeValue.microseconds;

            if (usecs < 0)
            {
                usecs += 1000000;
                secs--;
            }

            long tc = _timeCount + secs * 70 + (usecs * 70) / 1000000;

            return tc;
        }

        public static void Calculate()
        {
            long newtime;
            long ticcount = 1;
            long tics;

            do
            {
                newtime = GetTimeCount();
                tics = newtime - _lastTimeCount;
            } while (tics <= ticcount);

            _lastTimeCount = newtime;

            Count = tics;
        }
    }
}
