using System;
using FarsiLibrary.Utils.Internals;

namespace FarsiLibrary.Utils.Formatter;

using System.Globalization;

public class BasicTimeFormat : ITimeFormat
{
    public static string NEGATIVE = "-";
    public static string SIGN = "%s";
    public static string QUANTITY = "%n";
    public static string UNIT = "%u";

    public double RoundingTolerance { get;set; }
    public string Pattern { get;set; }
    public string FuturePrefix { get; set; }
    public string FutureSuffix { get;set; }
    public string PastPrefix { get; set; }
    public string PastSuffix { get; set; }

    public BasicTimeFormat()
    {
        this.RoundingTolerance = 0;
        this.Pattern = string.Empty;
        this.FuturePrefix = string.Empty;
        this.FutureSuffix = string.Empty;
        this.PastPrefix = string.Empty;
        this.PastSuffix = string.Empty;
    }

    public string Format(Duration duration)
    {
        var sign = GetSign(duration);
        var quantity = this.GetQuantity(duration);
        var unit = GetGramaticallyCorrectName(duration, quantity);
        var result = this.ApplyPattern(sign, unit, quantity);

        result = this.Decorate(sign, result);

        return result;
    }

    private string Decorate(string sign, string result)
    {
        if (sign == NEGATIVE)
        {
            result = this.PastPrefix + " " + result + " " + this.PastSuffix;
        }
        else
        {
            result = this.FuturePrefix + " " + result + " " + this.FutureSuffix;
        }

        return result.Trim();
    }

    private string ApplyPattern(string sign, string unit, double quantity)
    {
        var result = this.Pattern.Replace(SIGN, sign);
        var number = FormatNumber(quantity);

        result = result.Replace(QUANTITY, number);
        result = result.Replace(UNIT, unit);

        return result;
    }

    private static string FormatNumber(double quantity)
    {
        return CultureHelper.IsFarsiCulture() ? ToWords.ToString(quantity) : quantity.ToString(CultureInfo.InvariantCulture);
    }

    private double GetQuantity(Duration duration)
    {
        var quantity = Math.Abs(duration.Quantity);

        if (duration.Delta != 0)
        {
            var threshold = Math.Abs(duration.Delta / duration.Unit.MillisPerUnit * 100);
            if (threshold < this.RoundingTolerance)
            {
                quantity = quantity + 1;
            }
        }

        return Math.Truncate(quantity);
    }

    private static string GetGramaticallyCorrectName(Duration d, double quantity)
    {
        var result = d.Unit.Name;
        var value = Math.Abs(quantity);
        if (value is 0 or > 1)
        {
            result = d.Unit.PluralName;
        }

        return result;
    }

    private static string GetSign(Duration d)
    {
        return d.Quantity < 0 ? NEGATIVE : string.Empty;
    }

    public BasicTimeFormat SetPattern(string pattern)
    {
        this.Pattern = pattern;
        return this;
    }

    public BasicTimeFormat SetFuturePrefix(string futurePrefix)
    {
        this.FuturePrefix = futurePrefix.Trim();
        return this;
    }

    public BasicTimeFormat SetFutureSuffix(string futureSuffix)
    {
        this.FutureSuffix = futureSuffix.Trim();
        return this;
    }

    public BasicTimeFormat SetPastPrefix(string pastPrefix)
    {
        this.PastPrefix = pastPrefix.Trim();
        return this;
    }

    public BasicTimeFormat SetPastSuffix(string pastSuffix)
    {
        this.PastSuffix = pastSuffix.Trim();
        return this;
    }
}