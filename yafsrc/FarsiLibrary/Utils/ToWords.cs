using System;

namespace FarsiLibrary.Utils;

/// <summary>
/// Classes to convert a number to its persian written form. It accepts both an Integer or Long as input parameter.
/// </summary>
/// <exception>Thrown when input number is larger than 999999999999</exception>
/// <example>
/// An example on how to convert a Integer number to words.
/// <code>
///		class MyClass 
///     {
///		   public static void Main() 
///        {
///		      Console.WriteLine(FarsiLibrary.Utils.ToWords.ToString(1452));
///		   }
///		}
/// </code>
/// </example>
/// <exception cref="ArgumentOutOfRangeException"></exception>
public static class ToWords
{
    private static readonly string[] cvtText = new string[1000];

    static ToWords()
    {
        cvtText[0] = "صفر";
        cvtText[1] = "يک";
        cvtText[2] = "دو";
        cvtText[3] = "سه";
        cvtText[4] = "چهار";
        cvtText[5] = "پنج";
        cvtText[6] = "شش";
        cvtText[7] = "هفت";
        cvtText[8] = "هشت";
        cvtText[9] = "نه";
        cvtText[10] = "ده";
        cvtText[11] = "یازده";
        cvtText[12] = "دوازده";
        cvtText[13] = "سیزده";
        cvtText[14] = "چهارده";
        cvtText[15] = "پانزده";
        cvtText[16] = "شانزده";
        cvtText[17] = "هفده";
        cvtText[18] = "هجده";
        cvtText[19] = "نوزده";
        cvtText[20] = "بيست";
        cvtText[21] = "سی";
        cvtText[22] = "چهل";
        cvtText[23] = "پنجاه";
        cvtText[24] = "شصت";
        cvtText[25] = "هفتاد";
        cvtText[26] = "هشتاد";
        cvtText[27] = "نود";
        cvtText[28] = "صد";
        cvtText[29] = "هزار";
        cvtText[30] = "میلیون";
        cvtText[31] = "میلیارد";
        cvtText[100] = "صد";
        cvtText[200] = "دویست";
        cvtText[300] = "سیصد";
        cvtText[400] = "چهارصد";
        cvtText[500] = "پانصد";
        cvtText[600] = "ششصد";
        cvtText[700] = "هفتصد";
        cvtText[800] = "هشتصد";
        cvtText[900] = "نهصد";
    }

    private static string cvt100(long Number)
    {
        var x = (int)Number;
        var result = string.Empty;
        int t;

        System.Diagnostics.Debug.Assert(x < 999);

        if (x > 99)
        {
            t = x / 100;
            result = t switch
                {
                    1 => cvtText[100],
                    2 => cvtText[200],
                    3 => cvtText[300],
                    4 => cvtText[400],
                    5 => cvtText[500],
                    6 => cvtText[600],
                    7 => cvtText[700],
                    8 => cvtText[800],
                    9 => cvtText[900],
                    _ => result
                };

            x = x - t * 100;

            if (x <= 0)
            {
                return result;
            }

            result += " و "; 
        }

        if (x > 20)
        {
            t = x / 10;
            result = result + cvtText[t + 18];
            x = x - t * 10;

            if (x <= 0)
            {
                return result;
            }

            result += " و "; 
        }

        switch (x)
        {
            case > 0:
                result += cvtText[x];
                break;
            case 0:
                result += cvtText[0];
                break;
        }

        return result;
    }


    /// <overloads>Has two overloads.</overloads>
    /// <summary>Converts an integer number to its written form in Persian</summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static string ToString(int x)
    {
        return ToString(long.Parse(x.ToString()));
    }

    public static string ToString(double  x)
    {
        return ToString((long)x);
    }

    /// <summary>Converts a long number to its written form in Persian</summary>
    /// <param name="x"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string ToString(long x)
    {
        long t;
        var result = string.Empty;

        switch (x)
        {
            case > 999999999999:
                throw new ArgumentOutOfRangeException(nameof(x), "Number is too large to process");
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(x), "Number should be a positive value");
            case > 999999999:
                {
                    t = x / 1000000000;
                    result += cvt100(t) + " " + cvtText[31];
                    x = x - t * 1000000000;

                    if (x <= 0)
                    {
                        return result;
                    }

                    result += " و ";
                    break;
                }
        }

        if (x > 999999)
        {
            t = x / 1000000;
            result += cvt100(t) + " " + cvtText[30];
            x = x - t * 1000000;

            if (x <= 0)
            {
                return result;
            }

            result += " و ";
        }

        if (x > 999)
        {
            t = x / 1000;
            result += cvt100(t) + " " + cvtText[29];
            x = x - t * 1000;

            if (x <= 0)
            {
                return result;
            }

            result += " و "; 
        }

        if (x >= 0)
        {
            result += cvt100(x);
        }

        return result;
    }
}