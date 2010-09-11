namespace YAF.Classes.Pattern
{
  using System;

  /// <summary>
  /// Provides functions used for code contracts.
  /// </summary>
  public static class CodeContracts
  {
    
    /// <summary>
    /// Validates argument (obj) is not <see langword="null"/>. Throws exception
    /// if it is.
    /// </summary>
    /// <typeparam name="T">type of the argument that's being verified
    /// </typeparam>
    /// <param name="obj">value of argument to verify not null</param>
    /// <param name="argumentName">name of the argument</param>
    /// <exception cref="ArgumentNullException"><paramref name="obj" /> is 
    /// <c>null</c>.</exception>
    [AssertionMethod]
    public static void ArgumentNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)]T obj, string argumentName) where T : class
    {
      if (obj == null)
      {
        throw new ArgumentNullException(argumentName, String.Format("{0} cannot be null", argumentName));
      }
    }
  }
}