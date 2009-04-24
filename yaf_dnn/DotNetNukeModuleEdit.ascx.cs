namespace yaf_dnn
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using DotNetNuke;
    using DotNetNuke.Common;
    using DotNetNuke.Services.Search;
    using DotNetNuke.Entities.Modules;
    using DotNetNuke.Entities.Modules.Actions;
    using DotNetNuke.Services.Localization;
    using DotNetNuke.Services.Exceptions;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Classes.Base;


    /// <summary>
    ///        Summary description for DotNetNukeModule.
    /// </summary>
    public partial class DotNetNukeModuleEdit : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        //protected DropDownList    BoardID, CategoryID;
        //protected LinkButton    update, cancel, create;

        private void DotNetNukeModuleEdit_Load(object sender, System.EventArgs e)
        {
            update.Text = "Update";
            cancel.Text = "Cancel";
            create.Text = "Create New Board";

            update.Visible = base.IsEditable;
            create.Visible = base.IsEditable;

            if (!IsPostBack)
            {
                using (DataTable dt = DB.board_list(DBNull.Value))
                {
                    BoardID.DataSource = dt;
                    BoardID.DataTextField = "Name";
                    BoardID.DataValueField = "BoardID";
                    BoardID.DataBind();
                    if (Settings["forumboardid"] != null)
                    {
                        ListItem item = BoardID.Items.FindByValue(Settings["forumboardid"].ToString());
                        if (item != null) item.Selected = true;
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
            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, "forumboardid", BoardID.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, "forumcategoryid", CategoryID.SelectedValue);

            
            YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.forum);
        }

        private void create_Click(object sender, EventArgs e)
        {
            
            YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.admin_editboard);

        }

        override protected void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.WriteLine("<link rel='stylesheet' type='text/css' href='{0}themes/standard/theme.css'/>", Config.Root);
            base.Render(writer);
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            
            YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.forum);

        }

        private void BoardID_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCategories();
        }

        private void BindCategories()
        {
            using (DataTable dt = DB.category_list(BoardID.SelectedValue, DBNull.Value))
            {
                DataRow row = dt.NewRow();
                row["Name"] = "[All Categories]";
                row["CategoryID"] = DBNull.Value;
                dt.Rows.InsertAt(row, 0);

                CategoryID.DataSource = dt;
                CategoryID.DataTextField = "Name";
                CategoryID.DataValueField = "CategoryID";
                CategoryID.DataBind();

                if (Settings["forumcategoryid"] != null)
                {
                    ListItem item = CategoryID.Items.FindByValue(Settings["forumcategoryid"].ToString());
                    if (item != null) item.Selected = true;
                }
            }
        }
    }
}
