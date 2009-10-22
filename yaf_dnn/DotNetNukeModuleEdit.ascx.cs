#region Usings

using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using YAF.Classes;
using YAF.Classes.Data;
using YAF.Classes.Utils;

#endregion

namespace yaf_dnn
{
	/// <summary>
	///        Summary description for DotNetNukeModule.
	/// </summary>
	public partial class DotNetNukeModuleEdit : PortalModuleBase
	{
		//protected DropDownList    BoardID, CategoryID;
		//protected LinkButton    update, cancel, create;

		private void DotNetNukeModuleEdit_Load( object sender, EventArgs e )
		{
			this.update.Text = "Update";
			this.cancel.Text = "Cancel";
			this.create.Text = "Create New Board";

			this.update.Visible = IsEditable;
			this.create.Visible = IsEditable;

			if ( !IsPostBack )
			{
				using ( DataTable dt = DB.board_list( DBNull.Value ) )
				{
					this.BoardID.DataSource = dt;
					this.BoardID.DataTextField = "Name";
					this.BoardID.DataValueField = "BoardID";
					this.BoardID.DataBind();
					if ( Settings["forumboardid"] != null )
					{
						ListItem item = this.BoardID.Items.FindByValue( Settings["forumboardid"].ToString() );
						if ( item != null )
							item.Selected = true;
					}
				}
				BindCategories();
			}
		}

		protected override void OnInit( EventArgs e )
		{
			Load += DotNetNukeModuleEdit_Load;
			this.update.Click += UpdateClick;
			this.cancel.Click += CancelClick;
			this.create.Click += CreateClick;
			this.BoardID.SelectedIndexChanged += BoardID_SelectedIndexChanged;
			base.OnInit( e );
		}

		private void UpdateClick( object sender, EventArgs e )
		{
			ModuleController objModules = new ModuleController();

			objModules.UpdateModuleSetting( ModuleId, "forumboardid", this.BoardID.SelectedValue );
			objModules.UpdateModuleSetting( ModuleId, "forumcategoryid", this.CategoryID.SelectedValue );

			YafBuildLink.Redirect( ForumPages.forum );
		}

		private void CreateClick( object sender, EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_editboard );
		}

		protected override void Render( HtmlTextWriter writer )
		{
			writer.WriteLine( "<link rel='stylesheet' type='text/css' href='{0}themes/standard/theme.css'/>", Config.Root );
			base.Render( writer );
		}

		private void CancelClick( object sender, EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.forum );
		}

		private void BoardID_SelectedIndexChanged( object sender, EventArgs e )
		{
			BindCategories();
		}

		private void BindCategories()
		{
			using ( DataTable dt = DB.category_list( this.BoardID.SelectedValue, DBNull.Value ) )
			{
				DataRow row = dt.NewRow();
				row["Name"] = "[All Categories]";
				row["CategoryID"] = DBNull.Value;
				dt.Rows.InsertAt( row, 0 );

				this.CategoryID.DataSource = dt;
				this.CategoryID.DataTextField = "Name";
				this.CategoryID.DataValueField = "CategoryID";
				this.CategoryID.DataBind();

				if ( Settings["forumcategoryid"] != null )
				{
					ListItem item = this.CategoryID.Items.FindByValue( Settings["forumcategoryid"].ToString() );
					if ( item != null )
						item.Selected = true;
				}
			}
		}
	}
}