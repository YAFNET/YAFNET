namespace YAF.Classes
{
  using System;

  using Interfaces;

  using Pattern;

  public static class YafProvider
  {
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

    private static ITypeFactoryInstance<IUrlBuilder> _builderFactory = null;

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
  }
}