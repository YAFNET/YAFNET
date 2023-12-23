using System;

namespace ServiceStack;

/// <summary>
/// Document a longer form description about a Type
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class NotesAttribute : AttributeBase
{
    /// <summary>
    /// Get or sets a Label
    /// </summary>
    /// <value>The notes.</value>
    public string Notes { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="NotesAttribute"/> class.
    /// </summary>
    /// <param name="notes">The notes.</param>
    public NotesAttribute(string notes) => Notes = notes;
}