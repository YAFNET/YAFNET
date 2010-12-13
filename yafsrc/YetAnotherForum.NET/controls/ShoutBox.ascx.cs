//*****************************************************************************************************
//  Original code by: DLESKTECH at http://www.dlesktech.com/support.aspx
//  Modifications by: KASL Technologies at www.kasltechnologies.com
//  Mod date:7/21/2009
//  Mods: working smileys, moved smilies to bottom, added clear button for admin, new stored procedure
//  Mods: fixed the time to show the viewers time not the server time
//  Mods: added small chat window popup that runs separately from forum
//  Note: flyout button opens smaller chat window
//  Note: clear button removes message more than 24hrs old from db
//*****************************************************************************************************
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Pattern;
    using YAF.Classes.Utils;
    using YAF.Utilities;

    #endregion

    /// <summary>
    /// The shout box.
    /// </summary>
    public partial class ShoutBox : BaseUserControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ShoutBox" /> class.
        /// </summary>
        public ShoutBox()
        {
            this.PreRender += this.ShoutBox_PreRender;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets CacheKey.
        /// </summary>
        public string CacheKey
        {
            get
            {
                return YafCache.GetBoardCacheKey(Constants.Cache.Shoutbox);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The data bind.
        /// </summary>
        public override void DataBind()
        {
            this.BindData();
            base.DataBind();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The format smilies on click string.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The format smilies on click string.
        /// </returns>
        protected static string FormatSmiliesOnClickString([NotNull] string code, [NotNull] string path)
        {
            code = code.Replace("'", "\'");
            code = code.Replace("\"", "\"\"");
            code = code.Replace("\\", "\\\\");
            string onClickScript = "insertsmiley('{0}','{1}');return false;".FormatWith(code, path);
            return onClickScript;
        }

        /// <summary>
        /// The collapsible image shout box_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CollapsibleImageShoutBox_Click([NotNull] object sender, [NotNull] ImageClickEventArgs e)
        {
            this.DataBind();
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender(EventArgs e)
        {
            YafContext.Current.PageElements.RegisterJsBlockStartup(
                this.shoutBoxUpdatePanel, "DisablePageManagerScrollJs", JavaScriptBlocks.DisablePageManagerScrollJs);

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
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.User != null)
            {
                // phShoutText.Visible = true;
                this.shoutBoxPanel.Visible = true;

                if (this.PageContext.IsAdmin)
                {
                    this.btnClear.Visible = true;
                }
            }

            if (!this.IsPostBack)
            {
                this.btnFlyOut.Text = this.PageContext.Localization.GetText("SHOUTBOX", "FLYOUT");
                this.btnClear.Text = this.PageContext.Localization.GetText("SHOUTBOX", "CLEAR");
                this.btnButton.Text = this.PageContext.Localization.GetText("SHOUTBOX", "SUBMIT");

                this.FlyOutHolder.Visible = !YafControlSettings.Current.Popup;
                this.CollapsibleImageShoutBox.Visible = !YafControlSettings.Current.Popup;

                this.DataBind();
            }
        }

        /// <summary>
        /// The shout box refresh timer_ tick.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ShoutBoxRefreshTimer_Tick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.DataBind();
        }

        /// <summary>
        /// The btn button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string username = this.PageContext.PageUserName;

            if (username != null && this.messageTextBox.Text != String.Empty)
            {
                DB.shoutbox_savemessage(
                    this.PageContext.PageBoardID,
                    this.messageTextBox.Text,
                    username,
                    this.PageContext.PageUserID,
                    this.Get<HttpRequestBase>().UserHostAddress);

                // clear cache...
                this.PageContext.Cache.Remove(this.CacheKey);
            }

            this.DataBind();
            this.messageTextBox.Text = String.Empty;

            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);

            if (scriptManager != null)
            {
                scriptManager.SetFocus(this.messageTextBox);
            }
        }

        /// <summary>
        /// The btn clear_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnClear_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            bool bl = DB.shoutbox_clearmessages(this.PageContext.PageBoardID);

            // cleared... re-load from cache...
            this.PageContext.Cache.Remove(this.CacheKey);
            this.DataBind();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (!this.shoutBoxPlaceHolder.Visible)
            {
                return;
            }

            var shoutBoxMessages = (DataTable)this.PageContext.Cache[this.CacheKey];

            if (shoutBoxMessages == null)
            {
                shoutBoxMessages = DB.shoutbox_getmessages(
                    this.PageContext.PageBoardID,
                    this.PageContext.BoardSettings.ShoutboxShowMessageCount,
                    this.PageContext.BoardSettings.UseStyledNicks);

                // Set colorOnly parameter to false, as we get all color info from data base
                if (this.PageContext.BoardSettings.UseStyledNicks)
                {
                    new StyleTransform(this.PageContext.Theme).DecodeStyleByTable(ref shoutBoxMessages, false);
                }

                var flags = new MessageFlags { IsBBCode = true, IsHtml = false };

                for (int i = 0; i < shoutBoxMessages.Rows.Count; i++)
                {
                    string formattedMessage =
                        YafFormatMessage.FormatMessage(shoutBoxMessages.Rows[i]["Message"].ToString(), flags);

                    // Extra Formating not needed already done tru YafFormatMessage.FormatMessage
                    //formattedMessage = FormatHyperLink(formattedMessage);

                    shoutBoxMessages.Rows[i]["Message"] = formattedMessage;
                }

                // cache for 30 seconds -- could cause problems on web farm configurations.
                this.PageContext.Cache.Add(this.CacheKey, shoutBoxMessages, DateTime.UtcNow.AddSeconds(30));
            }

            this.shoutBoxRepeater.DataSource = shoutBoxMessages;
            if (this.PageContext.BoardSettings.ShowShoutboxSmiles)
            {
                this.smiliesRepeater.DataSource = DB.smiley_listunique(this.PageContext.PageBoardID);
            }
        }

        /// <summary>
        /// The shout box_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ShoutBox_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set timer status based on if the place holder is visible...
            this.shoutBoxRefreshTimer.Enabled = this.shoutBoxPlaceHolder.Visible;
        }

        #endregion
    }
}