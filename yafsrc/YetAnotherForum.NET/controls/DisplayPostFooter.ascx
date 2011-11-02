<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.DisplayPostFooter" Codebehind="DisplayPostFooter.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<div class="displayPostFooter">
	<div class="leftItem postInfoLeft">
		<YAF:ThemeButton ID="btnTogglePost" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="TOGGLEPOST" Visible="false" />
        <YAF:ThemeButton ID="Albums" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="ALBUM"
			TextLocalizedTag="ALBUMS" ImageThemeTag="ALBUMS" TitleLocalizedTag="ALBUMS_HEADER_TEXT" Visible="false" />
		<YAF:ThemeButton ID="Pm" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="PM" ImageThemeTag="PM" TitleLocalizedTag="PM_TITLE" />
		<YAF:ThemeButton ID="Email" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="EMAIL" ImageThemeTag="EMAIL" TitleLocalizedTag="EMAIL_TITLE" />
		<YAF:ThemeButton ID="Home" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="HOME" ImageThemeTag="HOME" TitleLocalizedTag="HOME_TITLE" />
		<YAF:ThemeButton ID="Blog" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="BLOG" ImageThemeTag="BLOG" TitleLocalizedTag="BLOG_TITLE" />
		<YAF:ThemeButton ID="Msn" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="MSN" ImageThemeTag="MSN" Visible="false" TitleLocalizedTag="MSN_TITLE" />
		<YAF:ThemeButton ID="Aim" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="AIM" ImageThemeTag="AIM" Visible="false" TitleLocalizedTag="AIM_TITLE" />
		<YAF:ThemeButton ID="Yim" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="YIM" ImageThemeTag="YIM" Visible="false" TitleLocalizedTag="YIM_TITLE" />
		<YAF:ThemeButton ID="Icq" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="ICQ" ImageThemeTag="ICQ" Visible="false" TitleLocalizedTag="ICQ_TITLE" />
		<YAF:ThemeButton ID="Xmpp" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="XMPP" ImageThemeTag="XMPP" Visible="false" TitleLocalizedTag="XMPP_TITLE" />	
		<YAF:ThemeButton ID="Skype" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="SKYPE" ImageThemeTag="SKYPE" Visible="false" TitleLocalizedTag="SKYPE_TITLE" />
        <YAF:ThemeButton ID="Facebook" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="FACEBOOK" ImageThemeTag="Facebook2" TitleLocalizedTag="FACEBOOK_TITLE" />
        <YAF:ThemeButton ID="Twitter" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="TWITTER" ImageThemeTag="Twitter2" TitleLocalizedTag="TWITTER_TITLE" />
	</div>
	<div class="rightItem postInfoRight">
        <YAF:ThemeButton ID="ReportPost" runat="server" CssClass="yafcssimagebutton" Visible="false" TextLocalizedPage="POSTS"
				TextLocalizedTag="REPORTPOST" ImageThemeTag="REPORT_POST" TitleLocalizedTag="REPORTPOST_TITLE"></YAF:ThemeButton>					
		<asp:PlaceHolder ID="MessageHistoryHolder" runat="server" Visible="false">
		&nbsp;|&nbsp;
		<a id="messageHistoryLink" rel="nofollow" runat="server" /> 
		</asp:PlaceHolder>
		<span id="DetailsDelimiter1" runat="server" visible="<%# this.MessageDetails.Text.Length > 0 %>">&nbsp;|</span>
		<asp:Literal id="MessageDetails" runat="server" visible="false" Mode="PassThrough"></asp:Literal>
		<span id="IPSpan1" runat="server" visible="false"> 
		&nbsp;|&nbsp;
		<b><%# this.GetText("IP") %>:</b>&nbsp;<a id="IPLink1" target="_blank" runat="server"/>			   
		</span> 		
	</div>
</div>
