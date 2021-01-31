using SSCMobileServiceBus.OfflineSync.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobile.Models.Common
{
    public class SortOrder
    {
        public string ColumnName { get; set; } = "Name";
        public string Title { get; set; } = "Name";
        public SortTypes SortTypes { get; set; } = SortTypes.Ascending;

    }
}
