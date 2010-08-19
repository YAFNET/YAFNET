namespace YAF.Classes.Core
{
  #region Using

  using System;

  #endregion

  /// <summary>
  /// The no valid guest user for board exception.
  /// </summary>
  public class NoValidGuestUserForBoardException : Exception
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="NoValidGuestUserForBoardException"/> class.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public NoValidGuestUserForBoardException(string message)
      : base(message)
    {
    }

    #endregion
  }
}