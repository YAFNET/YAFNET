namespace yaf_dnn
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using DotNetNuke;
	using yaf;


	/// <summary>
	///		Summary description for DotNetNukeModule.
	/// </summary>
	public class DotNetNukeModuleEdit : PortalModuleControl
	{
		protected DropDownList	BoardID, CategoryID;
		protected LinkButton	update, cancel, create;

		private void DotNetNukeModuleEdit_Load(object sender, System.EventArgs e)
		{
			update.Text = "Update";
			cancel.Text = "Cancel";
			create.Text = "Create New Board";

			//update.Visible = base.IsEditable;
			//create.Visible = base.IsEditable;

			if(!IsPostBack) 
			{
				using(DataTable dt = yaf.DB.board_list(DBNull.Value)) 
				{
					BoardID.DataSource = dt;
					BoardID.DataTextField = "Name";
					BoardID.DataValueField = "BoardID";
					BoardID.DataBind();
					if(Settings["forumboardid"]!=null) 
					{
						ListItem item = BoardID.Items.FindByValue(Settings["forumboardid"].ToString());
						if(item!=null) item.Selected = true;
					}
				}
				BindCategories();
			}
		}

		override protected void OnInit(EventArgs e)
		{
			Load += new EventHandler(DotNetNukeModuleEdit_Load);
			update.Click += new EventHandler(update_Click);
			cancel.Click += new EventHandler(cancel_Click);
			create.Click += new EventHandler(create_Click);
			BoardID.SelectedIndexChanged += new EventHandler(BoardID_SelectedIndexChanged);
			base.OnInit(e);
		}

		private void update_Click(object sender, EventArgs e)
		{
			DotNetNuke.ModuleController objModules = new DotNetNuke.ModuleController();
			objModules.UpdateModuleSetting(ModuleId,"forumboardid",BoardID.SelectedValue);
			objModules.UpdateModuleSetting(ModuleId,"forumcategoryid",CategoryID.SelectedValue);
			yaf.Forum.Redirect(Pages.forum);
		}

		private void create_Click(object sender, EventArgs e)
		{
			yaf.Forum.Redirect(Pages.admin_editboard);
		}

		override protected void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			writer.WriteLine("<link rel='stylesheet' type='text/css' href='{0}themes/standard/theme.css'/>",Config.ConfigSection["root"]);
			base.Render(writer);
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			yaf.Forum.Redirect(Pages.forum);
		}

		private void BoardID_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindCategories();
		}

		private void BindCategories() 
		{
			using(DataTable dt = yaf.DB.category_list(BoardID.SelectedValue,DBNull.Value))
			{
				DataRow row = dt.NewRow();
				row["Name"] = "[All Categories]";
				row["CategoryID"] = DBNull.Value;
				dt.Rows.InsertAt(row,0);

				CategoryID.DataSource = dt;
				CategoryID.DataTextField = "Name";
				CategoryID.DataValueField = "CategoryID";
				CategoryID.DataBind();
				if(Settings["forumcategoryid"]!=null) 
				{
					ListItem item = CategoryID.Items.FindByValue(Settings["forumcategoryid"].ToString());
					if(item!=null) item.Selected = true;
				}
			}
		}
	}
}

