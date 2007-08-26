using System;
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Base
{
  /// <summary>
  /// Runs 
  /// </summary>
  public class YAFInitModule : System.Web.IHttpModule 
  {
    protected int BoardID
    {
      get
      {
        int boardID;

        try { boardID = int.Parse( YAF.Classes.Config.BoardID ); }
        catch { boardID = 1; }

        return boardID;
      }
    }

    #region IHttpModule Members

    void System.Web.IHttpModule.Dispose()
    {

    }

    void System.Web.IHttpModule.Init( System.Web.HttpApplication context )
    {      
      try
      {
        // attempt to sync roles. Assumes a perfect world in which this version is completely up to date... which might not be the case.
        YAF.Classes.Utils.MembershipHelper.SyncRoles( BoardID );
      }
      catch
      {
        // do nothing here--upgrading/DB connectivity issues will be handled in ForumPage.cs
      }
    }

    #endregion
  }
}
