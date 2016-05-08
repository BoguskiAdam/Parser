using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public static class TimeConverter
    {
        public static TimeSpan StringToTimeSpan(string time)
        {
            int indexOfStart = time.IndexOf(":");
            int hours = int.Parse(time.Substring(0, indexOfStart));
            int minutes = int.Parse(time.Substring(indexOfStart + 1, time.Length - indexOfStart - 1)); 
            TimeSpan timeSpan = new TimeSpan(hours,minutes,0);
            return timeSpan;
        }
    }
}
