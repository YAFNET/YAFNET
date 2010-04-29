/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages
{
    // YAF.Pages
    #region Using

    using System;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;
    using YAF.Controls;

    using HistoryEventArgs = nStuff.UpdateControls.HistoryEventArgs;

    #endregion

    /// <summary>
    /// Summary description for view thanks.
    /// </summary>
    public partial class ViewThanks : ForumPage
    {
        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the viewthanks class.
        /// </summary>
        public ViewThanks()
            : base("VIEWTHANKS")
        {
        }
        #endregion
        
        /* Public Methods */
        #region Public Methods
        
        /// <summary>
        /// Initializes the ThanksList controls.
        /// </summary>
        /// <param name="thanksList">The control which is being initialized.</param>
        /// <param name="currentMode">the CurrentMode property of the control.</param>
        /// <param name="userID">the UserID of the control.</param>
        /// <param name="thanksInfo">The Dataview for the control's data.</param>
        public void InitializeThanksList(ViewThanksList thanksList, ThanksListMode currentMode, int userID, DataView thanksInfo)
        {
            thanksList.CurrentMode = currentMode;
            thanksList.UserID = userID;
            thanksList.ThanksInfo = thanksInfo;
        }

        #endregion

        /* Methods */
        #region Methods
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
            var userID = (int)Security.StringToLongOrRedirect(this.Request.QueryString["u"]);
            string displayName = UserMembershipHelper.GetDisplayNameFromID(userID);
            if (!IsPostBack)
            {
                this.PageLinks.Clear();
                this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(displayName, YafBuildLink.GetLink(ForumPages.profile, "u={0}", userID));
                this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);


            }
            DataView thanksInfo = DB.user_viewallthanks(userID).DefaultView;
            InitializeThanksList(ThanksFromList, ThanksListMode.FromUser,userID,thanksInfo);
            InitializeThanksList(ThanksToList, ThanksListMode.ToUser, userID, thanksInfo);

                // Set the DNA Views' titles.
            this.ThanksTabs.Views[0].Text = String.Format(GetText("VIEWTHANKS", "ThanksFromUser"),displayName);
            this.ThanksTabs.Views[1].Text = String.Format(GetText("VIEWTHANKS", "ThanksToUser"),displayName);

        }
        #endregion

    }
}
