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

namespace YAF.Controls
{
    public partial class EditUsersSuspend : YAF.Classes.Base.BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "DAYS"), "1"));
                SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "HOURS"), "2"));
                SuspendUnit.Items.Add(new ListItem(PageContext.Localization.GetText("PROFILE", "MINUTES"), "3"));
                SuspendUnit.SelectedIndex = 1;
                SuspendCount.Text = "2";
                this.BindData();
            }
        }

        private void BindData()
        {
            using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, Request.QueryString["u"], true))
            {
                if (dt.Rows.Count < 1)
                    yaf_BuildLink.AccessDenied(/*No such user exists*/);
                DataRow user = dt.Rows[0];

                SuspendedRow.Visible = !user.IsNull("Suspended");
                if (!user.IsNull("Suspended"))
                    ViewState["PageContext.SuspendedUntil"] = yaf_DateTime.FormatDateTime(user["Suspended"]);

                RemoveSuspension.Text = PageContext.Localization.GetText("PROFILE", "REMOVESUSPENSION");
                Suspend.Text = PageContext.Localization.GetText("PROFILE", "SUSPEND");
            }        
        }


        /// <summary>
        /// Suspends a user when clicked.
        /// </summary>
        /// <param name="sender">The object sender inherit from Page.</param>
        /// <param name="e">The System.EventArgs inherit from Page.</param>
        private void Suspend_Click(object sender, System.EventArgs e)
        {
            // Admins can suspend anyone not admins
            // Forum Moderators can suspend anyone not admin or forum moderator
            using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, Request.QueryString["u"], null))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (int.Parse(row["PageContext.IsAdmin"].ToString()) > 0)
                    {
                        PageContext.AddLoadMessage(PageContext.Localization.GetText("PROFILE", "ERROR_ADMINISTRATORS"));
                        return;
                    }
                    if (!PageContext.IsAdmin && int.Parse(row["PageContext.IsForumModerator"].ToString()) > 0)
                    {
                        PageContext.AddLoadMessage(PageContext.Localization.GetText("PROFILE", "ERROR_FORUMMODERATORS"));
                        return;
                    }
                }
            }

            DateTime suspend = DateTime.Now;
            int count = int.Parse(SuspendCount.Text);
            switch (SuspendUnit.SelectedValue)
            {
                case "1":
                    suspend += new TimeSpan(count, 0, 0, 0);
                    break;
                case "2":
                    suspend += new TimeSpan(0, count, 0, 0);
                    break;
                case "3":
                    suspend += new TimeSpan(0, 0, count, 0);
                    break;
            }

            YAF.Classes.Data.DB.user_suspend(Request.QueryString["u"], suspend);
            BindData();
        }

        private void RemoveSuspension_Click(object sender, System.EventArgs e)
        {
            YAF.Classes.Data.DB.user_suspend(Request.QueryString["u"], null);
            BindData();
        }

        protected string GetSuspendedTo()
        {
            if (ViewState["PageContext.SuspendedUntil"] != null)
                return (string)ViewState["PageContext.SuspendedUntil"];
            else
                return "";
        }

        protected override void OnInit(EventArgs e)
        {
            RemoveSuspension.Click += new EventHandler(RemoveSuspension_Click);
            Suspend.Click += new EventHandler(Suspend_Click);
            base.OnInit(e);
        }
    }
}