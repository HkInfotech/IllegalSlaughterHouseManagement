using System;
using System.Text.RegularExpressions;

namespace SSCMobile.Validations
{
    public class MobilenumberRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null || Convert.ToString(value) == "")
            {
                return true;
            }
            else
            {
                var str = value as string;
                Regex regex = new Regex(@"^\d{10}$");
                Match match = regex.Match(str.Trim());
                return match.Success;
            }
        }
    }
}