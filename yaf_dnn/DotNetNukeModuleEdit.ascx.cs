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
		protected DropDownList	BoardID;
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
				}
				DataBind();
				if(ModuleId>0)
				{
					if(Settings["forumboardid"]!=null)
						BoardID.Items.FindByValue(Settings["forumboardid"].ToString()).Selected = true;
				}
			}
		}

		private void InitializeComponent()
		{
		}
	
		override protected void OnInit(EventArgs e)
		{
			Load += new EventHandler(DotNetNukeModuleEdit_Load);
			update.Click += new EventHandler(update_Click);
			cancel.Click += new EventHandler(cancel_Click);
			create.Click += new EventHandler(create_Click);
			base.OnInit(e);
		}

		private void update_Click(object sender, EventArgs e)
		{
			DotNetNuke.ModuleController objModules = new DotNetNuke.ModuleController();
			objModules.UpdateModuleSetting(ModuleId,"forumboardid",BoardID.SelectedValue);
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
	}
}

