namespace YAF.Classes.Utils
{
  using System;

  /// <summary>
  /// This attribute is used to represent a string value
  /// for a value in an enum.
  /// </summary>
  public class StringValueAttribute : Attribute
  {
    #region Properties

    /// <summary>
    /// Holds the stringvalue for a value in an enum.
    /// </summary>
    public string StringValue
    {
      get;
      protected set;
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="StringValueAttribute"/> class. 
    /// Constructor used to init a StringValue Attribute
    /// </summary>
    /// <param name="value">
    /// </param>
    public StringValueAttribute(string value)
    {
      StringValue = value;
    }

    #endregion
  }
}