using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models.Exceptions
{
    public class ConvertFromViewModelToModelException : ConvertFromModelToViewModelException
    {
        public ConvertFromViewModelToModelException(
            string message, string modelPropertyName,
            string viewModelPropertyName, object value) :
                base(message, modelPropertyName, viewModelPropertyName, value)
        {

        }

    }
}
