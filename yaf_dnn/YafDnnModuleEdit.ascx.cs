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

namespace YAF.DotNetNuke
{
    /// <summary>
    /// Summary description for DotNetNukeModule.
    /// </summary>
    public partial class YafDnnModuleEdit : PortalModuleBase
    {
        //protected DropDownList    BoardID, CategoryID;
        //protected LinkButton    update, cancel, create;

        private void DotNetNukeModuleEdit_Load(object sender, EventArgs e)
        {
            update.Text = "Update";
            cancel.Text = "Cancel";
            create.Text = "Create New Board";

            update.Visible = IsEditable;
            create.Visible = IsEditable;

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
                        if (item != null)
                            item.Selected = true;
                    }
                }
                BindCategories();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Load += DotNetNukeModuleEdit_Load;
            update.Click += UpdateClick;
            cancel.Click += CancelClick;
            create.Click += CreateClick;
            BoardID.SelectedIndexChanged += BoardIdSelectedIndexChanged;
            base.OnInit(e);
        }

        private void UpdateClick(object sender, EventArgs e)
        {
            ModuleController objModules = new ModuleController();

            objModules.UpdateModuleSetting(ModuleId, "forumboardid", BoardID.SelectedValue);
            objModules.UpdateModuleSetting(ModuleId, "forumcategoryid", CategoryID.SelectedValue);

            YafBuildLink.Redirect(ForumPages.forum);
        }

        private static void CreateClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_editboard);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteLine("<link rel='stylesheet' type='text/css' href='{0}themes/standard/theme.css'/>", "~/DesktopModules/YetAnotherForumDotNet");
            base.Render(writer);
        }

        private static void CancelClick(object sender, EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.forum);
        }

        private void BoardIdSelectedIndexChanged(object sender, EventArgs e)
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
                    if (item != null)
                        item.Selected = true;
                }
            }
        }
    }
}