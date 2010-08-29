namespace YAF.Classes.Utils
{
  #region Using

  using System;

  using YAF.Classes.Core;

  #endregion

  /// <summary>
  /// The resources extensions.
  /// </summary>
  public static class ResourcesExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get hours offset.
    /// </summary>
    /// <param name="resource">
    /// The resource.
    /// </param>
    /// <returns>
    /// The get hours offset.
    /// </returns>
    public static decimal GetHoursOffset(this ResourcesPageResource resource)
    {
      // calculate hours -- can use prefix of either UTC or GMT...
      decimal hours = 0;

      try
      {
        hours = Convert.ToDecimal(resource.tag.Replace("UTC", string.Empty).Replace("GMT", string.Empty));
      }
      catch (FormatException)
      {
        hours =
          Convert.ToDecimal(resource.tag.Replace(".", ",").Replace("UTC", string.Empty).Replace("GMT", string.Empty));
      }

      return hours;
    }

    #endregion
  }
}