using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace SSCMobile.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// It prevents selecting a record from a list
        /// </summary>
        /// <param name="list">List to disable event</param>
        public static void HideSelection(this ListView list)
        {
            if (list != null)
            {
                list.ItemSelected += ((sender, e) => ((ListView)sender).SelectedItem = null);
            }
        }

        /// <summary>
        /// Verify that a list is not null and has records
        /// </summary>
        /// <typeparam name="T">type item list</typeparam>
        /// <param name="list">list to evaluate</param>
        /// <returns>list have records</returns>
        public static bool AnyExtended<T>(this IEnumerable<T> list)
        {
            return (list != null && list.Any());
        }

        /// <summary>
        /// Add range
        /// </summary>
        /// <typeparam name="T">Collection Type</typeparam>
        /// <param name="list">target list</param>
        /// <param name="range">Range to add</param>
        public static void AddRange<T>(this ICollection<T> list, ICollection<T> range)
        {
            if (list != null && range != null)
            {
                foreach (var item in range)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Clear the collection and add a range
        /// </summary>
        /// <typeparam name="T">Collection Type</typeparam>
        /// <param name="list">target list</param>
        /// <param name="range">Range to add</param>
        public static void ReplaceRange<T>(this ICollection<T> list, ICollection<T> range)
        {
            if (list != null && range != null)
            {
                list.Clear();

                foreach (var item in range)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Validate if an a string is not null or empty
        /// </summary>
        /// <param name="value">value to evaluate</param>
        /// <returns>Is not null or empty</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Validate if an a string is null or empty
        /// </summary>
        /// <param name="value">value to evaluate</param>
        /// <returns>Is value or empty</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Validate if an a string is not null or white space
        /// </summary>
        /// <param name="value">value to evaluate</param>
        /// <returns>Is not null or white space</returns>
        public static bool IsNotNullOrWhiteSpace(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Validate if an a string is null or white space
        /// </summary>
        /// <param name="value">value to evaluate</param>
        /// <returns>Is null or white space</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Get the firs letter of an a string
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>First string letter</returns>
        public static char GetFirstLetter(this string value)
        {
            return value.IsNotNullOrWhiteSpace() ? value[default(int)] : default(char);
        }

        /// <summary>
        /// Get the firs letter of an a string
        /// </summary>
        /// <param name="value">string</param>
        /// <param name="upperCase">Upper or lowercase result</param>
        /// <returns>First string letter</returns>
        public static char GetFirstLetter(this string value, bool upperCase)
        {
            var firstLetter = value.IsNotNullOrWhiteSpace() ? value[default(int)] : default(char);

            return upperCase ? char.ToUpperInvariant(firstLetter) : char.ToLowerInvariant(firstLetter);
        }

        /// <summary>
        /// Change the first string char to upper
        /// </summary>
        /// <param name="value">string to change</param>
        /// <returns>changed string</returns>
        public static string FirstCharToUpper(this string value)
        {
            var result = value;

            if (result.IsNotNullOrWhiteSpace())
            {
                char[] a = value.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                result = new string(a);
            }

            return result;
        }
    }
}