<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="../../../controls/displaypost.ascx.cs"
    Inherits="YAF.Controls.DisplayPost" EnableViewState="false" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPostFooter" Src="../../../controls/DisplayPostFooter.ascx" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<tr class="postheader">		
    <%#GetIndentCell()%>
    <td id="NameCell" class="postUser" runat="server">
            <span id="<%# "dvThankBox" + DataRow["MessageID"] %>">
                <YAF:ThemeButton ID="Thank" runat="server" CssClass="yaflittlebutton" Visible="false" TextLocalizedTag="BUTTON_THANKS"
                    TitleLocalizedTag="BUTTON_THANKS_TT" />
            </span>    
            <asp:PlaceHolder ID="buttonsMobile" runat="server" Visible="false">
            <YAF:ThemeButton ID="Attach" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_ATTACH"
                TitleLocalizedTag="BUTTON_ATTACH_TT" />
            <YAF:ThemeButton ID="MovePost" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_MOVE"
                TitleLocalizedTag="BUTTON_MOVE_TT" />
            <YAF:ThemeButton ID="Delete" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_DELETE"
                TitleLocalizedTag="BUTTON_DELETE_TT" />
            <YAF:ThemeButton ID="UnDelete" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_UNDELETE"
                TitleLocalizedTag="BUTTON_UNDELETE_TT" />
            </asp:PlaceHolder>
            <YAF:ThemeButton ID="Edit" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_EDIT"
                TitleLocalizedTag="BUTTON_EDIT_TT" />
            <YAF:ThemeButton ID="Quote" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_QUOTE"
                TitleLocalizedTag="BUTTON_QUOTE_TT" />
        <a name="post<%# DataRow["MessageID"] %>" /><b>						
            <YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%#DataRow["UserID"]%>' Style='<%#DataRow["Style"]%>' />
            <YAF:OnlineStatusImage id="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataRow["UserID"] )%>' Style="vertical-align: bottom" UserID='<%# DataRow["UserID"] %>'  />
        </b>
        <br />
        <div class="leftItem postedLeft">        
            <strong><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                :</strong>
            <%# this.Get<YafDateTime>().FormatDateTime((System.DateTime)DataRow["Posted"]) %>
            </div>
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" class="message">
        <div class="postdiv">
            <asp:panel id="panMessage" runat="server">      
                <YAF:MessagePostData ID="MessagePost1" runat="server" DataRow="<%# DataRow %>"></YAF:MessagePostData>
            </asp:panel>
        </div>
    </td>
</tr>
<tr class="postfooter" runat="server" visible="false">
		<td class="postfooter postInfoBottom">
			<YAF:DisplayPostFooter id="PostFooter" runat="server" DataRow="<%# DataRow %>"></YAF:DisplayPostFooter>
		</td>
</tr>
<tr class="<%#GetPostClass()%>" runat="server" visible="false">
    <td style="padding: 5px;" colspan="2" valign="top">
        <div style="font-weight: bold;" id="<%# "dvThanksInfo" + DataRow["MessageID"] %>">
            <asp:Literal runat="server"  Visible="false" ID="Literal1"></asp:Literal></div>
    </td>
    <td class="message" style="padding: 5px;" valign="top">
        <div id="<%# "dvThanks" + DataRow["MessageID"] %>">
            <asp:Literal runat="server" Visible="false" ID="Literal2"></asp:Literal>
        </div>
    </td>
</tr>
<tr class="postsep">
    <td colspan="1">
        <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
    </td>
</tr>



