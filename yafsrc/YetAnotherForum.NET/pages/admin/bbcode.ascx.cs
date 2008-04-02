using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	public partial class bbcode : YAF.Classes.Base.AdminPage
	{
		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "BBCode Extensions", "" );

				BindData();
			}
		}

		private void BindData()
		{
			bbCodeList.DataSource = DB.bbcode_list( PageContext.PageBoardID, null );
			DataBind();
		}

		protected void Delete_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton )sender ).Attributes ["onclick"] = "return confirm('Delete this BBCode Extension?')";
		}

		protected void bbCodeList_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "add" )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_bbcode_edit );
			}
			else if ( e.CommandName == "edit" )
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_bbcode_edit, "b={0}", e.CommandArgument );
			}
			else if ( e.CommandName == "delete" )
			{
				DB.bbcode_delete( e.CommandArgument );
				YafCache.Current.Remove( YafCache.GetBoardCacheKey( Constants.Cache.CustomBBCode ) );
				BindData();
			}
			else if ( e.CommandName == "export" )
			{
				// export this list as XML...
				DataTable extensionList = YAF.Classes.Data.DB.bbcode_list( PageContext.PageBoardID, null );
				extensionList.DataSet.DataSetName = "YafBBCodeList";
				extensionList.TableName = "YafBBCode";
				extensionList.Columns.Remove( "BBCodeID" );
				extensionList.Columns.Remove( "BoardID" );

				Response.ContentType = "text/xml";
				Response.AppendHeader( "Content-Disposition", "attachment; filename=YafBBCodeExport.xml" );
				extensionList.DataSet.WriteXml( Response.OutputStream );
				Response.End();
			}
			else if ( e.CommandName == "import" )
			{
				YafBuildLink.Redirect( ForumPages.admin_bbcode_import );
			}
		}
	}
}
