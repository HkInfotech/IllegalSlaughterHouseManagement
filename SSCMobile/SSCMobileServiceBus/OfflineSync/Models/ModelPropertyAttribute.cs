using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models
{
    public class ModelPropertyAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public ModelPropertyAttribute(string propName)
        {
            this.PropertyName = propName;
        }

    }
}
