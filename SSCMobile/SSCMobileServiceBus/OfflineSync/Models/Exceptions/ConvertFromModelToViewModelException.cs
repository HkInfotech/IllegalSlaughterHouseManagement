using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobileServiceBus.OfflineSync.Models.Exceptions
{
    public class ConvertFromModelToViewModelException : ApplicationException
    {
        public string ModelPropertyName { get; private set; }
        public string ViewModelPropertyName { get; private set; }
        public object ValueToSet { get; private set; }

        public ConvertFromModelToViewModelException(string message, string modelPropertyName, string viewModelPropertyName, object value) : base(message)
        {
            this.ModelPropertyName = modelPropertyName;
            this.ViewModelPropertyName = viewModelPropertyName;
            this.ValueToSet = value;
        }
    }
}
