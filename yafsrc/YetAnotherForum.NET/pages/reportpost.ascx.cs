/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
 * Written by vzrus (c) 2009 for Yet Another Forum.NET */

namespace YAF.Pages
{
    using System;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;

    /// <summary>
    /// The form for reported post complaint text.
    /// </summary>
    public partial class reportpost : ForumPage
    {
        // message body editor
        /// <summary>
        /// The _editor.
        /// </summary>
        protected YAF.Editors.BaseForumEditor _editor;

        protected int messageID = 0;

        public reportpost()
            : base("REPORTPOST")
        {
        }
        /// <summary>
        /// Page initialization handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Init(object sender, EventArgs e)
        {
            // create editor based on administrator's settings
            this._editor = YafContext.Current.EditorModuleManager.GetEditorInstance(YafContext.Current.BoardSettings.ForumEditor);

            // add editor to the page
            this.EditorLine.Controls.Add(this._editor);
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
            // set attributes of editor
            this._editor.BaseDir = YafForumInfo.ForumRoot + "editors";
            this._editor.StyleSheet = YafContext.Current.Theme.BuildThemePath("theme.css");

            if (!IsPostBack)
            {

                this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

                if (!String.IsNullOrEmpty(Request.QueryString["m"]))
                {
                    // We check here if the user have access to the option
                    if (!PageContext.BoardSettings.AllowGuestToReportPost && PageContext.IsGuest)
                    {
                        Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.info, "i=1"));
                    }

                        if (!Int32.TryParse(Request.QueryString["m"], out messageID))
                        {
                            Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", messageID));
                        }
                        else
                        {
                            MessageIDH.Value = messageID.ToString();
                        }
                    
                }
            }
        }

       
        private void BindData()
        {
            DataBind();
        }

        /// <summary>
        /// The btn run query_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnReport_Click( object sender, EventArgs e )
        {
            DB.message_report( 9, Convert.ToInt32(MessageIDH.Value.Trim()), PageContext.PageUserID, DateTime.Now, this._editor.Text );
            //string link = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", messageID);
            Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", Convert.ToInt32( MessageIDH.Value.Trim() ) ) );
        }
        /// <summary>
        /// The btn cancel query_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {            
            Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( ForumPages.posts, "m={0}#post{0}", Convert.ToInt32( MessageIDH.Value.Trim() ) ) );
        }
    }
}