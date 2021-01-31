using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SSCWebApi.Helper
{
    public class Responce<T>
    {
        public bool Success { get; set; }
        public T ResponeContent { get; set; }
        public string Message { get; set; }
        public string StackStrace { get; set; }
        public string Version { get; set; }
        public double TotalResponceTime
        {
            get
            {
                responceStopWatch.Stop();
                return responceStopWatch.ElapsedMilliseconds;
            }
        }
        public DateTime RequestDatetime { get; set; }

        private Stopwatch responceStopWatch;
        public Responce()
        {
            responceStopWatch = new Stopwatch();
            RequestDatetime = DateTime.Now;
            responceStopWatch.Start();
        }

        public void Fail(string error)
        {
            Success = false;
            Message = error;
        }
    }
}