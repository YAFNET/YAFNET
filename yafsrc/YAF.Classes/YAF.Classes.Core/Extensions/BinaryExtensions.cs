namespace YAF.Classes.Utils
{
  using YAF.Classes.Data;

  public static class BinaryExtensions
  {
    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(this int value, int checkAgainst)
    {
      return (value & checkAgainst) == checkAgainst;
    }

    /// <summary>
    /// The binary and.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="checkAgainst">
    /// The check against.
    /// </param>
    /// <returns>
    /// The binary and.
    /// </returns>
    public static bool BinaryAnd(this object value, object checkAgainst)
    {
      return BinaryAnd(SqlDataLayerConverter.VerifyInt32(value), SqlDataLayerConverter.VerifyInt32(checkAgainst));
    }    
  }
}