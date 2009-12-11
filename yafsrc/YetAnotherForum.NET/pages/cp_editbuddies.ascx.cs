using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using YAF.Classes.Core;
using YAF.Classes;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Controls;

namespace YAF.Pages
{
    public partial class cp_editbuddies : ForumPageRegistered
    {
        /// <summary>
        /// Initializes a new instance of the cp_editbuddies class.
        /// </summary>
        public cp_editbuddies()
            : base("CP_EDITBUDDIES")
        {
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                PageLinks.AddLink(HtmlEncode(PageContext.PageUserName), YafBuildLink.GetLink(ForumPages.cp_profile));
                PageLinks.AddLink(PageContext.Localization.GetText("BUDDYLIST_TT"),"");
            }
            BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.BuddiesTabs.Views["BuddyListTab"].Text = GetText("CP_EDITBUDDIES","BUDDYLIST");
            this.BuddiesTabs.Views["PendingRequestsTab"].Text = GetText("CP_EDITBUDDIES", "PENDING_REQUESTS");
            this.BuddiesTabs.Views["YourRequestsTab"].Text = GetText("CP_EDITBUDDIES", "YOUR_REQUESTS");
            InitializeBuddyList(BuddyList1, 2);
            InitializeBuddyList(PendingBuddyList,3);
            InitializeBuddyList(BuddyRequested,4);
        }

        /// <summary>
        /// Initializes the values of BuddyList control's properties.
        /// </summary>
        /// <param name="customBuddyList">
        /// The custom BuddyList control.
        /// </param>
        /// <param name="Mode">
        /// The mode of this BuddyList.
        /// </param>
        private void InitializeBuddyList(BuddyList customBuddyList, int Mode)
        {
            customBuddyList.CurrentUserID = PageContext.PageUserID;
            customBuddyList.Mode = Mode;
            customBuddyList.Container = this;
        }
    }
}
