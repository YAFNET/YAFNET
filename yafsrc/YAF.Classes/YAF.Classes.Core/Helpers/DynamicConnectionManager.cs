namespace YAF.Classes.Core
{
  using YAF.Classes.Data;

  /// <summary>
  /// The yaf dynamic db conn manager.
  /// </summary>
  public class YafDynamicDBConnManager : YafDBConnManager
  {
    /// <summary>
    /// Gets ConnectionString.
    /// </summary>
    public override string ConnectionString
    {
      get
      {
        if (YafContext.Current.Vars.ContainsKey("ConnectionString"))
        {
          return YafContext.Current.Vars["ConnectionString"] as string;
        }

        return Config.ConnectionString;
      }
    }
  }
}