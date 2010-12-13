namespace YAF.Classes.Core
{
  #region Using

  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The i permissions extensions.
  /// </summary>
  public static class IPermissionsExtensions
  {
    #region Public Methods

    /// <summary>
    /// The check.
    /// </summary>
    /// <param name="permissions">
    /// The permissions.
    /// </param>
    /// <param name="permission">
    /// The permission.
    /// </param>
    /// <returns>
    /// The check.
    /// </returns>
    public static bool Check([NotNull] this IPermissions permissions, int permission)
    {
      CodeContracts.ArgumentNotNull(permissions, "permissions");

      return permissions.Check((ViewPermissions)permission);
    }

    /// <summary>
    /// The handle request.
    /// </summary>
    /// <param name="permissions">
    /// The permissions.
    /// </param>
    /// <param name="permission">
    /// The permission.
    /// </param>
    public static void HandleRequest([NotNull] this IPermissions permissions, int permission)
    {
      CodeContracts.ArgumentNotNull(permissions, "permissions");

      permissions.HandleRequest((ViewPermissions)permission);
    }

    #endregion
  }
}