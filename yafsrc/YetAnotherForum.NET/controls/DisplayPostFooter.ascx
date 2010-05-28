<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.DisplayPostFooter" Codebehind="DisplayPostFooter.ascx.cs" %>
<div class="displayPostFooter">
	<div class="leftItem postInfoLeft">
		<YAF:ThemeButton ID="btnTogglePost" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="TOGGLEPOST" Visible="false" />
		<YAF:ThemeButton ID="Pm" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="PM" ImageThemeTag="PM" />
		<YAF:ThemeButton ID="Email" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="EMAIL" ImageThemeTag="EMAIL" />
		<YAF:ThemeButton ID="Home" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="HOME" ImageThemeTag="HOME" />
		<YAF:ThemeButton ID="Blog" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="BLOG" ImageThemeTag="BLOG" />
		<YAF:ThemeButton ID="Msn" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="MSN" ImageThemeTag="MSN" Visible="false" />
		<YAF:ThemeButton ID="Aim" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="AIM" ImageThemeTag="AIM" Visible="false" />
		<YAF:ThemeButton ID="Yim" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="YIM" ImageThemeTag="YIM" Visible="false" />
		<YAF:ThemeButton ID="Icq" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="ICQ" ImageThemeTag="ICQ" Visible="false" />
		<YAF:ThemeButton ID="Xmpp" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="Xmpp" ImageThemeTag="XMPP" Visible="false" />	
		<YAF:ThemeButton ID="Skype" runat="server" CssClass="yafcssimagebutton" TextLocalizedPage="POSTS"
			TextLocalizedTag="SKYPE" ImageThemeTag="SKYPE" Visible="false" />
	</div>
	<div class="rightItem postInfoRight">			
		<a id="reportPostLink" rel="nofollow" runat="server" visible="false" />				
		<asp:PlaceHolder ID="MessageHistoryHolder" runat="server" Visible="false">
		&nbsp;|&nbsp;
		<a id="messageHistoryLink" rel="nofollow" runat="server" /> 
		</asp:PlaceHolder>
		<span id="DetailsDelimiter1" runat="server" visible="<%# this.MessageDetails.Text.Length > 0 %>">&nbsp;|</span>
		<asp:Literal id="MessageDetails" runat="server" visible="false" Mode="PassThrough"></asp:Literal>
		<span id="IPSpan1" runat="server" visible="false"> 
		&nbsp;|&nbsp;
		<b><%# this.PageContext.Localization.GetText("IP") %>:</b><a id="IPLink1" target="_blank" runat="server"/>			   
		</span> 		
	</div>
</div>
