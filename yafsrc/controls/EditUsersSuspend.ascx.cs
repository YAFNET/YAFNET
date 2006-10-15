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

namespace yaf.controls
{
    public partial class EditUsersSuspend : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SuspendUnit.Items.Add(new ListItem(ForumPage.GetText("PROFILE", "DAYS"), "1"));
                SuspendUnit.Items.Add(new ListItem(ForumPage.GetText("PROFILE", "HOURS"), "2"));
                SuspendUnit.Items.Add(new ListItem(ForumPage.GetText("PROFILE", "MINUTES"), "3"));
                SuspendUnit.SelectedIndex = 1;
                SuspendCount.Text = "2";
                this.BindData();
            }
        }

        private void BindData()
        {
            using (DataTable dt = DB.user_list(ForumPage.PageBoardID, Request.QueryString["u"], true))
            {
                if (dt.Rows.Count < 1)
                    Data.AccessDenied(/*No such user exists*/);
                DataRow user = dt.Rows[0];

                SuspendedRow.Visible = !user.IsNull("Suspended");
                if (!user.IsNull("Suspended"))
                    ViewState["SuspendedTo"] = ForumPage.FormatDateTime(user["Suspended"]);

                RemoveSuspension.Text = ForumPage.GetText("PROFILE", "REMOVESUSPENSION");
                Suspend.Text = ForumPage.GetText("PROFILE", "SUSPEND");
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
            using (DataTable dt = DB.user_list(ForumPage.PageBoardID, Request.QueryString["u"], null))
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (int.Parse(row["IsAdmin"].ToString()) > 0)
                    {
                        ForumPage.AddLoadMessage(ForumPage.GetText("PROFILE", "ERROR_ADMINISTRATORS"));
                        return;
                    }
                    if (!ForumPage.IsAdmin && int.Parse(row["IsForumModerator"].ToString()) > 0)
                    {
                        ForumPage.AddLoadMessage(ForumPage.GetText("PROFILE", "ERROR_FORUMMODERATORS"));
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

            DB.user_suspend(Request.QueryString["u"], suspend);
            BindData();
        }

        private void RemoveSuspension_Click(object sender, System.EventArgs e)
        {
            DB.user_suspend(Request.QueryString["u"], null);
            BindData();
        }

        protected string GetSuspendedTo()
        {
            if (ViewState["SuspendedTo"] != null)
                return (string)ViewState["SuspendedTo"];
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