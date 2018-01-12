/******************************************************************************************************
//  Original code by: DLESKTECH at http://www.dlesktech.com/support.aspx
//  Modifications by: KASL Technologies at www.kasltechnologies.com
//  Mod date:7/21/2009
//  Mods: working smileys, moved smilies to bottom, added clear button for admin, new stored procedure
//  Mods: fixed the time to show the viewers time not the server time
//  Mods: added small chat window popup that runs separately from forum
//  Note: flyout button opens smaller chat window
//  Note: clear button removes message more than 24hrs old from db
 */

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The shout box.
    /// </summary>
    public partial class ShoutBox : BaseUserControl
    {
        #region Constructors and Destructors

        #endregion

        #region Properties

        /// <summary>
        /// Gets the board identifier.
        /// </summary>
        /// <value>
        /// The board identifier.
        /// </value>
        protected int BoardID
        {
            get
            {
                try
                {
                    if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("board").IsSet())
                    {
                        return this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("board").ToType<int>();
                    }
                    else
                    {
                        return YafContext.Current.PageBoardID;
                    }
                   
                }
                catch (Exception)
                {
                    return YafContext.Current.PageBoardID;
                }
            }
        }

        /// <summary>
        /// Gets ShoutBoxMessages.
        /// </summary>
        public IEnumerable<DataRow> ShoutBoxMessages
        {
            get
            {
                return this.Get<YafDbBroker>().GetShoutBoxMessages(this.BoardID);
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
        /// Changes The CollapsiblePanelState of the ShoutBox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        protected void CollapsibleImageShoutBox_Click([NotNull] object sender, [NotNull] ImageClickEventArgs e)
        {
            this.DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.CollapsibleImageShoutBox.DefaultState = (CollapsiblePanelState)this.Get<YafBoardSettings>().ShoutboxDefaultState;
            
            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Get<YafBoardSettings>().ShowShoutbox
                                         && !this.Get<IPermissions>()
                                                .Check(this.Get<YafBoardSettings>().ShoutboxViewPermissions))
            {
                return;
            }

            if (this.PageContext.IsAdmin)
            {
                this.btnClear.Visible = true;
            }

            this.shoutBoxPanel.Visible = true;

            if (this.IsPostBack)
            {
                return;
            }

            this.btnFlyOut.Text = this.GetText("SHOUTBOX", "FLYOUT");
            this.btnClear.Text = this.GetText("SHOUTBOX", "CLEAR");
            this.btnButton.Text = this.GetText("SHOUTBOX", "SUBMIT");

            this.FlyOutHolder.Visible = !YafControlSettings.Current.Popup;
            this.CollapsibleImageShoutBox.Visible = !YafControlSettings.Current.Popup;

            this.DataBind();
        }

        /// <summary>
        /// Submit new Message
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Submit_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var username = this.PageContext.PageUserName;

            if (username != null && this.messageTextBox.Text != string.Empty)
            {
                LegacyDb.shoutbox_savemessage(
                    this.BoardID,
                    this.messageTextBox.Text,
                    username,
                    this.PageContext.PageUserID,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress());

                this.Get<IDataCache>().Remove(Constants.Cache.Shoutbox);
            }

            this.DataBind();
            this.messageTextBox.Text = string.Empty;

            var scriptManager = ScriptManager.GetCurrent(this.Page);

            if (scriptManager != null)
            {
                scriptManager.SetFocus(this.messageTextBox);
            }
        }

        /// <summary>
        /// Clears the ShoutBox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Clear_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            LegacyDb.shoutbox_clearmessages(this.BoardID);

            // cleared... re-load from cache...
            this.Get<IDataCache>().Remove(Constants.Cache.Shoutbox);
            this.DataBind();
        }

        /// <summary>
        /// Refreshes the ShoutBox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Refresh_Click(object sender, EventArgs e)
        {
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

            this.shoutBoxRepeater.DataSource = this.ShoutBoxMessages;
        }

        #endregion
    }
}