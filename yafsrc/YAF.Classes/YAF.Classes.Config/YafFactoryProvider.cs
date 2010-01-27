namespace YAF.Classes
{
  #region Using

  using System;

  using YAF.Classes.Interfaces;
  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// The yaf provider.
  /// </summary>
  public static class YafFactoryProvider
  {
    #region Constants and Fields

    /// <summary>
    /// The _builder factory.
    /// </summary>
    private static ITypeFactoryInstance<IUrlBuilder> _builderFactory = null;

    /// <summary>
    /// The _user display name factory.
    /// </summary>
    private static ITypeFactoryInstance<IUserDisplayName> _userDisplayNameFactory = null;

    #endregion

    #region Properties

    /// <summary>
    /// Gets UrlBuilder.
    /// </summary>
    public static IUrlBuilder UrlBuilder
    {
      get
      {
        if (_builderFactory == null)
        {
          _builderFactory = new TypeFactoryInstanceApplicationBoardScope<IUrlBuilder>(UrlBuilderType);
        }

        return _builderFactory.Get();
      }
    }

    /// <summary>
    /// Gets current <see cref="IUserDisplayName"/>.
    /// </summary>
    public static IUserDisplayName UserDisplayName
    {
      get
      {
        if (_userDisplayNameFactory == null)
        {
          _userDisplayNameFactory = new TypeFactoryInstanceApplicationBoardScope<IUserDisplayName>(UserDisplayNameType);
        }

        return _userDisplayNameFactory.Get();
      }
    }

    /// <summary>
    /// Gets UrlBuilderType.
    /// </summary>
    private static string UrlBuilderType
    {
      get
      {
        string urlAssembly;

        if (!String.IsNullOrEmpty(Config.GetProvider("UrlBuilder")))
        {
          urlAssembly = Config.GetProvider("UrlBuilder");
        }
        else if (Config.IsRainbow)
        {
          urlAssembly = "yaf_rainbow.RainbowUrlBuilder,yaf_rainbow";
        }
        else if (Config.IsDotNetNuke)
        {
          urlAssembly = "YAF.Classes.DotNetNukeUrlBuilder,YAF.Classes.Utils";
        }
        else if (Config.IsMojoPortal)
        {
          urlAssembly = "yaf_mojo.MojoPortalUrlBuilder,yaf_mojo";
        }
        else if (Config.IsPortal)
        {
          urlAssembly = "Portal.UrlBuilder,Portal";
        }
        else if (Config.IsPortalomatic)
        {
          urlAssembly = "Portalomatic.NET.Utils.URLBuilder,Portalomatic.NET.Utils";
        }
        else if (Config.EnableURLRewriting)
        {
          urlAssembly = "YAF.Classes.RewriteUrlBuilder,YAF.Classes.Utils";
        }
        else
        {
          urlAssembly = "YAF.Classes.DefaultUrlBuilder";
        }

        return urlAssembly;
      }
    }

    /// <summary>
    /// Gets UserDisplayNameType.
    /// </summary>
    private static string UserDisplayNameType
    {
      get
      {
        string urlAssembly;

        if (!String.IsNullOrEmpty(Config.GetProvider("UserDisplayName")))
        {
          urlAssembly = Config.GetProvider("UserDisplayName");
        }
        else
        {
          urlAssembly = "YAF.Classes.Core.DefaultUserDisplayName,YAF.Classes.Core";
        }

        return urlAssembly;
      }
    }

    #endregion
  }
}