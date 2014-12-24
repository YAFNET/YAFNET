<%@ Control Language="c#" AutoEventWireup="True" CodeBehind="../../../controls/displaypost.ascx.cs"
    Inherits="YAF.Controls.DisplayPost" EnableViewState="false" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPostFooter" Src="../../../controls/DisplayPostFooter.ascx" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
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
            <YAF:ThemeButton ID="Retweet" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_RETWEET"
                TitleLocalizedTag="BUTTON_RETWEET_TT" OnClick="Retweet_Click" />
            <YAF:ThemeButton ID="Edit" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_EDIT"
                TitleLocalizedTag="BUTTON_EDIT_TT" />
            <YAF:ThemeButton ID="Quote" runat="server" CssClass="yaflittlebutton" TextLocalizedTag="BUTTON_QUOTE"
                TitleLocalizedTag="BUTTON_QUOTE_TT" />
            <asp:CheckBox runat="server" ID="MultiQuote" CssClass="MultiQuoteButton"  />
        <a id="post<%# DataRow["MessageID"] %>" name="post<%# DataRow["MessageID"] %>" /><strong>						
            <YAF:OnlineStatusImage id="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( DataRow["UserID"] )%>' Style="vertical-align: bottom" UserID='<%# DataRow["UserID"] %>'  />
            <YAF:ThemeImage ID="ThemeImgSuspended" ThemePage="ICONS" ThemeTag="USER_SUSPENDED"  UseTitleForEmptyAlt="True" Enabled='<%# DataRow["Suspended"] != DBNull.Value && DataRow["Suspended"].ToType<DateTime>() > DateTime.UtcNow %>' runat="server"></YAF:ThemeImage>
            <YAF:UserLink  ID="UserProfileLink" runat="server" UserID='<%# DataRow["UserID"]%>' ReplaceName='<%#  PageContext.BoardSettings.EnableDisplayName && (!DataRow["IsGuest"].ToType<bool>() || (DataRow["IsGuest"].ToType<bool>() && DataRow["DisplayName"].ToString() == DataRow["UserName"].ToString())) ? DataRow["DisplayName"] : DataRow["UserName"]%>' PostfixText='<%# DataRow["IP"].ToString() == "NNTP" ? this.GetText("EXTERNALUSER") : String.Empty %>' Style='<%#DataRow["Style"]%>' CssClass="UserPopMenuLink" EnableHoverCard="False" />            
        </strong>
        &nbsp;<YAF:ThemeButton ID="AddReputation" runat="server" ImageThemeTag="VOTE_UP" Visible="false" TitleLocalizedTag="VOTE_UP_TITLE" OnClick="AddUserReputation"></YAF:ThemeButton>
        <YAF:ThemeButton ID="RemoveReputation" runat="server" ImageThemeTag="VOTE_DOWN" Visible="false" TitleLocalizedTag="VOTE_DOWN_TITLE" OnClick="RemoveUserReputation"></YAF:ThemeButton>
        <br />
        <div class="leftItem postedLeft">        
            <strong><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTED" />
                :</strong>
            <YAF:DisplayDateTime id="DisplayDateTime" runat="server" DateTime='<%# DataRow["Posted"] %>'></YAF:DisplayDateTime>
            </div>
    </td>
</tr>
<tr class="<%#GetPostClass()%>">
    <td valign="top" class="message">
        <div class="postdiv">
            <asp:panel id="panMessage" runat="server">      
                <YAF:MessagePostData ID="MessagePost1" runat="server" DataRow="<%# DataRow %>" IsAltMessage="<%# this.IsAlt %>" ColSpan="<%#GetIndentSpan()%>" ShowEditMessage="True"></YAF:MessagePostData>
            </asp:panel>
        </div>
    </td>
</tr>
<tr class="postfooter" runat="server" visible="false">
		<td class="postfooter postInfoBottom">
			 <span id="IPSpan1" class="rightItem postInfoRight" runat="server" visible="false"> 
		&nbsp;&nbsp;
		<strong><%# this.GetText("IP") %>:</strong>&nbsp;<a id="IPLink1" target="_blank" runat="server"/>			   
	</span> &nbsp;&nbsp;	<YAF:DisplayPostFooter id="PostFooter" runat="server" DataRow="<%# DataRow %>"></YAF:DisplayPostFooter>
		</td>
</tr>
<tr class="<%#GetPostClass()%>" runat="server">
    <td>
        <div style="font-weight: bold;" id="<%# "dvThanksInfo" + DataRow["MessageID"] %>">
            <asp:Literal runat="server"  Visible="false" ID="ThanksDataLiteral"></asp:Literal></div>
        <div id="<%# "dvThanks" + DataRow["MessageID"] %>">
            <asp:Literal runat="server" Visible="false" ID="thanksDataExtendedLiteral"></asp:Literal>
        </div>
    </td>
</tr>
<tr class="postsep">
    <td colspan="1">
        <YAF:PopMenu runat="server" ID="PopMenu1" Control="UserName" />
    </td>
</tr>
