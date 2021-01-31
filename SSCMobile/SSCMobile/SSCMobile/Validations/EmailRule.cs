using System.Text.RegularExpressions;

namespace SSCMobile.Validations
{
    public class EmailRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }
            const string emailRegex = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            var str = value as string;
            Regex regex = new Regex(emailRegex);
            Match match = regex.Match(str.Trim());

            return match.Success;
        }
    }
}