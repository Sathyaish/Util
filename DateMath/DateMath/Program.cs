using System;
using System.Collections.Generic;

// I want to have something like this:
// DateMath Now|Today|Tomorrow|Yesterday|ThisWednesday +years:1 
//                                      | +months:4 | +days:5 | +hours:2 
//                                      | +min:24 | +seconds:19 
//                                      | +milliseconds:100
//                                      /f:LongDate|ShortDate|LongTime|ShortTime|LongDateAndTime|ShortDateAndTime
// Or a minus sign in front of each to subtract from the base date

// My related question:
// Link: http://stackoverflow.com/questions/38099576/is-there-a-generic-numeric-type-in-c
// Permalink: http://stackoverflow.com/q/38099576/303685

namespace DateMath 
{
    class Program
    {
        private static readonly Dictionary<string, Func<DateTime, double, DateTime>> _resultSelectors =
            new Dictionary<string, Func<DateTime, double, DateTime>>();

        static Program()
        {
            InitializeResultSelectors();
        }

        static void InitializeResultSelectors()
        {
            _resultSelectors.Add("millisecond", (dt, m) => dt.AddMilliseconds(m));
            _resultSelectors.Add("milliseconds", (dt, m) => dt.AddMilliseconds(m));

            _resultSelectors.Add("second", (dt, s) => dt.AddSeconds(s));
            _resultSelectors.Add("seconds", (dt, s) => dt.AddSeconds(s));

            _resultSelectors.Add("minute", (dt, m) => dt.AddMinutes(m));
            _resultSelectors.Add("minutes", (dt, m) => dt.AddMinutes(m));

            _resultSelectors.Add("hour", (dt, h) => dt.AddHours(h));
            _resultSelectors.Add("hours", (dt, h) => dt.AddHours(h));

            _resultSelectors.Add("day", (dt, d) => dt.AddDays(d));
            _resultSelectors.Add("days", (dt, d) => dt.AddDays(d));

            _resultSelectors.Add("month", (dt, m) => dt.AddMonths((int)m));
            _resultSelectors.Add("months", (dt, m) => dt.AddMonths((int)m));

            _resultSelectors.Add("year", (dt, y) => dt.AddYears((int)y));
            _resultSelectors.Add("years", (dt, y) => dt.AddYears((int)y));
        }

        static void Main(string[] args)
        {
            var then = DateTime.Now.AddDays(396).ToLongDateString();

            Console.WriteLine(then);

            Console.ReadKey();
        }
    }
}