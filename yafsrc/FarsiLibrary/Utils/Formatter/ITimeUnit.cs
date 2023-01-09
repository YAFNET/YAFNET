namespace FarsiLibrary.Utils.Formatter;

public interface ITimeUnit
{
    ITimeFormat Format { get; }

    /// <summary>
    /// The number of milliseconds represented by each 
    /// instance of this TimeUnit.
    /// </summary>
    double MillisPerUnit { get; }

    /// <summary>
    /// The maximum quantity of this Unit to be used as a threshold for the next
    /// largest Unit (e.g. if one <code>Second</code> represents 1000ms, and
    /// <code>Second</code> has a maxQuantity of 5, then if the difference
    /// between compared timestamps is larger than 5000ms, PrettyTime will move
    /// on to the next smallest TimeUnit for calculation; <code>Minute</code>, by
    /// default)
    /// </summary>
    double MaxQuantity { get; }

    /// <summary>
    /// The grammatically singular name for this unit of time. (e.g. one "second")
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The grammatically plural name for this unit of time. (e.g. many "seconds")
    /// </summary>
    /// <returns></returns>
    string PluralName { get; }
}