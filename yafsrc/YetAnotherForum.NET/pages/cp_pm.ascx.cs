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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.Pages
{
	public partial class cp_pm : YAF.Classes.Base.ForumPage
	{
		private PMView _View;

		public cp_pm()
			: base("CP_PM")
		{ }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (User == null || PageContext.IsGuest)
				RedirectNoAccess();

			// check if this feature is disabled
			if (!PageContext.BoardSettings.AllowPrivateMessages)
				YafBuildLink.Redirect(ForumPages.info, "i=5");

			if (!IsPostBack)
			{
				_View = PMViewConverter.FromQueryString(Request.QueryString["v"]);
				if (_View == PMView.Inbox)
					this.PMTabs.ActiveTab = this.InboxTab;
				else if (_View == PMView.Outbox)
					this.PMTabs.ActiveTab = this.OutboxTab;
				else if (_View == PMView.Archive)
					this.PMTabs.ActiveTab = this.ArchiveTab;

				PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
				PageLinks.AddLink(PageContext.PageUserName, YafBuildLink.GetLink(ForumPages.cp_profile));
				PageLinks.AddLink(GetText("TITLE"));

				InboxTab.HeaderText = GetText("INBOX");
				OutboxTab.HeaderText = GetText("SENTITEMS");
				ArchiveTab.HeaderText = GetText("ARCHIVE");

				NewPM.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage);
				NewPM2.NavigateUrl = NewPM.NavigateUrl;

				// inbox tab
				ScriptManager.RegisterClientScriptBlock(InboxTabUpdatePanel, typeof(UpdatePanel), "InboxTabRefresh", String.Format("function InboxTabRefresh() {1}\n__doPostBack('{0}', '');\n{2}", InboxTabUpdatePanel.ClientID, '{', '}'), true);
				// sent tab
				ScriptManager.RegisterClientScriptBlock(SentTabUpdatePanel, typeof(UpdatePanel), "SentTabRefresh", String.Format("function SentTabRefresh() {1}\n__doPostBack('{0}', '');\n{2}", SentTabUpdatePanel.ClientID, '{', '}'), true);
				// archive tab
				ScriptManager.RegisterClientScriptBlock(ArchiveTabUpdatePanel, typeof(UpdatePanel), "ArchiveTabRefresh", String.Format("function ArchiveTabRefresh() {1}\n__doPostBack('{0}', '');\n{2}", ArchiveTabUpdatePanel.ClientID, '{', '}'), true);

			}

		}

		protected PMView View
		{
			get { return _View; }
		}
	}
}