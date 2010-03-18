/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjшrnar Henden
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
 */

namespace YAF.Pages
{
    // YAF.Pages
    using System;
    using System.Web.Security;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Utils;

    /// <summary>
    /// Class to communicate in XMPP.
    /// </summary>
    public partial class im_xmpp : ForumPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="im_xmpp"/> class.
        /// </summary>
        public im_xmpp()
            : base("IM_XMPP")
        {
        }

        /// <summary>
        /// Gets UserID.
        /// </summary>
        public int UserID
        {
            get
            {
                return (int)Security.StringToLongOrRedirect(Request.QueryString["u"]);
            }
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
            if (User == null)
            {
                YafBuildLink.AccessDenied();
            }

            if (!IsPostBack)
            {
                // get user data...
                MembershipUser userHe = UserMembershipHelper.GetMembershipUserById(this.UserID);
               
                string displayNameHe = UserMembershipHelper.GetDisplayNameFromID(this.UserID);

                this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(!string.IsNullOrEmpty(displayNameHe) ? displayNameHe : userHe.UserName, YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.UserID));
                this.PageLinks.AddLink(GetText("TITLE"), string.Empty);
                
                if (this.UserID == PageContext.PageUserID)
                {
                    NotifyLabel.Text = GetText("SERVERYOU");
                }
                else
                {                    
                    if (userHe == null)
                    {
                        YafBuildLink.AccessDenied(/*No such user exists*/);
                    }
                    
                    // Data for current page user
                    MembershipUser userMe = UserMembershipHelper.GetMembershipUserById(PageContext.PageUserID);
                   
                    // get full user data...
                    var userDataHe = new CombinedUserDataHelper(userHe, this.UserID);
                    var userDataMe = new CombinedUserDataHelper(userMe, PageContext.PageUserID);

                    string serverHe = userDataHe.Profile.XMPP.Substring(userDataHe.Profile.XMPP.IndexOf("@") + 1).Trim();
                    string serverMe = userDataMe.Profile.XMPP.Substring(userDataMe.Profile.XMPP.IndexOf("@") + 1).Trim();
                    if (serverMe == serverHe)
                    {
                        NotifyLabel.Text = GetTextFormatted("SERVERSAME", userDataHe.Profile.XMPP);
                    }
                    else
                    {
                        NotifyLabel.Text = GetTextFormatted("SERVEROTHER", "http://" + serverHe);
                    }
                }
            } 
        }
    }
}
