﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobile.Extensions
{
    public static class DoubleExtensions
    {
        public static double Clamp(this double self, double min, double max)
        {
            return Math.Min(max, Math.Max(self, min));
        }
    }
}
