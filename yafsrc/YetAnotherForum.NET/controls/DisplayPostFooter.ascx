<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DisplayPostFooter.ascx.cs"
	Inherits="YAF.Controls.DisplayPostFooter" %>
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
		<span id="ReportButtons" runat="server">	
			<asp:LinkButton ID="ReportPostLinkButton" CommandName="ReportPost" CommandArgument='<%# DataRow["MessageID"] %>' Text="Report" runat="server" Visible="true"></asp:LinkButton>					
		</span>					
		
		<asp:PlaceHolder ID="MessageHistoryHolder" runat="server" Visible="false">
		&nbsp;|&nbsp;
		<asp:LinkButton ID="MessageHistoryLBtn" CommandName="ShowHistory" CommandArgument='<%# DataRow["MessageID"] %>' Text="Message History" runat="server"></asp:LinkButton>	
		</asp:PlaceHolder>
		<span id="DetailsDelimiter1" runat="server" visible="<%# this.MessageDetails.Text.Length > 0 %>">&nbsp;|&nbsp;</span>
		<asp:Literal id="MessageDetails" runat="server" visible="false" Mode="PassThrough"></asp:Literal>		
	</div>
</div>
