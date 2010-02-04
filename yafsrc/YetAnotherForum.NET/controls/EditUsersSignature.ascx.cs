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

namespace YAF.Controls
{
    using System;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Utils;
    using YAF.Editors;

    public partial class EditUsersSignature : BaseUserControl
    {
        protected BaseForumEditor _sig;

        public bool ShowHeader
        {
            get
            {
                if (ViewState["ShowHeader"] != null)
                {
                    return Convert.ToBoolean(ViewState["ShowHeader"]);
                }

                return true;
            }
            set
            {
                ViewState["ShowHeader"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PageContext.QueryIDs = new QueryStringIDHelper("u");

            this._sig.BaseDir = YafForumInfo.ForumRoot + "editors";
            this._sig.StyleSheet = YafContext.Current.Theme.BuildThemePath("theme.css");

            if (!IsPostBack)
            {
                save.Text = PageContext.Localization.GetText("COMMON", "Save");
                cancel.Text = PageContext.Localization.GetText("COMMON", "Cancel");

                BindData();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            trHeader.Visible = this.ShowHeader;
        }

        protected void BindData()
        {
            this._sig.Text = YAF.Classes.Data.DB.user_getsignature(CurrentUserID);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            string body = this._sig.Text;
            System.Data.DataTable sigData = YAF.Classes.Data.DB.user_getsignaturedata(CurrentUserID, YafContext.Current.PageBoardID);
            if (sigData.Rows.Count > 0)
            {
                // find forbidden BBcodes in signature
                string detectedBBCode = YAF.Classes.UI.FormatMsg.BBCodeForbiddenDetector(body, sigData.Rows[0]["UsrSigBBCodes"].ToString().Trim().Trim(',').Trim(), ',');
                if (!string.IsNullOrEmpty(detectedBBCode) && detectedBBCode != "ALL")
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBBCode));
                    return;
                }
                else if (detectedBBCode == "ALL")
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("BBCODE_FORBIDDEN"));
                    return;
                }
                // find forbidden HTMLTags in signature
                string detectedHTMLTag = YAF.Classes.UI.FormatMsg.HTMLTagForbiddenDetector(body, sigData.Rows[0]["UsrSigHTMLTags"].ToString().Trim().Trim(',').Trim(), ',');
                if (!string.IsNullOrEmpty(detectedHTMLTag) && detectedHTMLTag != "ALL")
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("HTMLTAG_WRONG", detectedBBCode));
                    return;
                }
                else if (detectedHTMLTag == "ALL")
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("HTMLTAG_FORBIDDEN"));
                    return;
                }
            }
            
            // body = FormatMsg.RepairHtml(this,body,false);

            if (_sig.Text.Length > 0)
            {
                if (_sig.Text.Length <= Convert.ToInt32(sigData.Rows[0]["UsrSigChars"]))
                {
                    YAF.Classes.Data.DB.user_savesignature(this.CurrentUserID, YafServices.BadWordReplace.Replace(body));
                }
                else
                {
                    PageContext.AddLoadMessage(PageContext.Localization.GetTextFormatted("SIGNATURE_MAX", sigData.Rows[0]["UsrSigChars"]));
                    return;
                }
            }
            else
            {
                YAF.Classes.Data.DB.user_savesignature(this.CurrentUserID, DBNull.Value);
            }

            // clear the cache for this user...
            UserMembershipHelper.ClearCacheForUserId(this.CurrentUserID);

            if (this.InAdminPages)
            {
                BindData();
            }
            else
            {
                this.DoRedirect();
            }
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.DoRedirect();
        }

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

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            // since signatures are so small only allow YafBBCode in them...
            _sig = new BBCodeEditor();
            EditorLine.Controls.Add(_sig);

            save.Click += new EventHandler(Save_Click);
            cancel.Click += new EventHandler(cancel_Click);
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        private int CurrentUserID
        {
            get
            {
                if (this.InAdminPages && PageContext.IsAdmin && PageContext.QueryIDs.ContainsKey("u"))
                {
                    return (int)PageContext.QueryIDs["u"];
                }
                else if (this.InModeratorMode && (PageContext.IsAdmin || PageContext.IsModerator) && PageContext.QueryIDs.ContainsKey("u"))
                {
                    return (int)PageContext.QueryIDs["u"];
                }
                else
                {
                    return PageContext.PageUserID;
                }
            }
        }

        protected bool _adminEditMode = false;
        public bool InAdminPages
        {
            get
            {
                return this._adminEditMode;
            }
            set
            {
                this._adminEditMode = value;
            }
        }

        protected bool _moderatorEditMode = false;
        public bool InModeratorMode
        {
            get
            {
                return this._moderatorEditMode;
            }
            set
            {
                this._moderatorEditMode = value;
            }
        }
    }
}