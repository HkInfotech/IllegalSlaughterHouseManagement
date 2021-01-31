using SSCMobileServiceBus.OfflineSync.Models.ComplaintModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SSCMobile.Converters
{
    public class ListViewHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string lowsId = (string)value;
            int ListViewHeight = 10;
            if (!string.IsNullOrEmpty(lowsId))
            {
                var SplitLows = lowsId.Split(',')?.Select(Int32.Parse).ToList();

                foreach (var item in SplitLows)
                {
                    ListViewHeight = ListViewHeight + 50;
                }
                return ListViewHeight;
            }
            else
            {
                return ListViewHeight = 0;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
