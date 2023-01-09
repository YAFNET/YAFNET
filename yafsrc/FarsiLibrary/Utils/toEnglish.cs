namespace FarsiLibrary.Utils;

using System.Linq;

/// <summary>
/// Helper class to convert numbers to it's farsi equivalent. Use this class' methods to overcome a problem in displaying farsi numeric values.
/// </summary>
public sealed class toEnglish
{
    /// <summary>
    /// Converts a Farsi number to it's English numeric values.
    /// </summary>
    /// <remarks>This method only converts the numbers in a string, and does not convert any non-numeric characters.</remarks>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string Convert(string num)
    {
        if (string.IsNullOrEmpty(num))
            return num;

        var result = num.Select((t, i) => num.Substring(i, 1))
            .Aggregate(
                string.Empty,
                (current, numTemp) => numTemp switch
                    {
                        "۰" => current + "0",
                        "۱" => current + "1",
                        "۲" => current + "2",
                        "۳" => current + "3",
                        "۴" => current + "4",
                        "۵" => current + "5",
                        "۶" => current + "6",
                        "۷" => current + "7",
                        "۸" => current + "8",
                        "۹" => current + "9",
                        _ => current + numTemp
                    });

        return result;
    }
}