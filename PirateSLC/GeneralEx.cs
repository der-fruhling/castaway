using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace PirateSLC
{
    internal static class GeneralEx
    {
        [Pure] public static bool Matches(this string str, string regex) => Regex.IsMatch(str, regex);
        [Pure] public static Match Match(this string str, string regex) => Regex.Match(str, regex);
    }
}