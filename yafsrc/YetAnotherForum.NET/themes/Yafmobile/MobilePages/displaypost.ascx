<%@ Control Language="c#" AutoEventWireup="True" CodeFile="displaypost.ascx.cs" Inherits="YAF.Controls.DisplayPost"
	EnableViewState="false" %>
<%@ Import Namespace="YAF.Classes.Core"%>
<tr >
<td colspan="3">
<YAF:PopMenu ID="PopMenu1" runat="server" Control="UserName" />
<table id="NameCell" style="width: 100%">
	<tr>
		<td><a name='post<%# DataRow["MessageID"] %>' /><b>
		<YAF:UserLink ID="UserProfileLink" runat="server" Style='<%#DataRow["Style"]%>' UserID='<%#DataRow["UserID"]%>' />
		<YAF:OnlineStatusImage id="OnlineStatusImage" runat="server" Style="vertical-align: bottom" UserID='<%# DataRow["UserID"] %>' Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataRow["UserID"] )%>' />
		</b>
		<div >
			<b>
			<a href='<%# YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}",DataRow["MessageID"]) %>'>
			#<%# Convert.ToInt32((DataRow["Position"]))+1 %></a>
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
			:</b> <%# YafServices.DateTime.FormatDateTime((System.DateTime)DataRow["Posted"]) %>
				<YAF:ThemeButton ID="Attach" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_ATTACH" TitleLocalizedTag="BUTTON_ATTACH_TT" />
		<YAF:ThemeButton ID="Quote" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_QUOTE" TitleLocalizedTag="BUTTON_QUOTE_TT" />
</div>
</td>
	</tr>
	<tr>
		<td>
		<div >
			<asp:Panel id="panMessage" runat="server">      
                <YAF:MessagePostDataMobile ID="MessagePost1" runat="server" DataRow="<%# DataRow %>"></YAF:MessagePostDataMobile>
            </asp:Panel>
		</div>
&nbsp;</td>
	</tr>
	<tr class="postfooter">
		<td class="small postTop" colspan="<%#GetIndentSpan()%>">
		<a href="javascript:scroll(0,0)">
		<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TOP" />
		</a></td>
	</tr>
	<tr class="<%#GetPostClass()%>">
		<td style="padding: 5px;" valign="top"></td>
		
	</tr>
</table>
</td>
</tr>
