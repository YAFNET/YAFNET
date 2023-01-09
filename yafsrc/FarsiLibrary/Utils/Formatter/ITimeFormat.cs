namespace FarsiLibrary.Utils.Formatter;

public interface ITimeFormat
{
    /// <summary>
    /// Formats a duration
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    string Format(Duration duration);

    double RoundingTolerance { get; set; }

    string Pattern { get; set; }

    string FuturePrefix { get; set; }

    string FutureSuffix { get; set; }

    string PastPrefix { get; set; }

    string PastSuffix { get; set; }
}