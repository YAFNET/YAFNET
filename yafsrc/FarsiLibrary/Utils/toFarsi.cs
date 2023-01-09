using System.Globalization;
using FarsiLibrary.Localization;

namespace FarsiLibrary.Utils;

using System.Linq;

/// <summary>
/// Helper class to convert numbers to it's farsi equivalent. Use this class' methods to overcome a problem in displaying farsi numeric values.
/// </summary>
public sealed class toFarsi
{
    /// <summary>
    /// Converts a number in string format e.g. 14500 to its localized version, if <c>Localized</c> value is set to <c>true</c>.
    /// </summary>
    /// <param name="num"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public static string Convert(string num, CultureInfo culture)
    {
        if(string.IsNullOrEmpty(num))
            return num;

        var numEnglish = num.Select((t, i) => num.Substring(i, 1))
            .Aggregate(
                string.Empty,
                (current, s) => s switch
                    {
                        "0" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_0),
                        "1" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_1),
                        "2" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_2),
                        "3" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_3),
                        "4" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_4),
                        "5" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_5),
                        "6" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_6),
                        "7" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_7),
                        "8" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_8),
                        "9" => current + FALocalizeManager.Instance.GetLocalizerByCulture(culture).GetLocalizedString(StringID.Numbers_9),
                        _ => current + s
                    });

        return numEnglish;
    }

    /// <summary>
    /// Converts an English number to it's Farsi value.
    /// </summary>
    /// <remarks>This method only converts the numbers in a string, and does not convert any non-numeric characters.</remarks>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string Convert(string num)
    {
        return Convert(num, FALocalizeManager.Instance.FarsiCulture);
    }
}