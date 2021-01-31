using System;

namespace SSCMobile.Validations
{
    public class CityValidate<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }
            else if ((Convert.ToString(value) == "---Select city---"))
            {
                return false;
            }
            else
            {
                if ((Convert.ToString(value) == "Surat") || (Convert.ToString(value) == "Bangalore"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}