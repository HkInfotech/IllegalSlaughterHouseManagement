using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobile.Manager
{
    public static class Mapper
    {
        public static T ConvertTo<T>(this BaseViewModel item)
        {
            return default(T);
        }
    }
}
