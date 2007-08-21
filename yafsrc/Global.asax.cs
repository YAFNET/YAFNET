using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Threading;

namespace YAF
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start( object sender, EventArgs e )
		{
      int boardID;

      try
      {
        boardID = int.Parse( YAF.Classes.Config.BoardID );
      }
      catch
      {
        boardID = 1;
      }

			// start by syncing roles to groups
      YAF.Classes.Utils.Security.SyncRoles( boardID );
		}

		protected void Application_End( object sender, EventArgs e )
		{

		}
	}
}