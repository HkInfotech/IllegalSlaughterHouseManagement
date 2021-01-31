using System;
using System.Collections.Generic;

namespace SSCMobile.Validations
{
    public class CompareRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public Func<T> CompareFunction { get; set; }

        public bool Check(T value)
        {
            if (CompareFunction == null)
                throw new ArgumentException();

            var compareValue = CompareFunction();

            if (value == null && compareValue == null)
                return true;

            if (value != null && compareValue == null)
                return false;

            if (value == null && compareValue != null)
                return false;

            return EqualityComparer<T>.Default.Equals(value, compareValue);
        }
    }
}