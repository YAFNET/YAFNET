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
    public partial class mytopics : ForumPage
    {
        /// <summary>
        /// Initializes a new instance of the mytopics class.
        /// </summary>
        public mytopics()
            : base("MYTOPICS")
        {
        }

        /// <summary>
        /// The Page_ Load Event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.FavoriteTopicsTab.Visible = !PageContext.IsGuest;
                this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                if (PageContext.IsGuest)
                {
                    this.PageLinks.AddLink(GetText("GUESTTITLE"), string.Empty);
                }
                else
                {
                    this.PageLinks.AddLink(GetText("MEMBERTITLE"), string.Empty);
                }

                this.ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;
            }
            
            // Set the DNA Views' titles.
            TopicsTabs.Views[0].Text = GetText("MyTopics", "ActiveTopics");
            if (!PageContext.IsGuest)
            {
                TopicsTabs.Views[1].Text = GetText("MyTopics", "FavoriteTopics");
            }
        }
    }
}
