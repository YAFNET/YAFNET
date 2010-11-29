namespace YAF.Controls
{
    using System;
    using System.Data;

    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Utilities;

    /// <summary>
    /// The last posts.
    /// </summary>
    public partial class LastPosts : BaseUserControl
    {
        /// <summary>
        /// Gets or sets TopicID.
        /// </summary>
        public long? TopicID
        {
            get
            {
                if (ViewState["TopicID"] != null)
                {
                    return Convert.ToInt32(ViewState["TopicID"]);
                }

                return null;
            }

            set
            {
                ViewState["TopicID"] = value;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                this.LastPostUpdatePanel, "DisablePageManagerScrollJs", JavaScriptBlocks.DisablePageManagerScrollJs);

            base.OnPreRender(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.repLastPosts.DataSource = this.TopicID.HasValue
                                               ? DB.post_list_reverse10(this.TopicID.Value).AsEnumerable()
                                               : null;
            this.repLastPosts.DataBind();
        }

        /// <summary>
        /// The last post update timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void LastPostUpdateTimer_Tick(object sender, EventArgs e)
        {
            BindData();
        }
    }
}