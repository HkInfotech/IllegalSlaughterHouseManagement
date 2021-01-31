using System;

namespace SSCMobile.Helpers
{
    public class Responce<T>
    {
        public bool Success { get; set; }
        public T ResponeContent { get; set; }
        public string Message { get; set; }
        public string StackStrace { get; set; }
        public string Version { get; set; }
        public DateTime RequestDatetime { get; set; }
    }
}