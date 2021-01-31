using System;

namespace SSCMobile.Validations
{
    public class SpeciesSelectRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }
            else if ((Convert.ToString(value) == "---Select species---"))
            {
                return false;
            }
            else
            {
                return true;
            }
            return false;
        }
    }
}