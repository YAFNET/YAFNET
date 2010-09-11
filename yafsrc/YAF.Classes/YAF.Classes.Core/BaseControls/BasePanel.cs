namespace YAF.Controls
{
  using System;
  using System.Web.UI.WebControls;

  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// Control derived from Panel that includes a reference to the <see cref="YafContext"/>.
  /// </summary>
  public class BasePanel : Panel
  {
    /// <summary>
    /// Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return this.PageContext();
      }
    }

    /// <summary>
    /// Creates a Unique ID
    /// </summary>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get unique id.
    /// </returns>
    public string GetUniqueID(string prefix)
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
    /// Creates a ID Based on the Control Structure
    /// </summary>
    /// <param name="prefix">
    /// </param>
    /// <returns>
    /// The get extended id.
    /// </returns>
    public string GetExtendedID(string prefix)
    {
      string createdID = null;

      if (this.ID.IsSet())
      {
        createdID = ID + "_";
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
  }
}