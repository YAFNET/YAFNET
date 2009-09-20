/* YetAnotherForum.NET
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Modules
{
    /// <summary>
    /// Summary description for PageTitleModule
    /// </summary>
    [YafModule("Page Title Module", "Tiny Gecko", 1)]
    public class PagePmPopupModule : SimpleBaseModule
    {

        public PagePmPopupModule()
        {
        }

        override public void InitAfterPage()
        {
            CurrentForumPage.PreRender += new EventHandler(ForumPage_PreRender);
            CurrentForumPage.Load += new EventHandler(ForumPage_Load);
        }

        override public void InitBeforePage()
        {

        }

        void ForumPage_Load(object sender, EventArgs e)
        {
            GeneratePopUp();
        }

        /// <summary>
        /// Creates this pages title and fires a PageTitleSet event if one is set
        /// </summary>
        private void GeneratePopUp()
        {
            // This happens when user logs in
            if (DisplayPMPopup())
            {
                PageContext.AddLoadMessage(String.Format(PageContext.Localization.GetText("COMMON","UNREAD_MSG"), PageContext.UnreadPrivate));
                Mession.LastPm = PageContext.LastUnreadPm;
            }
        }

        protected bool DisplayPMPopup()
        {
            return (PageContext.UnreadPrivate > 0) && (PageContext.LastUnreadPm > Mession.LastPm);

        }

        void ForumPage_PreRender(object sender, EventArgs e)
        {

        }
    }
}