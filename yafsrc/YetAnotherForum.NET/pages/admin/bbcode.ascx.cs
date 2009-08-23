using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	public partial class bbcode : YAF.Classes.Core.AdminPage
	{
		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "YafBBCode Extensions", "" );

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
			( ( LinkButton )sender ).Attributes ["onclick"] = "return confirm('Delete this YafBBCode Extension?')";
		}

		protected void bbCodeList_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "add" )
			{
				YafBuildLink.Redirect( ForumPages.admin_bbcode_edit );
			}
			else if ( e.CommandName == "edit" )
			{
				YafBuildLink.Redirect( ForumPages.admin_bbcode_edit, "b={0}", e.CommandArgument );
			}
			else if ( e.CommandName == "delete" )
			{
				DB.bbcode_delete( e.CommandArgument );
				PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.CustomBBCode ) );
				BindData();
			}
			else if ( e.CommandName == "export" )
			{
				List<int> bbCodeIds = GetSelectedBBCodeIDs();

				if ( bbCodeIds.Count > 0 )
				{
					// export this list as XML...
					DataTable dtBBCode = YAF.Classes.Data.DB.bbcode_list( PageContext.PageBoardID, null );
					// remove all but required bbcodes...
					foreach ( DataRow row in dtBBCode.Rows )
					{
						int id = Convert.ToInt32( row ["BBCodeID"] );
						if ( !bbCodeIds.Contains( id ) )
						{
							// remove from this table...
							row.Delete();
						}
					}
					// store delete changes...
					dtBBCode.AcceptChanges();
					
					// export...
					dtBBCode.DataSet.DataSetName = "YafBBCodeList";
					dtBBCode.TableName = "YafBBCode";
					dtBBCode.Columns.Remove( "BBCodeID" );
					dtBBCode.Columns.Remove( "BoardID" );

					Response.ContentType = "text/xml";
					Response.AppendHeader( "Content-Disposition", "attachment; filename=YafBBCodeExport.xml" );
					dtBBCode.DataSet.WriteXml( Response.OutputStream );
					Response.End();
				}
				else
				{
					PageContext.AddLoadMessage( "Nothing selected to export." );
				}
			}
			else if ( e.CommandName == "import" )
			{
				YafBuildLink.Redirect( ForumPages.admin_bbcode_import );
			}
		}

		protected List<int> GetSelectedBBCodeIDs()
		{
			List<int> idList = new List<int>();

			// get checked items....
			foreach ( RepeaterItem item in bbCodeList.Items )
			{
				CheckBox sel = ( CheckBox )item.FindControl( "chkSelected" );
				if ( sel.Checked )
				{
					HiddenField hiddenId = ( HiddenField )item.FindControl( "hiddenBBCodeID" );

					idList.Add( Convert.ToInt32( hiddenId.Value ) );
				}
			}

			return idList;
		}
	}
}
