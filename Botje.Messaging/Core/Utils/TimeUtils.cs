using System;
using System.Diagnostics;
using System.Linq;

namespace Botje.Core.Utils
{
    /// <summary>
    /// Helpers for working with time.
    /// </summary>
    public static class TimeUtils
    {
        private static TimeZoneInfo _tzInfo;

        /// <summary>
        /// Returns the currently registered local user's timezone.
        /// </summary>
        public static TimeZoneInfo TzInfo { get { return _tzInfo; } }

        static TimeUtils()
        {
            Initialize(new string[] { "UTC" });
        }

        /// <summary>
        /// Initialize to these timezones. Note that windows timzeones are different form Unix/Linux ones! Give this method multiple choices and the first one that exists will be used.
        /// </summary>
        /// <param name="timezones"></param>
        public static void Initialize(string[] timezones)
        {
            _tzInfo = null;
            foreach (var tzID in timezones)
            {
                _tzInfo = TimeZoneInfo.GetSystemTimeZones().Where(x => x.Id == tzID).FirstOrDefault();
                if (null != _tzInfo)
                {
                    return;
                }
            }
            if (null == _tzInfo)
            {
                string tzstr = string.Join(", ", TimeZoneInfo.GetSystemTimeZones().OrderBy(x => x.Id).Select(x => $"'{x.Id}'").ToArray());
                Debug.WriteLine($"Timezone not found, pick one of these: {tzstr}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localTime"></param>
        /// <returns></returns>
        public static DateTime ToUTC(DateTime localTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(localTime, _tzInfo);
        }

        public static TimeSpan ParseTimeSpan(string v)
        {
            TimeSpan result = TimeSpan.Zero;
            string number = "";
            foreach (char c in v)
            {
                if (char.IsDigit(c))
                {
                    number += c;
                }
                else
                {
                    switch (c)
                    {
                        case 'd':
                            result += TimeSpan.FromDays(Int32.Parse(number));
                            break;
                        case 'w':
                            result += TimeSpan.FromDays(7 * Int32.Parse(number));
                            break;
                        case 'h':
                        case 'u':
                            result += TimeSpan.FromHours(Int32.Parse(number));
                            break;
                    }
                    number = "";
                }
            }
            if (!string.IsNullOrWhiteSpace(number))
            {
                result += TimeSpan.FromDays(Int32.Parse(number));
            }
            return result;
        }

    }
}
