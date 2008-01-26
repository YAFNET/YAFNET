/* YetAnotherForum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
  // Note: As of AjaxControlToolkit v1.0.10301.0 (build 10531), there is a bug in the Tabs control that prevents some postbacks from 
  // remembering the previous active tab.  
  // See here: http://www.codeplex.com/WorkItem/View.aspx?ProjectName=AtlasControlToolkit&WorkItemId=8255
  //
  // Workaround: 
  // Wrapping the Tabs control in an UpdatePanel, as suggested in forum 
  // discussion: http://forums.asp.net/p/1068120/1579184.aspx


  public partial class cp_pm : YAF.Classes.Base.ForumPage
  {
    private PMView _View;

    public cp_pm()
      : base( "CP_PM" )
    { }

    protected void Page_Load( object sender, EventArgs e )
    {
      if ( User == null || PageContext.IsGuest )
        YafBuildLink.Redirect( ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );

      // check if this feature is disabled
      if ( !PageContext.BoardSettings.AllowPrivateMessages )
        YafBuildLink.Redirect( ForumPages.info, "i=5" );

      if ( !IsPostBack )
      {
        _View = PMView.FromQueryString( Request.QueryString ["v"] );
        if ( _View == PMView.Inbox )
          this.PMTabs.ActiveTab = this.InboxTab;
        else if ( _View == PMView.Outbox )
          this.PMTabs.ActiveTab = this.OutboxTab;
        else if ( _View == PMView.Archive )
          this.PMTabs.ActiveTab = this.ArchiveTab;

        PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
        PageLinks.AddLink( PageContext.PageUserName, YafBuildLink.GetLink( ForumPages.cp_profile ) );
        PageLinks.AddLink( GetText( "TITLE" ) );

        InboxTab.HeaderText = GetText( "INBOX" );
        OutboxTab.HeaderText = GetText( "SENTITEMS" );
        ArchiveTab.HeaderText = GetText( "ARCHIVE" );

				NewPM.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.pmessage );
				NewPM2.NavigateUrl = NewPM.NavigateUrl;

        // DOCTYPE COMPATIBILITY ISSUE
        // Adding this css style block to the page's output fixes rendering of
        // tabs in Internet Explorer quirks mode.
        // If the page's DOCTYPE were set to XHTML 1.0 Transitional, this call would not be needed.
        // This must be attached to the PreRenderComplete event handler so that the css block
        // is registered *after* the ASP.NET AJAX Control Toolkit adds in it's HTML link reference to
        // its default CSS styles.  This is necessary in order for the styles defined below to override the
        // default ones.
       /* this.Page.PreRenderComplete += delegate( object s, EventArgs evt )
           {
             this.RegisterClientCssBlock(
                 ".ajax__tab_default .ajax__tab_inner {height : 100%;}" + "\n" +
                 ".ajax__tab_default .ajax__tab_tab {height : 100%;}" + "\n" +
                 "\n" +
                 ".ajax__tab_xp .ajax__tab_hover .ajax__tab_tab {height : 100%;}" +
                 "\n" +
                 ".ajax__tab_xp .ajax__tab_active .ajax__tab_tab {height : 100%;}" +
                 "\n" +
                 "\n" +
                 ".ajax__tab_xp .ajax__tab_inner {height : 100%;}" + "\n" +
                 ".ajax__tab_xp .ajax__tab_tab {height : 100%;}" + "\n" +
                 ".ajax__tab_xp .ajax__tab_hover .ajax__tab_inner {height : 100%;}" +
                 "\n" +
                 ".ajax__tab_xp .ajax__tab_active .ajax__tab_inner {height : 100%;}" );
           };
			  */
      }
    }

    protected PMView View
    {
      get { return _View; }
    }
  }
}