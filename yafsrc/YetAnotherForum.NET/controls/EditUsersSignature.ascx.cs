﻿/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Editors;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
        private ForumEditor _sig;

        /// <summary>
        ///  The signature Preview
        /// </summary>
        private SignaturePreview signaturePreview;

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
        public int CurrentUserID
        {
            get
            {
                if (this.InAdminPages && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
                {
                    return this.PageContext.QueryIDs["u"].ToType<int>();
                }

                if (this.InModeratorMode && (this.PageContext.IsAdmin || this.PageContext.IsModerator) &&
                    this.PageContext.QueryIDs.ContainsKey("u"))
                {
                    return this.PageContext.QueryIDs["u"].ToType<int>();
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
            this._sig.Text = LegacyDb.user_getsignature(this.CurrentUserID);

            this.signaturePreview.Signature = this._sig.Text;
            this.signaturePreview.DisplayUserID = this.CurrentUserID;
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // since signatures are so small only allow YafBBCode in them...
            this._sig = new BBCodeEditor();
            this.EditorLine.Controls.Add(this._sig);

            this.signaturePreview = new SignaturePreview();
            this.PreviewLine.Controls.Add(this.signaturePreview);
       
            this.save.Click += this.Save_Click;
            this.preview.Click += this.Preview_Click;
            this.cancel.Click += this.Cancel_Click;

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
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper("u");

            this._sig.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
            this._sig.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");

            DataTable sigData = LegacyDb.user_getsignaturedata(this.CurrentUserID, YafContext.Current.PageBoardID);
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

            this.save.Text = this.GetText("COMMON", "SAVE");
            this.preview.Text = this.GetText("COMMON", "PREVIEW");
            this.cancel.Text = this.GetText("COMMON", "CANCEL");

            var warningMessage = new StringBuilder();

            warningMessage.Append("<ul>");

            if (this._allowedBbcodes.IsSet())
            {
                warningMessage.AppendFormat(
                  "<li>{0}</li>",
                  this._allowedBbcodes.Contains("ALL")
                    ? this.GetText("BBCODE_ALLOWEDALL")
                    : this.GetTextFormatted("BBCODE_ALLOWEDLIST", this._allowedBbcodes));
            }
            else
            {
                warningMessage.AppendFormat("<li>{0}</li>", this.GetText("BBCODE_FORBIDDEN"));
            }

            if (this._allowedHtml.IsSet())
            {
                warningMessage.AppendFormat(
                  "<li>{0}</li>",
                  this._allowedHtml.Contains("ALL")
                    ? this.GetText("HTML_ALLOWEDALL")
                    : this.GetTextFormatted("HTML_ALLOWEDLIST", this._allowedHtml));
            }
            else
            {
                warningMessage.AppendFormat("<li>{0}</li>", this.GetText("HTML_FORBIDDEN"));
            }

            warningMessage.AppendFormat(
                "<li>{0}</li>",
                this._allowedNumberOfCharacters > 0
                    ? this.GetTextFormatted("SIGNATURE_CHARSMAX", this._allowedNumberOfCharacters)
                    : this.GetText("SIGNATURE_NOEDIT"));

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
        protected void Page_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.trHeader.Visible = this.ShowHeader;

            // Setup Pagination js
            YafContext.Current.PageElements.RegisterJsResourceInclude("paginationjs", "js/jquery.pagination.js");
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
        private void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string body = this._sig.Text;

            // find forbidden BBcodes in signature
            string detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this._allowedBbcodes, ',');
            if (this._allowedBbcodes.IndexOf("ALL") < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                      this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode));
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("BBCODE_FORBIDDEN"));
                    return;
                }
            }

            // find forbidden HTMLTags in signature
            if (!this.PageContext.IsAdmin && this._allowedHtml.IndexOf("ALL") < 0)
            {
                string detectedHtmlTag = this.Get<IFormatMessage>().CheckHtmlTags(body, this._allowedHtml, ',');
                if (detectedHtmlTag.IsSet() && detectedHtmlTag != "ALL")
                {
                    this.PageContext.AddLoadMessage(detectedHtmlTag);
                    return;
                }

                if (detectedHtmlTag.IsSet() && detectedHtmlTag == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("HTML_FORBIDDEN"));
                    return;
                }
            }

            // body = this.Get<IFormatMessage>().RepairHtml(this,body,false);
            if (this._sig.Text.Length > 0)
            {
                if (this._sig.Text.Length <= this._allowedNumberOfCharacters)
                {
                    LegacyDb.user_savesignature(this.CurrentUserID, this.Get<IBadWordReplace>().Replace(body));
                }
                else
                {
                    this.PageContext.AddLoadMessage(
                      this.GetTextFormatted("SIGNATURE_MAX", this._allowedNumberOfCharacters));

                    return;
                }
            }
            else
            {
                LegacyDb.user_savesignature(this.CurrentUserID, DBNull.Value);
            }

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

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
        /// Update the Signature Preview.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            string body = this._sig.Text;

            // find forbidden BBcodes in signature
            string detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this._allowedBbcodes, ',');
            if (this._allowedBbcodes.IndexOf("ALL") < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                      this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode));
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("BBCODE_FORBIDDEN"));
                    return;
                }
            }

            // find forbidden HTMLTags in signature
            if (!this.PageContext.IsAdmin && this._allowedHtml.IndexOf("ALL") < 0)
            {
                string detectedHtmlTag = this.Get<IFormatMessage>().CheckHtmlTags(body, this._allowedHtml, ',');
                if (detectedHtmlTag.IsSet() && detectedHtmlTag != "ALL")
                {
                    this.PageContext.AddLoadMessage(detectedHtmlTag);
                    return;
                }

                if (detectedHtmlTag.IsSet() && detectedHtmlTag == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("HTML_FORBIDDEN"));
                    return;
                }
            }

            if (this._sig.Text.Length <= this._allowedNumberOfCharacters)
            {
                this.signaturePreview.Signature = this.Get<IBadWordReplace>().Replace(body);
                this.signaturePreview.DisplayUserID = this.CurrentUserID;
            }
            else
            {
                this.PageContext.AddLoadMessage(
                  this.GetTextFormatted("SIGNATURE_MAX", this._allowedNumberOfCharacters));

                return;
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
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.DoRedirect();
        }

        #endregion
    }
}