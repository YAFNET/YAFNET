/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using YAF.Classes.Pattern;

namespace YAF.Controls
{
    #region

    using System;
    using System.Data;
    using System.Text;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;
    using YAF.Editors;

    #endregion

    /// <summary>
    /// The edit users signature.
    /// </summary>
    public partial class EditUsersSignature : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The string with allowed BBCodes info.
        /// </summary>
        private string _allowedBbcodes;

        /// <summary>
        ///   The string with allowed HTML tags info.
        /// </summary>
        private string _allowedHtml;

        /// <summary>
        ///   The number of characters which is allowed in user signature.
        /// </summary>
        private int _allowedNumberOfCharacters;

        /// <summary>
        ///   The _sig.
        /// </summary>
        private BaseForumEditor _sig;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether InAdminPages.
        /// </summary>
        public bool InAdminPages { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether InModeratorMode.
        /// </summary>
        public bool InModeratorMode { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether ShowHeader.
        /// </summary>
        public bool ShowHeader
        {
            get
            {
                return this.ViewState["ShowHeader"] == null || Convert.ToBoolean(this.ViewState["ShowHeader"]);
            }

            set
            {
                this.ViewState["ShowHeader"] = value;
            }
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        private int CurrentUserID
        {
            get
            {
                if (this.InAdminPages && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
                {
                    return (int)this.PageContext.QueryIDs["u"];
                }

                if (this.InModeratorMode && (this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
                    this.PageContext.QueryIDs.ContainsKey("u"))
                {
                    return (int)this.PageContext.QueryIDs["u"];
                }

                return this.PageContext.PageUserID;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The bind data.
        /// </summary>
        protected void BindData()
        {
            this._sig.Text = DB.user_getsignature(this.CurrentUserID);
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            // since signatures are so small only allow YafBBCode in them...
            this._sig = new BBCodeEditor();
            this.EditorLine.Controls.Add(this._sig);

            this.save.Click += this.Save_Click;
            this.cancel.Click += this.cancel_Click;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent();
            base.OnInit(e);
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
            this.PageContext.QueryIDs = new QueryStringIDHelper("u");

            this._sig.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
            this._sig.StyleSheet = YafContext.Current.Theme.BuildThemePath("theme.css");

            DataTable sigData = DB.user_getsignaturedata(this.CurrentUserID, YafContext.Current.PageBoardID);
            if (sigData.Rows.Count > 0)
            {
                this._allowedBbcodes = sigData.Rows[0]["UsrSigBBCodes"].ToString().Trim().Trim(',').Trim();

                this._allowedHtml = sigData.Rows[0]["UsrSigHTMLTags"].ToString().Trim().Trim(',').Trim();

                this._allowedNumberOfCharacters = sigData.Rows[0]["UsrSigChars"].ToType<int>();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.save.Text = this.PageContext.Localization.GetText("COMMON", "Save");
            this.cancel.Text = this.PageContext.Localization.GetText("COMMON", "Cancel");

            var warningMessage = new StringBuilder();

            warningMessage.Append("<ul>");

            if (this._allowedBbcodes.IsSet())
            {
                warningMessage.AppendFormat(
                    "<li>{0}</li>",
                    this._allowedBbcodes.Contains("ALL")
                        ? this.PageContext.Localization.GetText("BBCODE_ALLOWEDALL")
                        : this.PageContext.Localization.GetTextFormatted("BBCODE_ALLOWEDLIST", this._allowedBbcodes));
            }
            else
            {
                warningMessage.AppendFormat(
              "<li>{0}</li>", this.PageContext.Localization.GetText("BBCODE_FORBIDDEN"));
            }

            if (this._allowedHtml.IsSet())
            {
                warningMessage.AppendFormat(
                   "<li>{0}</li>",
                this._allowedHtml.Contains("ALL")
                         ? this.PageContext.Localization.GetText("HTML_ALLOWEDALL")
                         : this.PageContext.Localization.GetTextFormatted(
                         "HTML_ALLOWEDLIST", this._allowedHtml));
            }
            else
            {
                warningMessage.AppendFormat(
                    "<li>{0}</li>", 
                    this.PageContext.Localization.GetText("HTML_FORBIDDEN"));
            }

            if (this._allowedNumberOfCharacters > 0)
            {
                warningMessage.AppendFormat(
                     "<li>{0}</li>", 
                     this.PageContext.Localization.GetTextFormatted(
                    "SIGNATURE_CHARSMAX", this._allowedNumberOfCharacters));
            }
            else
            {
                warningMessage.AppendFormat(
                   "<li>{0}</li>", this.PageContext.Localization.GetText("SIGNATURE_NOEDIT"));
            }

            warningMessage.Append("</ul>");

            this.TagsAllowedWarning.Text = warningMessage.ToString();

            this.BindData();
        }

        /// <summary>
        /// The page_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.trHeader.Visible = this.ShowHeader;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private static void InitializeComponent()
        {
        }

        /// <summary>
        /// The do redirect.
        /// </summary>
        private void DoRedirect()
        {
            if (this.InModeratorMode)
            {
                YafBuildLink.Redirect(ForumPages.profile, "u={0}", this.CurrentUserID);
            }
            else
            {
                YafBuildLink.Redirect(ForumPages.cp_profile);
            }
        }

        /// <summary>
        /// The save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Save_Click(object sender, EventArgs e)
        {
            string body = this._sig.Text;

            // find forbidden BBcodes in signature
            string detectedBbCode = YafFormatMessage.BBCodeForbiddenDetector(body, this._allowedBbcodes, ',');
            if (this._allowedBbcodes.IndexOf("ALL") < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                        this.PageContext.Localization.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode));
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("BBCODE_FORBIDDEN"));
                    return;
                }
            }

            // find forbidden HTMLTags in signature
            if (!this.PageContext.IsAdmin && this._allowedHtml.IndexOf("ALL") < 0 )
            {
                string detectedHtmlTag = YafFormatMessage.CheckHtmlTags(body, this._allowedHtml, ',');
                if (detectedHtmlTag.IsSet() && detectedHtmlTag != "ALL")
                {
                    this.PageContext.AddLoadMessage(detectedHtmlTag);
                    return;
                }
                if (detectedHtmlTag.IsSet() && detectedHtmlTag == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.PageContext.Localization.GetText("HTML_FORBIDDEN"));
                    return;
                }
            }

            // body = YafFormatMessage.RepairHtml(this,body,false);
            if (this._sig.Text.Length > 0)
            {
                if (this._sig.Text.Length <= this._allowedNumberOfCharacters)
                {
                    DB.user_savesignature(this.CurrentUserID, this.Get<IBadWordReplace>().Replace(body));
                }
                else
                {
                    this.PageContext.AddLoadMessage(
                        this.PageContext.Localization.GetTextFormatted("SIGNATURE_MAX", this._allowedNumberOfCharacters));

                    return;
                }
            }
            else
            {
                DB.user_savesignature(this.CurrentUserID, DBNull.Value);
            }

            // clear the cache for this user...
            UserMembershipHelper.ClearCacheForUserId(this.CurrentUserID);

            if (this.InAdminPages)
            {
                this.BindData();
            }
            else
            {
                this.DoRedirect();
            }
        }

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void cancel_Click(object sender, EventArgs e)
        {
            this.DoRedirect();
        }

        #endregion
    }
}