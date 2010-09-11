namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Web;
  using System.Web.UI;

  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The control extensions.
  /// </summary>
  public static class ControlExtensions
  {
    #region Public Methods

    /// <summary>
    /// Creates a ID Based on the Control Structure
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get extended id.
    /// </returns>
    public static string GetExtendedID(this Control currentControl, string prefix)
    {
      string createdID = null;

      if (currentControl.ID.IsSet())
      {
        createdID = currentControl.ID + "_";
      }

      if (prefix.IsSet())
      {
        createdID += prefix;
      }
      else
      {
        createdID += Guid.NewGuid().ToString().Substring(0, 5);
      }

      return createdID;
    }

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public static string GetUniqueID(this Control currentControl, string prefix)
    {
      if (prefix.IsSet())
      {
        return prefix + Guid.NewGuid().ToString().Substring(0, 5);
      }
      else
      {
        return Guid.NewGuid().ToString().Substring(0, 10);
      }
    }

    /// <summary>
    /// The html encode.
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The html encode.
    /// </returns>
    public static string HtmlEncode(this Control currentControl, object data)
    {
      return HttpContext.Current.Server.HtmlEncode(data.ToString());
    }

    /// <summary>
    /// Gets PageContext.
    /// </summary>
    /// <param name="currentControl">
    /// The current Control.
    /// </param>
    public static YafContext PageContext(this Control currentControl)
    {
      if (currentControl.Site != null && currentControl.Site.DesignMode)
      {
        // design-time, return null...
        return null;
      }

      return YafContext.Current;
    }

    #endregion
  }
}