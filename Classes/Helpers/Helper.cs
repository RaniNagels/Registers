using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.Classes.Helpers
{
    class Helper
    {
        public static bool ContainsIgnoreCaseAndDiacritics(string? source, string? search)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(search))
                return false;

            return CultureInfo.InvariantCulture.CompareInfo.IndexOf(
                source,
                search,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace
            ) >= 0;
        }

        public static bool ContainsAnyWordIgnoreCaseAndDiacritics(string? text, IEnumerable<string> words)
        {
            if (string.IsNullOrWhiteSpace(text) || words is null)
                return false;

            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

            return words.Any(word =>
                !string.IsNullOrWhiteSpace(word) &&
                compareInfo.IndexOf(text, word, options) >= 0);
        }

        public static bool ContainsAllWordsInAnyField(string[] fields, IEnumerable<string> words)
        {
            if (fields is null || words is null) return false;

            var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            var options = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

            return words.All(word =>
                !string.IsNullOrWhiteSpace(word) &&
                fields.Any(field => compareInfo.IndexOf(field, word, options) >= 0));
        }
    }
}
