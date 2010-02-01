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
    public partial class ReportPost : ForumPage
    {        
        // messageid

        /// <summary>
        /// To save messageid value.
        /// </summary>
        private int messageID = 0;
       
        // message body editor

        /// <summary>
        /// The _editor.
        /// </summary>
        private YAF.Editors.BaseForumEditor reportEditor;
        //// Class constructor
         
        /// <summary>
        /// Initializes a new instance of the ReportPost class.
        /// </summary>
        public ReportPost()
            : base("REPORTPOST")
        {
        }
        //// public YAF.Editors.BaseForumEditor Editor { get {return _editor}; set; }
        
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
            this.reportEditor = YafContext.Current.EditorModuleManager.GetEditorInstance(YafContext.Current.BoardSettings.ForumEditor);

            // add editor to the page
            this.EditorLine.Controls.Add(this.reportEditor);
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
            this.reportEditor.BaseDir = YafForumInfo.ForumRoot + "editors";
            this.reportEditor.StyleSheet = YafContext.Current.Theme.BuildThemePath("theme.css");
          
                if (!String.IsNullOrEmpty(Request.QueryString["m"]))
                {
                    // We check here if the user have access to the option
                    if (!PageContext.BoardSettings.AllowGuestToReportPost && PageContext.IsGuest)
                    {
                        Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.info, "i=1"));
                    }

                    if (!Int32.TryParse(Request.QueryString["m"], out this.messageID))
                    {
                        Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.error, "Incorrect message value: {0}", this.messageID));
                    }            
                }

                if (!IsPostBack)
                {                   
                    // Get reported message text for better quoting
                    System.Data.DataTable messageRow = DB.message_list(this.messageID);
                    if (messageRow.Rows.Count > 0)
                    {
                        // populate the message preview with the message datarow...
                        MessagePreview.Message = messageRow.Rows[0]["message"].ToString();
                        MessagePreview.MessageFlags.BitValue = Convert.ToInt32(messageRow.Rows[0]["Flags"]);
                        UserProfileLink.UserID = Convert.ToInt32(messageRow.Rows[0]["UserID"]);
                    }
                   
                    // Get Forum Link
                    this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                }
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
        protected void BtnReport_Click(object sender, EventArgs e)
        {
            // Save the reported message
            DB.message_report(7, this.messageID, PageContext.PageUserID, DateTime.Now, this.reportEditor.Text);
            //// Redirect to reported post
            this.RedirectToPost(); 
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
        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            // Redirect to reported post
            this.RedirectToPost();  
        }

        /// <summary>
        /// Redirects to reported post after Save or Cancel
        /// </summary>
        protected void RedirectToPost()
        {
            // Redirect to reported post
            Response.Redirect(YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "m={0}#post{0}", this.messageID));            
        }
        
        /// <summary>
        /// Binds data to data source
        /// </summary>
        private void BindData()
        {
            DataBind();
        }
    }
}