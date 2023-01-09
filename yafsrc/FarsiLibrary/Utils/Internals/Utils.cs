namespace FarsiLibrary.Utils.Internals;

internal static class Util
{
    /// <summary>
    /// Adds a preceding zero to single day or months
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    internal static string toDouble(int i)
    {
        return i > 9 ? i.ToString() : $"0{i}";
    }
}